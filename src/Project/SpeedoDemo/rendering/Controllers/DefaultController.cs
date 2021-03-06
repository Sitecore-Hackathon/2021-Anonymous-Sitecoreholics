using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sitecore.AspNet.RenderingEngine;
using Sitecore.AspNet.RenderingEngine.Filters;
using Sitecore.LayoutService.Client.Exceptions;
using Speedo.Project.SpeedoDemo.Rendering.Models;
using System.Net;

namespace Speedo.Project.SpeedoDemo.Rendering.Controllers
{
    /// <summary>
    /// The entry point controller for this sample site.
    /// </summary>
    public class DefaultController : Controller
    {
        /// <summary>
        /// The entry point action for this sample site. Use of the <c>[UseSitecoreRendering]</c>
        /// attribute means that requests mapped to this action will cause requests to the
        /// Sitecore Layout Service. Model binding extensions for the Layout Service result can
        /// therefore also be used.
        /// </summary>
        [UseSitecoreRendering]
        public IActionResult Index(PageModel page)
        {
            var request = HttpContext.GetSitecoreRenderingContext();

            if (request.Response.HasErrors)
            {
                foreach (var error in request.Response.Errors)
                {
                    switch (error)
                    {
                        case ItemNotFoundSitecoreLayoutServiceClientException notFound:
                            Response.StatusCode = (int)HttpStatusCode.NotFound;
                            return View("NotFound", request);
                        case InvalidRequestSitecoreLayoutServiceClientException badRequest:
                        case CouldNotContactSitecoreLayoutServiceClientException transportError:
                        case InvalidResponseSitecoreLayoutServiceClientException serverError:
                        default:
                            throw error;
                    }
                }
            }

            return View(page);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            return View(new ErrorViewModel
            {
                // Some information to help with debugging a common error response from the Layout Service.
                IsInvalidRequest =
                    exceptionHandlerPathFeature?.Error is InvalidRequestSitecoreLayoutServiceClientException
            });
        }
    }
}
