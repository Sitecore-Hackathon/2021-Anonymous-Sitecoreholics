using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Speedo.Feature.ContentComponents.Rendering.Models
{
    /// <summary>
    /// A model used for creating a Header
    /// </summary>
    public class HeaderModel
    {
        public TextField Title { get; set; }
        public TextField MenuHeader { get; set; }
    }
}
