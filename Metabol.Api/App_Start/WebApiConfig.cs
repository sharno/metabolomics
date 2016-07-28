using System;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;

namespace Metabol.Api
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
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
        /// <summary>
        /// 
        /// </summary>
        public class BrowserJsonFormatter : JsonMediaTypeFormatter
        {
            /// <summary>
            /// 
            /// </summary>
            public BrowserJsonFormatter()
            {
                this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
                this.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
                this.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                this.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            }

            /// <summary> Sets the default headers for content that will be formatted using this formatter. This method is called from the <see cref="T:System.Net.Http.ObjectContent" /> constructor. This implementation sets the Content-Type header to the value of mediaType if it is not null. If it is null it sets the Content-Type to the default media type of this formatter. If the Content-Type does not specify a charset it will set it using this formatters configured <see cref="T:System.Text.Encoding" />. </summary>
            /// <param name="type">The type of the object being serialized. See <see cref="T:System.Net.Http.ObjectContent" />.</param>
            /// <param name="headers">The content headers that should be configured.</param>
            /// <param name="mediaType">The authoritative media type. Can be null.</param>
            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                base.SetDefaultContentHeaders(type, headers, mediaType);
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
        }
    }
}
