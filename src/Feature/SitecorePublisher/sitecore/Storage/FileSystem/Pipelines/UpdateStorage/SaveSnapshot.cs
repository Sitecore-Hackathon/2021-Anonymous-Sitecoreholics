using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class SaveSnapshot
    {
        private readonly ChildListOptions _childListOptions;
        private readonly WebClient _http;
        private readonly string _endpoint;

        public SaveSnapshot()
        {
            _childListOptions = ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity | ChildListOptions.AllowReuse;
            _http = new WebClient();
            _endpoint = "http://localhost/sitecore/api/layout/render/jss";
        }

        public void Process(UpdateStorageArgs args)
        {
            Log.Info($"Speedo: saving snapshot...", this);

            var watch = Stopwatch.StartNew();
            var sourceDatabase = Factory.GetDatabase(args.SourceDatabaseName);
            var device = sourceDatabase.Resources.Devices["/sitecore/layout/devices/default"];

            foreach (var source in args.Sources)
            {
                var site = Factory.GetSite(source.SiteName);

                // process content items
                var sitePath = site.RootPath + site.StartItem;
                var sourceItem = sourceDatabase.Items.GetItem(sitePath);

                if (sourceItem == null)
                {
                    throw new Exception($"Speedo source '{sitePath}' not found in database '{args.SourceDatabaseName}'.");
                }

                var rootUrl = $"{_endpoint}?sc_apikey={source.ApiKey}&sc_site={source.SiteName}";
                var itemsFileRootPath = Path.Combine(source.OutputRootPath, "content");
                var urlBuilderOptions = new ItemUrlBuilderOptions
                {
                    AddAspxExtension = false,
                    AlwaysIncludeServerUrl = false,
                    EncodeNames = true,
                    LanguageEmbedding = LanguageEmbedding.Never,
                    Site = site,
                    LowercaseUrls = true
                };

                var items = CollectItems(sourceItem).Where(x => x.Visualization.GetLayout(device) != null);

                foreach (var item in items)
                {
                    foreach (var language in item.Languages)
                    {
                        var version = ItemManager.GetItem(item.ID, language, Sitecore.Data.Version.Latest, item.Database, SecurityCheck.Disable);

                        if (version.Versions.Count == 0)
                        {
                            continue;
                        }

                        SaveItem(version, itemsFileRootPath, rootUrl, urlBuilderOptions);
                    }
                }

                // process media items
                sourceItem = sourceDatabase.Items.GetItem(source.MediaPath);

                if (sourceItem == null)
                {
                    throw new Exception($"Speedo source '{source.MediaPath}' not found in database '{args.SourceDatabaseName}'.");
                }

                var mediasFileRootPath = Path.Combine(source.OutputRootPath, "media");

                items = CollectItems(sourceItem).Where(x => IsMediaWithBlob(x));

                foreach (var item in items)
                {
                    SaveMedia(item, mediasFileRootPath);
                }
            }

            Log.Info($"Speedo: saving snapshot took {watch.Elapsed}.", this);
        }

        private void SaveItem(Item item, string fileRootPath, string rootUrl, ItemUrlBuilderOptions urlBuilderOptions)
        {
            var path = LinkManager.GetItemUrl(item, urlBuilderOptions).Trim('/');
            string filePath;

            if (string.IsNullOrEmpty(path))
            {
                filePath = Path.Combine(fileRootPath, $"{item.Language.Name}.json").ToLowerInvariant();
            }
            else
            {
                filePath = Path.Combine(fileRootPath, $"{path}\\{item.Language.Name}.json").ToLowerInvariant();
            }

            var url = $"{rootUrl}&item={item.ID:D}&sc_lang={item.Language.Name}";

            Log.Info($"Speedo: saving '{url}' as '{filePath}'...", this);

            string json;

            try
            {
                json = _http.DownloadString(url);
            }
            catch (Exception ex)
            {
                Log.Error($"Speedo: failed to get json for item '{item.Paths.FullPath}'.", ex, this);

                return;
            }

            FileInfo file = new FileInfo(filePath);

            // if the directory already exists, this method does nothing.
            file.Directory.Create();

            // save json
            File.WriteAllText(file.FullName, json, Encoding.UTF8);
        }

        private void SaveMedia(MediaItem media, string fileRootPath)
        {
            var blobField = media.InnerItem.Fields["Blob"];

            if (string.IsNullOrEmpty(blobField?.Value))
            {
                return;
            }

            var path = MediaManager.GetMediaUrl(media, new MediaUrlBuilderOptions { AlwaysIncludeServerUrl = false, AbsolutePath = false, IncludeExtension = true, LowercaseUrls = true })
                .Replace(Settings.Media.MediaLinkPrefix, "")
                .Trim('/')
                .Replace("/", "\\");
            var filePath = Path.Combine(fileRootPath, path).ToLowerInvariant();

            FileInfo file = new FileInfo(filePath);

            // if the directory already exists, this method does nothing.
            file.Directory.Create();

            // don't save again if we already have saved it once
            if (file.Exists)
            {
                Log.Info($"Speedo: skipping save of '{media.InnerItem.Paths.FullPath}' as '{filePath}', file exists...", this);

                return;
            }

            Log.Info($"Speedo: saving '{media.InnerItem.Paths.FullPath}' as '{filePath}'...", this);

            // save blob
            using (var blob = media.GetMediaStream())
            {
                if (blob == null)
                {
                    Log.Error($"Speedo: could not load blob on '{media.InnerItem.Paths.FullPath}'.", this);
                }
                else
                {
                    using (var fileStream = file.Create())
                    {
                        blob.Seek(0, SeekOrigin.Begin);
                        blob.CopyTo(fileStream);
                    }
                }
            }
        }

        private IEnumerable<Item> CollectItems(Item root)
        {
            yield return root;

            foreach (Item child in root.GetChildren(_childListOptions))
            {
                foreach (Item sub in CollectItems(child))
                {
                    yield return sub;
                }
            }
        }

        private bool IsMediaWithBlob(Item item)
        {
            // item within media library but not a folder...
            var isMediaItem = item.Paths.IsMediaItem && item.TemplateID != TemplateIDs.MediaFolder;

            if (!isMediaItem)
            {
                return false;
            }

            // in some cases there can exist items in media library that are not based on any media template type
            var hasBlobField = item.Fields["Blob"] != null;

            // if item does not have a blob field it is not considered a media item...
            if (!hasBlobField)
            {
                return false;
            }

            return true;
        }
    }
}