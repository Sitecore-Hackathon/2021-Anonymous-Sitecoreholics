using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Speedo.Feature.ContentComponents.Rendering.Models
{
    /// <summary>
    /// A model used for creating a Hero banner 
    /// </summary>
    public class HeroModel
    {
        public RichTextField Text { get; set; }

        public HyperLinkField NavigationLink { get; set; }

        public TextField NavigationLinkText { get; set; }
    }
}
