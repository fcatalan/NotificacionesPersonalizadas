//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Minvu.Notificaciones.IData.ORM
{
    using System;
    using System.Collections.Generic;
    
    public partial class EVENTO
    {
        public long IDEVENTO { get; set; }
        public string IDUSUARIO { get; set; }
        public string DESCRIPCION { get; set; }
        public System.DateTime FECHA { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
    }
}