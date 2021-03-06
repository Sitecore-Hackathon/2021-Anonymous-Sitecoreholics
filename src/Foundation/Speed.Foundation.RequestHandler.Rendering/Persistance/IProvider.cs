using System.Threading.Tasks;

namespace Speedo.Foundation.RequestHandler.Rendering.Persistance
{
    /// <summary>
    /// A provider to read persistent content from Sitecore 
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Returns static json persisted from the layout service
        /// </summary>
        /// <param name="item">The Sitecore item (path) to retrieve content for</param>
        /// <param name="language">The language version of the item to retrieve</param>
        /// <returns></returns>
        Task<string> GetLayoutServiceContent(string item, string language);
    }
}
