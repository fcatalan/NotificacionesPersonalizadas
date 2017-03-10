using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Minvu.Security;
using Minvu.Security.Entities;

namespace WebApi.Pssim
{
    public class PssimAuthenticate : FilterAttribute, IAuthorizationFilter
    {


       
            public PssimAuthenticate()
            {
                //Log.Info("PssimAuthenticate.");
            }

            public void OnAuthorization(AuthorizationContext filterContext)
            {
                SingleSignOn.Authenticate();
            }
        


    }
}