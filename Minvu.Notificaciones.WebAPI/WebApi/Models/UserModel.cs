using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class UserModel
    {
        public Minvu.Security.MinvuPrincipal MP { get; set; }
        public string ROL { get; set; }
        public IList<string> Regiones { get; set; }
    }
}