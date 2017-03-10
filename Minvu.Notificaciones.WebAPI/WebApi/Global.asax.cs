using FluentScheduler;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApi
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			JobManager.Initialize(new ProcesoEnvioCorreoBL());
			log4net.Config.XmlConfigurator.Configure();
			string rutaArchivoControlProceso = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoEnvios.txt";
			string rutaArchivoControlRebotes = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoRebotes.txt";
			Utils.RegistrarInfo("Iniciada la aplicacion en IIS. rutaArchivoControlProceso: " + rutaArchivoControlProceso);
			Utils.RegistrarInfo("\nExiste?:" + File.Exists(rutaArchivoControlProceso));
			Utils.RegistrarInfo("\nrutaArchivoControlRebotes:" + rutaArchivoControlRebotes);
			Utils.RegistrarInfo("\nExiste?:" + File.Exists(rutaArchivoControlRebotes));
			if (File.Exists(rutaArchivoControlProceso))
			{
				try
				{
					File.Delete(rutaArchivoControlProceso);
				}
				catch (Exception)
				{
				}
			}
			if (File.Exists(rutaArchivoControlRebotes))
			{ 
				try
				{
					File.Delete(rutaArchivoControlRebotes);
				}
				catch (Exception)
				{
				}
			}
		}

		protected void Application_Disposed(object sender, EventArgs e)
		{
			//Se invoca varias veces, no una unica vez al terminar IIS por lo que no sirve para hacer limpieza
		}
		protected void Application_End(object sender, EventArgs e)
		{
			string rutaArchivoControlProceso = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoEnvios.txt";
			string rutaArchivoControlRebotes = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "corriendoProcesoRebotes.txt";
			Utils.RegistrarInfo("Terminada la aplicacion en IIS. rutaArchivoControlProceso: " + rutaArchivoControlProceso);
			Utils.RegistrarInfo("\nExiste?:" + File.Exists(rutaArchivoControlProceso));
			Utils.RegistrarInfo("\nrutaArchivoControlRebotes:" + rutaArchivoControlRebotes);
			Utils.RegistrarInfo("\nExiste?:" + File.Exists(rutaArchivoControlRebotes));
			if (File.Exists(rutaArchivoControlProceso))
			{
				try
				{
					File.Delete(rutaArchivoControlProceso);
				}
				catch (Exception)
				{
				}
			}
			if (File.Exists(rutaArchivoControlRebotes))
			{
				try
				{
					File.Delete(rutaArchivoControlRebotes);
				}
				catch (Exception)
				{
				}
			}
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

			Ticket ticket = SingleSignOn.Authenticate();

			//Utils.RegistrarInfo("Access-Control-Allow-Origin:" + ConfigurationManager.AppSettings["UrlNotificaciones"]);
			//Utils.RegistrarInfo("Directorio virtual:" + HttpContext.Current.Request.ApplicationPath);
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", ConfigurationManager.AppSettings["UrlNotificaciones"]);

			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");


			//if (HttpContext.Current.Request.Headers.AllKeys.Contains("Origin") && HttpContext.Current.Request.HttpMethod == "OPTIONS")
			//{
			//    Response.Flush();
			//}

			if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
			{
				//These headers are handling the "pre-flight" OPTIONS call sent by the browser
				HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
				HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "ACCEPT, CONTENT-TYPE, X-CSRF-TOKEN");
				//HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");
				HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers, sesion");
				HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
				HttpContext.Current.Response.End();
			}
			if (Request.Path != "/TicketReceiver.sso" && Request.Path != "/")
			{
				if (ticket == null || ticket.Session.HasExpired || !SingleSignOn.HasSessionCookie())
				{
					Response.Headers.Add(ConfigurationManager.AppSettings["NombreHeaderUrl"], Request.Url.AbsoluteUri);
					Response.Headers.Add(ConfigurationManager.AppSettings["NombreHeaderSinSesion"], ConfigurationManager.AppSettings["ValorHeaderSinSesion"]);
				}
			}
		}

			/*
			protected void Application_Error()
			{
				var exception = Server.GetLastError();
				//TODO: Log!
				Utils.RegistrarError(exception, "Excepcion registrada en Global.asax: " + exception);

				Response.Clear();
				Server.ClearError();

				//Response.RedirectToRoute("Error");
				Response.Redirect("~/");
			}
			*/


			/*
							 protected void Application_BeginRequest(object sender, EventArgs e)
					{

							if (Request.Path != "/TicketReceiver.sso" && Request.Path != "/")
							{
									Ticket ticket = SingleSignOn.Authenticate();
									List<String> Extension = new List<String>(SingleSignOnConfiguration.Instance.ExtensionsToExclude.Split(';'));

									//if (SingleSignOnConfiguration.Instance.ExcludePaths != a && a!="/Content")            
									if (!Extension.Exists(delegate(string item) { return item == Request.CurrentExecutionFilePathExtension; }))
											if (SingleSignOn.HasSessionCookie())
											{
													if (ticket == null && Request.Path != "/SessionError/SessionError.aspx" && Request.Path != "/SessionError/SessionErrorMVC.aspx")
													{
															Response.Cookies["Set-Cookie"].Value = "false";
															if (Request.CurrentExecutionFilePathExtension == ".aspx")
																	Response.Redirect("~/SessionError/SessionError.aspx", true);
															else
															{
																	Response.Redirect("~/SessionError/SessionErrorMVC.aspx", true);
															}
													}
											}
							}
					}


			 */
		}
}
