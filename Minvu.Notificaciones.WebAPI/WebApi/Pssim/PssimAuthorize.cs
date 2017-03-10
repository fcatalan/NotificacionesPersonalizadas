using Minvu.Security;
using Minvu.Security.Configuration;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApi.Pssim
{
    public class PssimAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
        public String Tarea { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult(SingleSignOnConfiguration.Instance.EntryPoint);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!String.IsNullOrWhiteSpace(Tarea))
            {
                if (SingleSignOn.CurrentPrincipal != null && !SingleSignOn.CurrentPrincipal.HasTarea(Tarea))
                {
                    return false;
                }
            }

            return base.AuthorizeCore(httpContext);
        }

        #region IAuthorizationFilter Members

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            Ticket ticket = SingleSignOn.Authenticate();

            if (ticket == null)
            {
                if (SingleSignOn.HasSessionCookie())
                {
                    // filterContext.Result = new RedirectResult("/SesionExpirada");
                }
                else
                {
                    // filterContext.Result = new RedirectResult(System.Configuration.ConfigurationManager.AppSettings["PortalAutenticacion"].ToString());
                }
            }

        }

        #endregion


    }
}