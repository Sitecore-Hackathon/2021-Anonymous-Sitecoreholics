using Sitecore.Xml;
using System.Collections.Generic;
using System.Xml;

namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{
    public class StorageConfiguration
    {
        private readonly List<StorageSource> _sources = new List<StorageSource>();

        protected void AddSource(XmlNode node)
        {
            _sources.Add(ParseSource(node));
        }

        public IEnumerable<StorageSource> Sources => _sources;

        private StorageSource ParseSource(XmlNode node)
        {
            if (node == null || !node.LocalName.Equals("source"))
            {
                return null;
            }

            var path = XmlUtil.GetAttribute("path", node, string.Empty).ToLowerInvariant();
            var site = XmlUtil.GetAttribute("site", node, string.Empty).ToLowerInvariant();

            return new StorageSource(path, site);
        }
    }
}