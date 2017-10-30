using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.OData.Builder;
using MissionsService;
using System.Web.OData.Extensions;
using MissionsService.Models;

namespace MissionsService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Конфигурация и службы веб-API

            ODataModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Mission>("Missions");
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel());

            // Маршруты веб-API
           // config.MapHttpAttributeRoutes();
           //
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new {controller="Home", id = RouteParameter.Optional }
            );

        }
    }
}
