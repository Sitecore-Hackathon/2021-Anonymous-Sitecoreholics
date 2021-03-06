using Sitecore.Configuration;
using Speedo.Feature.SitecorePublisher.Storage.Configuration;
using System;

namespace Speedo.Feature.SitecorePublisher
{
    public class SpeedoConfiguration
    {
        private static readonly Lazy<StorageConfiguration> _storage = new Lazy<StorageConfiguration>(() =>
        {
            var storageNode = Factory.GetConfigNode("speedo/storage", true);

            return Factory.CreateObject<StorageConfiguration>(storageNode);
        });

        public StorageConfiguration Storage
        {
            get
            {
                return _storage.Value;
            }
        }
    }
}