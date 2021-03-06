using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace ContentComponents.Models
{
    /// <summary>
    /// A component used for creating a banner with the possibility to include navigation  
    /// </summary>
    public class BannerModel
    {
        public TextField Title { get; set; }

        public RichTextField Text { get; set; }

        public HyperLinkField NavigationLink { get; set; }
        
        public TextField Style { get; set; }

        public ImageField Image { get; set; }

        public TextField Author { get; set; }
    }
}
