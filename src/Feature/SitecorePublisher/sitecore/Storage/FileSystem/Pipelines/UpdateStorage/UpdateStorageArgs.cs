using Sitecore.Pipelines;
using Speedo.Feature.SitecorePublisher.Storage.Configuration;
using System.Collections.Generic;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class UpdateStorageArgs : PipelineArgs
    {
        public UpdateStorageArgs(string sourceDatabaseName, IEnumerable<StorageSource> sources)
        {
            SourceDatabaseName = sourceDatabaseName;
            Sources = sources;
        }

        public string SourceDatabaseName { get; }
        public IEnumerable<StorageSource> Sources { get; }
    }
}