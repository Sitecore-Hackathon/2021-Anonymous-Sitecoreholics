using Sitecore.Diagnostics;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class AbortOnNoChanges
    {

        public AbortOnNoChanges()
        {
        }

        public void Process(UpdateStorageArgs args)
        {
            Log.Info($"Checking for changes...", this);
        }
    }
}