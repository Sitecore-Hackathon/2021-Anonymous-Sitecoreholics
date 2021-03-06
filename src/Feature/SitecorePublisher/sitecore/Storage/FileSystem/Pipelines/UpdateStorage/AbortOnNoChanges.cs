using Sitecore.Diagnostics;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class AbortOnNoChanges
    {
        public void Process(UpdateStorageArgs args)
        {
            Log.Info($"Speedo: checking if we can abort...", this);

            // TODO: if no items was published within args.Sources (check root item on publish options), abort pipeline...
        }
    }
}