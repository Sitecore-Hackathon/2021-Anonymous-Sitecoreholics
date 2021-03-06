using System.Threading.Tasks;

namespace Speedo.Foundation.RequestHandler.Rendering.Persistance
{
    /// <summary>
    /// Speedo Blob Storage Persistence
    /// </summary>
    class BlobStorageProvider : IProvider
    {
        public Task<string> GetLayoutServiceContent(string item, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}