using Microsoft.AspNetCore.Cors;
using Minvu.Notificaciones.Domain.Util;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
	public class TareaController : ApiController
	{
		[HttpGet]
		[EnableCors("AllowSpecificOrigin")]
		public bool TienePermisoTarea(string nombreTarea)
		{
			bool result = false;
			try
			{
				Ticket tic = SingleSignOn.Authenticate();
				if (SingleSignOn.CurrentPrincipal != null && SingleSignOn.CurrentPrincipal.HasTarea(nombreTarea))
				{
					result = true;
				}
			} catch (Exception ex)
			{
				Utils.RegistrarError(ex, ex.Message);
			}
			return result;
		}
	}
}
