using Sitecore.Events;
using Sitecore.Pipelines;
using Sitecore.Publishing;
using Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Speedo.Feature.SitecorePublisher.Events
{
    public class PublishCompleteHandler
    {
        public void RunUpdateStoragePipeline(object sender, EventArgs e)
        {
            var args = e as SitecoreEventArgs;
            var options = args?.Parameters[0] as IEnumerable<DistributedPublishOptions>;

            if (options == null)
            {
                return;
            }

            var publishOptions = options.First();
            var pipelineArgs = new UpdateStorageArgs(publishOptions.TargetDatabaseName);

            CorePipeline.Run("updatestorage", pipelineArgs, "speedo", true);
        }

    }
}
