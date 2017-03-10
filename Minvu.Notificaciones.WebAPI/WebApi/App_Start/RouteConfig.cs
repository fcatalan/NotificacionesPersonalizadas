using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApi
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			//      routes.IgnoreRoute("{resource}.sso/{*pathInfo}");//PSSIM 12-01-2017

			routes.MapRoute(
					name: "Default",
					url: "{controller}/{action}/{id}",
					defaults: new { id = RouteParameter.Optional, format = RouteParameter.Optional }
			);

			routes.MapRoute(
				"Root",
				"",
				new { controller = "Home", action = "Index", id = "" }
				);
		}
	}
}
