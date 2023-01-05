using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            /*routes.MapRoute(
                name: "Login",
                url: "Views/login",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );*/

            /*routes.MapRoute(
                "HomeController",
                "{controller}/{action}/{flight_num}",
                new { controller = "HomeController", action = "RemoveFlights", flight_num = UrlParameter.Optional }
            );*/

        }
    }
}
