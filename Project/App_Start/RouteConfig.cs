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

            /*routes.MapRoute(
                name: "OrderUserFlights",
                url: "{controller}/OrderUserFlights/{username}/",
                defaults: new { controller = "Home", action = "OrderUserFlights", username = UrlParameter.Optional,  }
            );*/

            routes.MapRoute(
                name: "OrderFlights",
                url: "{controller}/OrderUserFlights/{username}/{reset}/",
                defaults: new { controller = "Home", action = "OrderUserFlights", username = UrlParameter.Optional, reset = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SearchFlights",
                url: "{controller}/SearchFlights/{username}/",
                defaults: new { controller = "Home", action = "SearchFlights", username = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ShowTickets",
                url: "{controller}/ShowTickets/{username}/",
                defaults: new { controller = "Home", action = "ShowTickets", username = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ShowFlightsUsername",
                url: "{controller}/ShowFlightsUser/{username}/",
                defaults: new { controller = "Home", action = "ShowFlightsUser", username = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BookFlights",
                url: "{controller}/BookFlights/{username}/{id}",
                defaults: new { controller = "Home", action = "BookFlights", username = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "RemoveFlights",
                url: "{controller}/RemoveFlights/{id}",
                defaults: new { controller = "Home", action = "RemoveFlights", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EditFlights",
                url: "{controller}/EditFlights/{id}",
                defaults: new { controller = "Home", action = "EditFlights", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
