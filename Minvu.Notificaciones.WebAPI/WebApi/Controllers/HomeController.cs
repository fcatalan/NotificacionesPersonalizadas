using Minvu.Notificaciones.Domain.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
				/// <summary>
				/// Redirige al sitio de notificaciones cuando no se apunta a una accción ni controlador
				/// </summary>
				/// <returns></returns>
        public ActionResult Index()
        {
					Utils.RegistrarInfo(string.Format("redirigiendo a {0}", ConfigurationManager.AppSettings["UrlNotificaciones"]));
					return new RedirectResult(ConfigurationManager.AppSettings["UrlNotificaciones"], false);
				}
    }
}
