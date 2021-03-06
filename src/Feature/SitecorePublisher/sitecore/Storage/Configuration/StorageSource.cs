namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{
    public class StorageSource
    {
        public StorageSource(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}