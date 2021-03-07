namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{
    public class StorageSource
    {
        public StorageSource(string mediaPath, string siteName, string apiKey, string outputRootPath)
        {
            MediaPath = mediaPath;
            SiteName = siteName;
            ApiKey = apiKey;
            OutputRootPath = outputRootPath;
        }

        public string MediaPath { get; }
        public string SiteName { get; }
        public string ApiKey { get; }
        public string OutputRootPath { get; }
    }
}