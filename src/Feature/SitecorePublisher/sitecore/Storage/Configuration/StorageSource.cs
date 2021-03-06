namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{
    public class StorageSource
    {
        public StorageSource(string mediaPath, string siteName)
        {
            MediaPath = mediaPath;
            SiteName = siteName;
        }

        public string MediaPath { get; }
        public string SiteName { get; }
    }
}