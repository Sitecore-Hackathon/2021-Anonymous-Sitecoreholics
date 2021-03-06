using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class SaveSnapshot
    {
        private readonly ChildListOptions _childListOptions;
        private readonly ILayoutService _layoutService;
        private readonly ISerializerService _serializerService;
        private readonly IConfiguration _layoutServiceConfiguration;

        public SaveSnapshot(ILayoutService layoutService, ISerializerService serializerService, IConfiguration layoutServiceConfiguration)
        {
            _layoutService = layoutService;
            _serializerService = serializerService;
            _layoutServiceConfiguration = layoutServiceConfiguration;
            _childListOptions = ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity | ChildListOptions.AllowReuse;
        }

        public void Process(UpdateStorageArgs args)
        {
            Log.Info($"Saving snapshot...", this);

            var watch = Stopwatch.StartNew();
            var sourceDatabase = Factory.GetDatabase(args.SourceDatabaseName);
            var device = sourceDatabase.Resources.Devices["/sitecore/layout/devices/default"];
            var root = sourceDatabase.GetItem("/sitecore/content"/* TODO: move to config and/or args */);
            var items = CollectItemsWithLayout(root, device);
            var config = _layoutServiceConfiguration.GetNamedConfiguration("default" /* TODO: move to config */);

            foreach (var item in items)
            {
                // save all language versions
                foreach (var language in item.Languages)
                {
                    var version = ItemManager.GetItem(item.ID, language, Sitecore.Data.Version.Latest, item.Database, SecurityCheck.Disable);

                    if (version.Versions.Count == 0)
                    {
                        continue;
                    }

                    RenderedItem renderedItem;

                    try
                    {
                        renderedItem = _layoutService.Render(version, config.RenderingConfiguration);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to render item {version.Uri}.", ex, this);

                        continue;
                    }

                    var jsonString = _serializerService.Serialize(renderedItem, config.SerializationConfiguration);

                    System.IO.File.WriteAllText($"C:\\speedo\\{version.ID:D}.{version.Language.Name}.json", jsonString, System.Text.Encoding.UTF8);

                    Log.Info($"TEST json: {jsonString}", this);
                }

                if (!IsMediaWithBlob(item))
                {
                    continue;
                }

                // TODO: save blob
            }

            Log.Info($"Saving snapshot took {watch.Elapsed}.", this);
        }

        private IEnumerable<Item> CollectItemsWithLayout(Item root, DeviceItem device)
        {
            var items = new List<Item>();

            var hasLayout = root.Visualization.GetLayout(device) != null;

            if (hasLayout)
            {
                items.Add(root);
            }

            foreach (Item child in root.GetChildren(_childListOptions))
            {
                items.AddRange(CollectItemsWithLayout(child, device));
            }

            return items;
        }

        private bool IsMediaWithBlob(Item item)
        {
            // Item within media library but not a folder...
            var isMediaItem = item.Paths.IsMediaItem && item.TemplateID != TemplateIDs.MediaFolder;

            if (!isMediaItem)
            {
                return false;
            }

            // In some cases there can exist items in media library that are not based on any media template type
            var hasBlobField = item.Fields["Blob"] != null;

            // If item does not have a blob field it is not considered a media item...
            if (!hasBlobField)
            {
                return false;
            }

            return true;
        }
    }
}