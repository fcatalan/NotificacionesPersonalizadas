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
    
    public partial class NOTIFICACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NOTIFICACION()
        {
            this.SISTEMA_EMISOR = new HashSet<SISTEMA_EMISOR>();
        }
    
        public int IDNOTIFICACION { get; set; }
        public string IDUSUARIO { get; set; }
        public string NOMBRE { get; set; }
        public string MENSAJE { get; set; }
        public Nullable<System.DateTime> FECHAHORAINICIO { get; set; }
        public Nullable<System.DateTime> FECHAHORAFIN { get; set; }
        public Nullable<bool> ESTADOVIGENCIA { get; set; }
        public Nullable<System.DateTime> FECHACREACION { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SISTEMA_EMISOR> SISTEMA_EMISOR { get; set; }
    }
}
