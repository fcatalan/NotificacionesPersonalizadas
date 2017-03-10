using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
    public class NotificacionDAO
    {
        public static List<NOTIFICACION> ObtenerNotificaciones()
        {
            List<NOTIFICACION> notificaciones = new List<NOTIFICACION>();
            using (ContextoBD contexto = new ContextoBD())
            {
                notificaciones = contexto.NOTIFICACION.ToList();
            }
            return notificaciones;
        }
        public static Boolean GuardarNotificacion(NOTIFICACION notificacion, List<int> SistemasEmisores)
        {
            Boolean result = false;
            using (ContextoBD contexto = new ContextoBD())
            {
                if (notificacion.IDNOTIFICACION == 0)
                {
                    //Se debe guardar en la tabla NOTIFICACION_SISTEMA_EMISOR
                    // id 1 = Sistema de Arriendo y notificacion.IDNOTIFICACION 
                    foreach (int id in SistemasEmisores)
                    {
                        notificacion.SISTEMA_EMISOR.Add(contexto.SISTEMA_EMISOR.Find(id));
                    }
                    contexto.NOTIFICACION.Add(notificacion);
                }
                else
                {
                    NOTIFICACION notificacionExistente = contexto.NOTIFICACION.Find(notificacion.IDNOTIFICACION);
                    // notificacionExistente.IDNOTIFICACION = notificacion.IDNOTIFICACION;
                    notificacionExistente.IDUSUARIO = notificacion.IDUSUARIO;
                    notificacionExistente.NOMBRE = notificacion.NOMBRE;
                    //notificacionExistente.ASUNTO = notificacion.ASUNTO;
                    notificacionExistente.MENSAJE = notificacion.MENSAJE;
                    notificacionExistente.FECHAHORAINICIO = notificacion.FECHAHORAINICIO;
                    notificacionExistente.FECHAHORAFIN = notificacion.FECHAHORAFIN;
                    notificacionExistente.ESTADOVIGENCIA = notificacion.ESTADOVIGENCIA;
                    notificacionExistente.FECHACREACION = notificacion.FECHACREACION;
                }
                contexto.SaveChanges();
                result = true;
            }
            return result;
        }
        public static void ActualizarNotificacion(NOTIFICACION notificacion)
        {
            using (ContextoBD contexto = new ContextoBD())
            {
                NOTIFICACION notificacionExistente = contexto.NOTIFICACION.Find(notificacion.IDNOTIFICACION);

                notificacionExistente.IDUSUARIO = notificacion.IDUSUARIO;
                //notificacionExistente.NOMBRE = notificacion.NOMBRE;
                //notificacionExistente.ASUNTO = notificacion.ASUNTO;
                notificacionExistente.MENSAJE = notificacion.MENSAJE;
                notificacionExistente.FECHAHORAINICIO = notificacion.FECHAHORAINICIO;
                notificacionExistente.FECHAHORAFIN = notificacion.FECHAHORAFIN;
                //notificacionExistente.ESTADOVIGENCIA = notificacion.ESTADOVIGENCIA;
                //notificacionExistente.FECHACREACION = notificacion.FECHACREACION;

                contexto.SaveChanges();
            }
        }
        public static List<SISTEMA_EMISOR> ObtenerSistemasAsociadosANotificacion(int idNotificacion)
        {
            NOTIFICACION notificacion = new NOTIFICACION();

            ICollection<SISTEMA_EMISOR> c;

            List<SISTEMA_EMISOR> lst = new List<SISTEMA_EMISOR>();
            using (ContextoBD contexto = new ContextoBD())
            {
                //envios = contexto.ENVIO.Where(e => e.IDESTADOENVIO == estadoListoParaEnviar).ToList();
                notificacion = contexto.NOTIFICACION.Where(notif => notif.IDNOTIFICACION == idNotificacion).First();

                foreach (SISTEMA_EMISOR item in notificacion.SISTEMA_EMISOR)
                {
                    lst.Add(item);
                }
            }
            return lst;
            //lst = a.SISTEMA_EMISOR.Cast<SISTEMA_EMISOR>().ToList();
            //lst = notificacion.SISTEMA_EMISOR.Cast<SISTEMA_EMISOR>().ToList();
        }
        public static List<NOTIFICACION> ObtenerNotificacionesPorUsuario(string idUsuario)
        {
            NOTIFICACION notificacion = new NOTIFICACION();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.Where(notif => notif.IDUSUARIO == idUsuario).ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> ObtenerNotificacionesPorNombre(string nombreNotificacion)
        {
            NOTIFICACION notificacion = new NOTIFICACION();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            using (ContextoBD contexto = new ContextoBD())
            {
                //se debe usar like para obtener notificaciones por nombre
                lst = contexto.NOTIFICACION.Where(notif => notif.NOMBRE.ToUpper().Contains(nombreNotificacion.ToUpper())).ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> ObtenerNotificacionesPorEstado(int IdEstadoNotificacion)
        {
            List<int> sistemasLst = new List<int>();
            sistemasLst.Add(IdEstadoNotificacion);

            NOTIFICACION notificacion = new NOTIFICACION();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            DateTime FechaActual = DateTime.Now;

            using (ContextoBD contexto = new ContextoBD())
            {
                //se debe agregar el campo estado de la notificacion
                lst = contexto.NOTIFICACION.Where(notif => notif.ESTADOVIGENCIA == true 
                                               && notif.SISTEMA_EMISOR.Any(SE => sistemasLst.Contains(SE.IDSISTEMAEMISOR))
                                               && notif.FECHAHORAFIN ==null || notif.FECHAHORAFIN >= FechaActual).ToList();


             /*   query = from N in contexto.NOTIFICACION
                        where N.ESTADOVIGENCIA == true
                        && N.SISTEMA_EMISOR.Any(c => sistemasLst.Contains(c.IDSISTEMAEMISOR))
                        && (N.FECHAHORAFIN == null || N.FECHAHORAFIN >= FechaActual) */
            }
            return lst;
        }

        public static List<NOTIFICACION> ObtenerNotificacionesPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            NOTIFICACION notificacion = new NOTIFICACION();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            using (ContextoBD contexto = new ContextoBD())
            {
                //se debe agregar el campo estado de la notificacion
                lst = contexto.NOTIFICACION.Where(notif => notif.FECHAHORAINICIO >= fechaInicio && notif.FECHAHORAFIN <= fechaFin).ToList();
                //var _dataToWebPage = lst.Skip(10).Take(10);
                //var cantResults = lst.Count;
                //if (registrosPagina > 0)
                //    return lst.Skip(numeroPagina * registrosPagina).Take(registrosPagina).ToList();
                //else
                //    return lst;
            }
            return lst;
        }

        public static List<NOTIFICACION> ObtenerNotificacionesPorSistema(int idSistema)
        {
            NOTIFICACION notificacion = new NOTIFICACION();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            using (ContextoBD contexto = new ContextoBD())
            {
                //se debe usar like para obtener notificaciones por nombre
                lst = contexto.SISTEMA_EMISOR.Find(idSistema).NOTIFICACION.ToList();

                //var cantResults = lst.Count;
                //if (registrosPagina > 0)
                //    return lst.Skip(numeroPagina * registrosPagina).Take(registrosPagina).ToList();
                //else
                //    return lst;
            }
            return lst;
        }
        public static int ObtenerTotNotificaciones()
        {
            int lstTot = 0;

            using (ContextoBD contexto = new ContextoBD())
            {
                lstTot = contexto.NOTIFICACION.Count();
            }
            return lstTot;
        }
        public static List<NOTIFICACION> ObtenerTodasLasNotificaciones()
        {
            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> HacerBusquedaUno(FiltroBusquedaNotificacion filtro)
        {
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            //Sin busqueda por sistema emisor
            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.Where(notif => notif.FECHAHORAINICIO >= _fechaInicio && notif.FECHAHORAFIN <= _fechaFin
                                                           //&& notif.IDESTADONOTIFICACION == filtro.IdEstadoNotificacion
                                                           && notif.IDUSUARIO == filtro.IdUsuario
                                                           && notif.NOMBRE.ToUpper().Contains(filtro.NombreNotificacion.ToUpper())).ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> HacerBusquedaDos(FiltroBusquedaNotificacion filtro)
        {
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            //Sin busqueda por sistema emisor
            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.Where(notif => notif.FECHAHORAINICIO >= _fechaInicio && notif.FECHAHORAFIN <= _fechaFin
                                                          // && notif.IDESTADONOTIFICACION == filtro.IdEstadoNotificacion
                                                           && notif.IDUSUARIO == filtro.IdUsuario
                                                           && notif.NOMBRE.ToUpper().Contains(filtro.NombreNotificacion.ToUpper())).ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> HacerBusquedaTres(FiltroBusquedaNotificacion filtro)
        {
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            //Sin busqueda por sistema emisor
            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.Where(notif => notif.FECHAHORAINICIO >= _fechaInicio && notif.FECHAHORAFIN <= _fechaFin
                                                           //&& notif.IDESTADONOTIFICACION == filtro.IdEstadoNotificacion
                                                           && notif.NOMBRE.ToUpper().Contains(filtro.NombreNotificacion.ToUpper())).ToList();
            }
            return lst;
        }
        public static List<NOTIFICACION> HacerBusquedaCuatro(FiltroBusquedaNotificacion filtro)
        {
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

            //busqueda 4 --sin busqueda de sistema emisor,  usuario y estado notificacion
            using (ContextoBD contexto = new ContextoBD())
            {
                lst = contexto.NOTIFICACION.Where(notif => notif.FECHAHORAINICIO >= _fechaInicio && notif.FECHAHORAFIN <= _fechaFin
                                                           && notif.NOMBRE.ToUpper().Contains(filtro.NombreNotificacion.ToUpper())).ToList();
            }
            return lst;
        }
        /// <summary>
        /// Se obtiene la notificación por id y retorna un objeto ORM con la información de la notificación
        /// <param name="idNotificacion"></param>
        /// <returns></returns>
        /// </summary>
        public static NOTIFICACION ObtenerNotificacionesPorId(int idNotificacion)
        {
            NOTIFICACION notificacion = new NOTIFICACION();


            using (ContextoBD contexto = new ContextoBD())
            {
                notificacion = contexto.NOTIFICACION.Where(notif => notif.IDNOTIFICACION >= idNotificacion).First();
            }
            return notificacion;
        }
    }
}
