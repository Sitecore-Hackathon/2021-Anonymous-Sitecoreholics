using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.LayoutService.Client;
using Sitecore.LayoutService.Client.Exceptions;
using Sitecore.LayoutService.Client.Request;
using Sitecore.LayoutService.Client.Response;
using Speedo.Foundation.RequestHandler.Rendering.Persistance;

namespace Speedo.Foundation.RequestHandler.Rendering

{
    /// <summary>
    /// Implmentation of ILayoutRequest Handler that uses persisted layout service results
    /// </summary>
    public class LayoutRequestHandler : ILayoutRequestHandler
    {
        private IProvider _persistencyProvider;
        private readonly ISitecoreLayoutSerializer _layoutSerializer;
        private readonly ILogger<LayoutRequestHandler> _logger;

        public LayoutRequestHandler(IProvider persistencyProvider, ISitecoreLayoutSerializer layoutSerializer, ILogger<LayoutRequestHandler> logger)
        {
            _persistencyProvider = persistencyProvider;
            _layoutSerializer = layoutSerializer;
            _logger = logger;
        }

        public async Task<SitecoreLayoutResponse> Request(SitecoreLayoutRequest request, string handlerName)
        {
            string itemPath = (string)request["item"];
            string itemLang = (string)request["sc_lang"];

            if (string.IsNullOrEmpty(itemPath))
            {
                throw new Exception($"{nameof(itemPath)} is nut defined");
            }

            if (string.IsNullOrEmpty(itemLang))
            {
                throw new Exception($"{nameof(itemLang)} is nut defined");
            }

            //Get persisted layout service result
            string contentAsText = await _persistencyProvider.GetLayoutServiceContent(itemPath, itemLang);

            if (string.IsNullOrWhiteSpace(contentAsText))
            {
                //Register as 404 if the result was not found
                return new SitecoreLayoutResponse(request, new List<SitecoreLayoutServiceClientException>
                {
                    new ItemNotFoundSitecoreLayoutServiceClientException()
                });
            }

            //Try to deserialize the content
            SitecoreLayoutResponseContent content = null;
            try
            {
                content = _layoutSerializer.Deserialize(contentAsText);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Could not parse persisted response as layout service content");
                }

                return new SitecoreLayoutResponse(request, new List<SitecoreLayoutServiceClientException>
                {
                    new InvalidRequestSitecoreLayoutServiceClientException()
                });
            }

            //Return a working layout response
            return new SitecoreLayoutResponse(request, new List<SitecoreLayoutServiceClientException>())
            {
                Content = content
            };
        }
    }
}
