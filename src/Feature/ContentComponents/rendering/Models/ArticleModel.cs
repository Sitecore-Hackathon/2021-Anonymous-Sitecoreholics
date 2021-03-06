using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Speedo.Feature.ContentComponents.Rendering.Models
{
    /// <summary>
    /// A model used for creating an article
    /// </summary>
    public class ArticleModel
    {
        public TextField Title { get; set; }

        public RichTextField Text { get; set; }       
        
        public ImageField Image { get; set; }

        public TextField Author { get; set; }
    }
}
