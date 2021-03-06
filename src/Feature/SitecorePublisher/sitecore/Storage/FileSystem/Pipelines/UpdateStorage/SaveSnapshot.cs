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
        private readonly WebClient _http;

        public SaveSnapshot()
        {
            _childListOptions = ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity | ChildListOptions.AllowReuse;
            _http = new WebClient();
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
            var fileRootPath = @"C:\speedo"; // TODO: move to config
            var itemsFileRootPath = Path.Combine(fileRootPath, "content");
            var mediasFileRootPath = Path.Combine(fileRootPath, "media");

            foreach (var source in args.Sources)
            {
                var sourceItem = sourceDatabase.Items.GetItem(source.Path);

                if (sourceItem == null)
                {
                    throw new Exception($"Speedo source item '{source.Path}' not found in database '{args.SourceDatabaseName}'.");
                }

                var rootUrl = $"{hostname}{endpoint}?sc_apikey={apiKey}&sc_site={source.SiteName}";
                var items = CollectItems(sourceItem);

                foreach (var item in items)
                {
                    if (IsMediaWithBlob(item))
                    {
                        SaveMedia(item, mediasFileRootPath);
                    }
                    else
                    {
                        var hasLayout = item.Visualization.GetLayout(device) != null;

                        if (hasLayout)
                        {
                            foreach (var language in item.Languages)
                            {
                                var version = ItemManager.GetItem(item.ID, language, Sitecore.Data.Version.Latest, item.Database, SecurityCheck.Disable);

                                if (version.Versions.Count == 0)
                                {
                                    continue;
                                }

                                SaveItem(version, itemsFileRootPath, rootUrl);
                            }
                        }
                    }
                }
            }

            Log.Info($"Speedo: saving snapshot took {watch.Elapsed}.", this);
        }

        private void SaveItem(Item item, string fileRootPath, string rootUrl)
        {
            var url = $"{rootUrl}&item={item.ID:D}&sc_lang={item.Language.Name}";
            var filePath = Path.Combine(fileRootPath, $"{item.Paths.ContentPath.Replace("/", "\\").TrimStart('\\')}\\{item.Language.Name}.json");

            Log.Info($"Speedo: saving '{url}' as '{filePath}'...", this);

            string json;

            try
            {
                json = _http.DownloadString(url);
            }
            catch (Exception ex)
            {
                Log.Error($"Speedo: failed to get json for item '{item.Paths.FullPath}'.", ex, this);

                return;
            }

            FileInfo file = new FileInfo(filePath);

            // if the directory already exists, this method does nothing.
            file.Directory.Create();

            File.WriteAllText(file.FullName, json, Encoding.UTF8);
        }

        private void SaveMedia(MediaItem media, string fileRootPath)
        {
            var blobField = media.InnerItem.Fields["Blob"];

            if (string.IsNullOrEmpty(blobField?.Value))
            {
                return;
            }

            var filePath = Path.Combine(fileRootPath, $"{media.InnerItem.Paths.FullPath.Replace("/sitecore/media library", "").Replace("/", "\\").TrimStart('\\')}.{media.Extension}");

            Log.Info($"Speedo: saving '{media.InnerItem.Paths.FullPath}' as '{filePath}'...", this);

            using (var blob = media.GetMediaStream())
            {
                if (blob == null)
                {
                    Log.Warn($"Speedo: could not load blob on '{media.InnerItem.Paths.FullPath}'...", this);
                }
                else
                {
                    FileInfo file = new FileInfo(filePath);

                    // if the directory already exists, this method does nothing.
                    file.Directory.Create();

                    using (var fileStream = file.Create())
                    {
                        blob.Seek(0, SeekOrigin.Begin);
                        blob.CopyTo(fileStream);
                    }
                }
            }
        }

        private IEnumerable<Item> CollectItems(Item root)
        {
            yield return root;

            foreach (Item child in root.GetChildren(_childListOptions))
            {
                foreach (Item sub in CollectItems(child))
                {
                    yield return sub;
                }
            }
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