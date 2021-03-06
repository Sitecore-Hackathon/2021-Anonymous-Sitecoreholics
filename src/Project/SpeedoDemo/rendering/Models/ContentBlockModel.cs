using Sitecore.LayoutService.Client.Response.Model.Fields;

namespace Speedo.Project.SpeedoDemo.Rendering.Models
{
    /// <summary>
    /// An example of binding to component fields (usually a serialized datasource item).
    /// </summary>
    public class ContentBlockModel
    {
        public TextField Title { get; set; }

        public RichTextField Text { get; set; }
    }
}
