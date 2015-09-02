using Microsoft.AspNet.WebApi.MessageHandlers.Compression;
using Microsoft.AspNet.WebApi.MessageHandlers.Compression.Compressors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace MetabolApi
{
    using System.Web.Http.Cors;

    using WebApiContrib.Formatting.Jsonp;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            GlobalConfiguration.Configuration.MessageHandlers.Insert(0, new ServerCompressionHandler(
                                        new GZipCompressor(),
                                        new DeflateCompressor()));
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter(new BrowserJsonFormatter()));
            config.Formatters.Add(new BrowserJsonFormatter());
        }

        public class BrowserJsonFormatter : JsonMediaTypeFormatter
        {
            public BrowserJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                this.SerializerSettings.Formatting = Formatting.None;
                this.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                this.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
