using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Speedo.Feature.SitecorePublisher.Storage.FileSystem.Pipelines.UpdateStorage
{
    public class SaveSnapshot
    {
        private readonly ChildListOptions _childListOptions;

        public SaveSnapshot()
        {
            _childListOptions = ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity | ChildListOptions.AllowReuse;
        }

        public void Process(UpdateStorageArgs args)
        {
            Log.Info($"Speedo: saving snapshot...", this);

            var watch = Stopwatch.StartNew();
            var sourceDatabase = Factory.GetDatabase(args.SourceDatabaseName);
            var device = sourceDatabase.Resources.Devices["/sitecore/layout/devices/default"]; // TODO: move to config
            var hostname = "http://cm"; // TODO: move to config
            var endpoint = "/sitecore/api/layout/render/jss"; // TODO: move to config
            var apiKey = "3c22a88c-600a-414b-87ca-2aee4e998fa4"; // TODO: move to config
            var fileRootPath = "C:\\speedo"; // TODO: move to config
            var client = new WebClient();

            try
            {
                foreach (var source in args.Sources)
                {
                    var sourceItem = sourceDatabase.Items.GetItem(source.Path);

                    if (sourceItem == null)
                    {
                        throw new Exception($"Speedo source item '{source.Path}' not found in database '{args.SourceDatabaseName}'.");
                    }

                    var items = CollectItemsWithLayout(sourceItem, device);

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

                            var url = $"{hostname}{endpoint}?sc_apikey={apiKey}&sc_site={source.SiteName}&item={version.ID:D}&sc_lang={version.Language.Name}";
                            var filePath = $"{fileRootPath}\\{version.Paths.ContentPath:D}\\{version.Language.Name}.json";

                            Log.Info($"Speedo: saving '{url}' as '{filePath}'...", this);

                            string json;

                            try
                            {
                                json = client.DownloadString(url);
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Speedo: failed to get json for item '{version.Paths.FullPath}'.", ex, this);

                                continue;
                            }

                            FileInfo file = new FileInfo(filePath);

                            // if the directory already exists, this method does nothing.
                            file.Directory.Create();

                            File.WriteAllText(file.FullName, json, Encoding.UTF8);
                        }

                        if (!IsMediaWithBlob(item))
                        {
                            continue;
                        }

                        // TODO: save blob
                    }
                }
            }
            finally
            {
                client.Dispose();
            }

            Log.Info($"Speedo: saving snapshot took {watch.Elapsed}.", this);
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
            // item within media library but not a folder...
            var isMediaItem = item.Paths.IsMediaItem && item.TemplateID != TemplateIDs.MediaFolder;

            if (!isMediaItem)
            {
                return false;
            }

            // in some cases there can exist items in media library that are not based on any media template type
            var hasBlobField = item.Fields["Blob"] != null;

            // if item does not have a blob field it is not considered a media item...
            if (!hasBlobField)
            {
                return false;
            }

            return true;
        }
    }
}