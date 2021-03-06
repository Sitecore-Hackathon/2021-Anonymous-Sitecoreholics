using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace ContentComponents.Models
{
    /// <summary>
    /// A component used for creating an article component
    /// </summary>
    public class ArticleModel
    {
        public TextField Title { get; set; }

        public RichTextField Text { get; set; }       
        
        public ImageField Image { get; set; }

        public TextField Author { get; set; }
    }
}
