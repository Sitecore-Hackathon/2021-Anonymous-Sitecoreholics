using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.LayoutService.Client;
using Sitecore.LayoutService.Client.Extensions;
using Speedo.Foundation.RequestHandler.Rendering.Options;
using Speedo.Foundation.RequestHandler.Rendering.Persistance;

namespace Speedo.Foundation.RequestHandler.Rendering.Extensions
{
    public static class SpeedoBuilderExtensions
    {
        /// <summary>
        /// Adds the Speedo Handler to manage Layout Service Requests
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handlerName"></param>
        /// <returns></returns>
        public static ILayoutRequestHandlerBuilder<LayoutRequestHandler> AddSpeedoHandler(
            this ISitecoreLayoutClientBuilder builder,
            string handlerName,
            IConfiguration configuration)
        {
            //Register Options
            builder.Services.Configure<SpeedoOptions>(configuration.GetSection(SpeedoOptions.Key));

            //Register Handler
            return builder.AddHandler<LayoutRequestHandler>(handlerName);
        }

        /// <summary>
        /// Registers a File System provider for Speedo
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ILayoutRequestHandlerBuilder<THandler> WithDiskPersistency<THandler>(
            this ILayoutRequestHandlerBuilder<THandler> builder)
            where THandler : ILayoutRequestHandler
        {
            //Register the File System Provider
            builder.Services.AddSingleton<IProvider, FileSystemProvider>();

            return builder;
        }

        public static ILayoutRequestHandlerBuilder<THandler> WithBlobStoragePersistency<THandler>(
            this ILayoutRequestHandlerBuilder<THandler> builder)
            where THandler : ILayoutRequestHandler
        {
            //Register Blob Storage Provider
            builder.Services.AddSingleton<IProvider, BlobStorageProvider>();
            return builder;
        }
    } 
}
