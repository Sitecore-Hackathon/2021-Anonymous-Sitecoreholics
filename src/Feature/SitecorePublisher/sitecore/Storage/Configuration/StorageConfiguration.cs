using Sitecore.Xml;
using System.Collections.Generic;
using System.Xml;

namespace Speedo.Feature.SitecorePublisher.Storage.Configuration
{

    public class StorageConfiguration
    {
        private readonly List<Source> _sources = new List<Source>();

        protected void AddSource(XmlNode node)
        {
            _sources.Add(ParseSource(node));
        }

        public IEnumerable<Source> GetSources() => _sources;

        private Source ParseSource(XmlNode node)
        {
            if (node == null || !node.LocalName.Equals("source"))
            {
                return null;
            }

            var path = XmlUtil.GetAttribute("path", node, string.Empty).ToLowerInvariant();

            return new Source(path);
        }

        public class Source
        {
            public Source(string path)
            {
                Path = path;
            }

            public string Path { get; }
        }
    }
}