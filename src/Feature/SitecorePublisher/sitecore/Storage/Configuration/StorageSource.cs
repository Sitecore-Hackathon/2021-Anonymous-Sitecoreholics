namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{
    public class StorageSource
    {
        public StorageSource(string path, string siteName)
        {
            Path = path;
            SiteName = siteName;
        }

        public string Path { get; }
        public string SiteName { get; }
    }
}