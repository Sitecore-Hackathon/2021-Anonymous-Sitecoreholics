using Sitecore.Pipelines;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class UpdateStorageArgs : PipelineArgs
    {
        public UpdateStorageArgs(string sourceDatabaseName)
        {
            SourceDatabaseName = sourceDatabaseName;
        }

        public string SourceDatabaseName { get; }
    }
}