using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Speedo.Foundation.RequestHandler.Rendering.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Speedo.Foundation.RequestHandler.Rendering.Persistance
{
    /// <summary>
    /// Speedo File System Persistence
    /// </summary>
    public class FileSystemProvider : IProvider
    {
        private readonly SpeedoOptions _options;
        private readonly ILogger<FileSystemProvider> _logger;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="options">Options for the service</param>
        /// <param name="logger">Logger</param>
        public FileSystemProvider(IOptions<SpeedoOptions> options, ILogger<FileSystemProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public Task<string> GetLayoutServiceContent(string item, string language)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentException($"{nameof(item)} is not specified");
            }

            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentException($"{nameof(language)} is not specified");
            }

            var path = BuildPath(item, language);

            if (!File.Exists(path))
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"couldn't find path layout service content in path: {path}");
                }
                return Task.FromResult(string.Empty);
            }

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"Reading file: {path} for {item} in language {language}");
            }

            return File.ReadAllTextAsync(path);
        }

        /// <summary>
        /// Builds the path of the persisted file 
        /// </summary>
        /// <param name="item">The path of the Sitecore item </param>
        /// <param name="language">The language version of the Sitecore item</param>
        /// <returns></returns>
        private string BuildPath(string item, string language)
        {
            if (string.IsNullOrEmpty(_options.LayoutServiceContentFilePath))
            {
                throw new Exception($"{nameof(_options.LayoutServiceContentFilePath)} is not specified");
            }

            //TODO: Use a shared layer?
            string path = item.Replace("/", "\\");
            path = Path.Combine(path, $"{language}.json");
            return Path.Join(_options.LayoutServiceContentFilePath, path);
        }
    }
}