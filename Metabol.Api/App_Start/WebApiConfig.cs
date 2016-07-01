using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.WebApi.MessageHandlers.Compression;
using Microsoft.AspNet.WebApi.MessageHandlers.Compression.Compressors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;

namespace Metabol.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
           
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api2/{controller}/{id}"
                //defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.MessageHandlers.Insert(0, new ServerCompressionHandler(
                                     new GZipCompressor(),
                                     new DeflateCompressor()));
            GlobalConfiguration.Configuration.Formatters.Insert(0, new BrowserJsonFormatter());
            config.Formatters.Add(new BrowserJsonFormatter());
        }
        public class BrowserJsonFormatter : JsonMediaTypeFormatter
        {
            public BrowserJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                this.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                this.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                this.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
