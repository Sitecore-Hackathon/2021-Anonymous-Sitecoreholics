using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using Sitecore.ApplicationCenter.Applications;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Shell.Framework.Commands.IDE;


namespace Speedo.Feature.ICanHazDadJokeProvider.Events
{
    public class ICanHazDadJokeHandler
    {
        public void OnItemCreated(object sender, EventArgs e)
        {
            var args = e as SitecoreEventArgs;
            var options = args?.Parameters[0] as ItemCreatedEventArgs;
            var item = options?.Item;
            if (item == null)
            {
                return;
            }
            if (!item.TemplateID.ToString().Equals(Constants.BannerTemplate)) return;
            
            var client = new ICanHazDadJokeClient(Constants.LibraryName);

            var image = client.GetRandomJokeAsImageAsync();
            var joke = client.GetRandomJokeAsync().Result;

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                //@TODO: Error when reading media stream - to be fixed if time allows
                // var mediaItem = UploadImage(image);
                Sitecore.Diagnostics.Log.Error("Psw2:" + joke.DadJoke, this);

                using (new EditContext(item))
                {
                    item.Fields[Constants.BannerTextField].Value = joke.DadJoke;
                   // AssignMediaItem(item.Fields[Constants.BannerImageField], mediaItem);
                }
            }
        }

        private static MediaItem UploadImage(Task<Stream> image)
        {
            var options = new Sitecore.Resources.Media.MediaCreatorOptions();
            options.Database = Sitecore.Configuration.Factory.GetDatabase("master");
            options.Language = Sitecore.Globalization.Language.Parse(Sitecore.Configuration.Settings.DefaultLanguage);
            options.Versioned = Sitecore.Configuration.Settings.Media.UploadAsVersionableByDefault;
            options.Destination = Constants.MediaLocationJokeImages + Guid.NewGuid().ToString().Replace("{", "").Replace("}","");
            options.FileBased = Sitecore.Configuration.Settings.Media.UploadAsFiles;
            var creator = new Sitecore.Resources.Media.MediaCreator();
            return creator.CreateFromStream(image.Result, ".png", options);
        }

        private static void AssignMediaItem(ImageField field, Item mediaItem)
        {
            field.MediaID = mediaItem.ID;
            field.SetAttribute("mediapath", mediaItem.Paths.MediaPath);
            field.SetAttribute("showineditor", "1");
        }
    }
}
