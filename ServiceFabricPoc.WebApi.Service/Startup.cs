﻿using System.Web.Http;
using Owin;

namespace ServiceFabricPoc.WebApi.Service
{
    public static class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public static void ConfigureApp(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}
