using Excel.Log;
using Minvu.Notificaciones.Domain.BLL;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.IData.Log;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Notificaciones.Personalizadas.Entidades.RespuestasJSON;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Minvu.Notificaciones.Domain.BLL
{
    public class NotificacionBLL
    {
        #region "Properties" 
        public int IdNotificacion { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public Boolean EstadoVigencia { get; set; }
        public DateTime FechaCreacion { get; set; }
        #endregion

        #region "Methods" 
        public static NotificacionDTO Obtener()
        {
            NotificacionDTO dto = new NotificacionDTO();

            return dto;
        }
        /// <summary>
        /// Este método permite ontener una notificación por id.
        /// <param name="idNotificacion"></param>Corresponde a identificador único de una notificación
        /// <returns>NotificacionDTO</returns> Se retorna un objeto Data Transfer Objet de tipo notificación.
        /// </summary>
        public static NotificacionDTO ObtenerNotificacionPorId(int idNotificacion)
        {
            string nombreUsuario = string.Empty;
            NOTIFICACION notificacion = new NOTIFICACION();
            NotificacionDTO notificacionDTO = new NotificacionDTO();
            try
            {
                notificacion = NotificacionDAO.ObtenerNotificacionesPorId(idNotificacion);

                notificacionDTO.IdNotificacion = notificacion.IDNOTIFICACION;
                notificacionDTO.IdUsuario = notificacion.IDUSUARIO;
                notificacionDTO.Nombre = notificacion.NOMBRE;
                //notificacionDTO.Asunto = notificacion.ASUNTO;
                notificacionDTO.Mensaje = notificacion.MENSAJE;
                notificacionDTO.FechaHoraInicio = (DateTime)notificacion.FECHAHORAINICIO;
                notificacionDTO.FechaHoraFin = (DateTime)notificacion.FECHAHORAFIN;
                notificacionDTO.EstadoVigencia = (bool)notificacion.ESTADOVIGENCIA;
                notificacionDTO.FechaCreacion = (DateTime)notificacion.FECHACREACION;
                notificacionDTO.HoraInicio = notificacionDTO.FechaHoraInicio.TimeOfDay.ToString();
                notificacionDTO.HoraFin = notificacionDTO.FechaHoraFin.TimeOfDay.ToString();
                notificacionDTO.StrFechaInicio = notificacionDTO.FechaHoraInicio.ToShortDateString();
                notificacionDTO.StrFechaFin = notificacionDTO.FechaHoraFin.ToShortDateString();
                notificacionDTO.HoraInicio = notificacionDTO.FechaHoraInicio.Hour.ToString() + ":" + notificacionDTO.FechaHoraInicio.Minute.ToString();
                notificacionDTO.HoraFin = notificacionDTO.FechaHoraFin.Hour.ToString() + ":" + notificacionDTO.FechaHoraFin.Minute.ToString();
            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);

            }
            return notificacionDTO;

        }
        /// <summary>
        /// Este método permite guardar una notificación en la fuente de datos.
        /// <param name="notificacionDTO">Recibe un objeto Data Transfer Objet de tipo notificación.</param>
        /// <param name="sistemasEmisoresAsoc">Recibe lista de id de sistena emisores asociados a la notidicación.</param>
        /// <returns>RespuestaGenerica</returns>Se retorna un objeto que contiene 2 propiedades CodError y MsjError
        /// si CodError=0 indica que la operación guardar se ejecutó correctamente, de lo contrario revisar MsjError para ver motivo de error.
        /// </summary>
        public static RespuestaGenerica Guardar(NotificacionDTO notificacionDTO, List<int> sistemasEmisoresAsoc)
        {
            RespuestaGenerica respGenerica = new RespuestaGenerica();
            DateTime _fechaFin;
            DateTime _fechaInicio = DateTime.ParseExact(notificacionDTO.StrFechaInicio, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            if (notificacionDTO.StrFechaFin != null)
            {
                _fechaFin = DateTime.ParseExact(notificacionDTO.StrFechaFin, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            string nombreUsuario = string.Empty;

            //Se obtienen los sistemas emisores que se incorporaran a la notificación.
            try
            {
                string[] arrs = notificacionDTO.StrAux.Split('_');
                foreach (string idSistEmi in arrs)
                {
                    if (idSistEmi.IndexOf("Notif") == -1 && idSistEmi != "0")
                    {
                        int idSistema = Convert.ToInt32(idSistEmi);
                        sistemasEmisoresAsoc.Add(idSistema);//Por defecto esá tomando el sistema de Arriendo
                    }
                }
            }
            catch (Exception)
            {
                sistemasEmisoresAsoc.Add(1);
            }


            try
            {
                Ticket tic = SingleSignOn.Authenticate();
                nombreUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;

                NOTIFICACION notificacion = new NOTIFICACION();
                notificacion.IDNOTIFICACION = notificacionDTO.IdNotificacion;
                notificacion.IDUSUARIO = nombreUsuario;//notificacionDTO.IdUsuario.ToString();
                notificacion.NOMBRE = notificacionDTO.Nombre;
                //notificacion.ASUNTO = notificacionDTO.Asunto;
                notificacion.MENSAJE = notificacionDTO.Mensaje;
                notificacion.FECHAHORAINICIO = _fechaInicio;

                //notificacion.FECHAHORAFIN = _fechaFin;
                notificacion.ESTADOVIGENCIA = notificacionDTO.EstadoVigencia;
                notificacion.FECHACREACION = (DateTime)notificacionDTO.FechaCreacion;
                // notificacion.IDESTADONOTIFICACION = 1; // 1=avtiva  2=Histórica
                //notificacion.USUARIO  = nombreUsuario;

                NotificacionDAO.GuardarNotificacion(notificacion, sistemasEmisoresAsoc);
                respGenerica.CodError = 0;
                respGenerica.MsjError = "ok";
                return respGenerica;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Método que permite actualizar una notificación.
        /// <param name="notificacionDTO"></param>
        /// <returns>RespuestaGenerica</returns>
        /// </summary>
        public static RespuestaGenerica Actualizar(NotificacionDTO notificacionDTO)
        {
            RespuestaGenerica respGenerica = new RespuestaGenerica();
            NOTIFICACION notificacion = new NOTIFICACION();

            DateTime _fechaInicio = DateTime.ParseExact(notificacionDTO.StrFechaInicio + " " + notificacionDTO.HoraInicio + ":00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
            DateTime _fechaFin = DateTime.ParseExact(notificacionDTO.StrFechaFin + " " + notificacionDTO.HoraFin + ":00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);


            notificacion.IDNOTIFICACION = notificacionDTO.IdNotificacion;
            notificacion.IDUSUARIO = notificacionDTO.IdUsuario;
            //notificacion.NOMBRE = notificacionDTO.Nombre;
            //notificacion.ASUNTO = notificacionDTO.Asunto;
            notificacion.MENSAJE = notificacionDTO.Mensaje;
            notificacion.FECHAHORAINICIO = _fechaInicio; //notificacionDTO.FechaHoraInicio;
            notificacion.FECHAHORAFIN = _fechaFin;  //notificacionDTO.FechaHoraFin;
                                                    //notificacion.ESTADOVIGENCIA = notificacionDTO.EstadoVigencia;
                                                    // notificacion.FECHACREACION = notificacionDTO.FechaCreacion;
            NotificacionDAO.ActualizarNotificacion(notificacion);

            return respGenerica;
        }
        public static Boolean Crear()
        {
            Boolean result = false;

            return result;
        }
        public static List<NotificacionDTO> Listar()
        {
            string nombreUsuario = string.Empty;
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            List<NotificacionDTO> lstDto = new List<NotificacionDTO>();
            try
            {
                lst = NotificacionDAO.ObtenerNotificaciones();
                foreach (NOTIFICACION notificacion in lst)
                {
                    NotificacionDTO notificacionDTO = new NotificacionDTO();

                    notificacionDTO.IdNotificacion = notificacion.IDNOTIFICACION;
                    notificacionDTO.IdUsuario = notificacion.IDUSUARIO;
                    notificacionDTO.Nombre = notificacion.NOMBRE;
                    //notificacionDTO.Asunto = notificacion.ASUNTO;
                    notificacionDTO.Mensaje = notificacion.MENSAJE;
                    notificacionDTO.FechaHoraInicio = (DateTime)notificacion.FECHAHORAINICIO;
                    notificacionDTO.FechaHoraFin = (DateTime)notificacion.FECHAHORAFIN;
                    notificacionDTO.EstadoVigencia = (bool)notificacion.ESTADOVIGENCIA;
                    notificacionDTO.FechaCreacion = (DateTime)notificacion.FECHACREACION;
                    notificacionDTO.HoraInicio = notificacionDTO.FechaHoraInicio.TimeOfDay.ToString();
                    notificacionDTO.HoraFin = notificacionDTO.FechaHoraFin.TimeOfDay.ToString();
                    if (notificacionDTO.IdEstadoNotificacion == 1)
                        notificacionDTO.EstadoNotificacion = "Activa";
                    else if (notificacionDTO.IdEstadoNotificacion == 2)
                        notificacionDTO.EstadoNotificacion = "Histórica";

                    lstDto.Add(notificacionDTO);
                }

            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);

            }
            return lstDto;
        }
        public static List<SistemaEmisorDTO> listarSistemasdeNotificacion(int idNotificacion)
        {
            string nombreUsuario = string.Empty;
            List<SISTEMA_EMISOR> lst = new List<SISTEMA_EMISOR>();
            List<SistemaEmisorDTO> lstDto = new List<SistemaEmisorDTO>();
            try
            {
                lst = NotificacionDAO.ObtenerSistemasAsociadosANotificacion(idNotificacion);
                foreach (SISTEMA_EMISOR sistemaEmisor in lst)
                {
                    SistemaEmisorDTO sistemaEmisorDTO = new SistemaEmisorDTO();

                    sistemaEmisorDTO.IdEmisor = sistemaEmisor.IDSISTEMAEMISOR;
                    sistemaEmisorDTO.NombreSistema = sistemaEmisor.NOMBRESISTEMA;
                    sistemaEmisorDTO.CasillaCorreo = sistemaEmisor.CASILLACORREO;

                    lstDto.Add(sistemaEmisorDTO);
                }

            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);

            }
            return lstDto;
        }



        //BuscarPorUsuario
        public static List<NotificacionDTO> BuscarNotificacionesPorUsuario(string idUsuario)
        {
            string nombreUsuario = string.Empty;

            List<NotificacionDTO> lstDto = new List<NotificacionDTO>();
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            try
            {
                lst = NotificacionDAO.ObtenerNotificacionesPorUsuario(idUsuario);
                foreach (NOTIFICACION notificacion in lst)
                {
                    NotificacionDTO objDTO = new NotificacionDTO();

                    objDTO.IdNotificacion = notificacion.IDNOTIFICACION;
                    objDTO.Nombre = notificacion.NOMBRE;
                    objDTO.Mensaje = notificacion.MENSAJE;
                    objDTO.FechaHoraInicio = (DateTime)notificacion.FECHAHORAINICIO;
                    objDTO.FechaHoraFin = (DateTime)notificacion.FECHAHORAFIN;
                    objDTO.EstadoNotificacion = "Activa"; //se debe agregar campo estadoNotificacion
                    objDTO.IdUsuario = notificacion.IDUSUARIO;
                    objDTO.EstadoVigencia = (Boolean)notificacion.ESTADOVIGENCIA;
                    lstDto.Add(objDTO);
                }
            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);
            }
            return lstDto;
        }

        /// <summary>
        /// Función publica permite obtener listado de notificaciones por distinto tipo filtrados,
        /// además debe controlar la paginación.
        /// <param name="filtro"></param>
        /// <returns>List<NotificacionDTO></returns>
        /// </summary>
        public static List<NotificacionDTO> BuscarNotificacionPorFiltro(FiltroBusquedaNotificacion filtro)
        {
            string nombreUsuario = string.Empty;
            List<NOTIFICACION> lst = new List<NOTIFICACION>();
            List<NOTIFICACION> lstPaginada = new List<NOTIFICACION>();
            List<NotificacionDTO> lstDto = new List<NotificacionDTO>();
            int cantResults = 0;

            switch (filtro.AccionAEjecutar)
            {
                case "TraerTodosUsaFiltro":
                    lst = AplicarFiltros(filtro);
                    lstPaginada = lst;
                    break;
                case "TraerTodos":
                    lst = ObtenerListadoNotificaciones();

                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > 0)
                    {
                        lstPaginada = lst.Skip(filtro.NumeroPagina * filtro.CantidadPaginacion).Take(filtro.CantidadPaginacion).ToList();
                    }
                    else
                    {
                        lstPaginada = lst;
                    }
                    break;
                case "NuevaBusqueda":

                    lst = AplicarFiltros(filtro);
                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > 0)
                    {
                        if (lst.Count <= filtro.CantidadPaginacion)
                        {
                            lstPaginada = lst;
                        }
                        else
                        {
                            lstPaginada = lst.Skip(0).Take(filtro.CantidadPaginacion).ToList();
                        }
                    }
                    else //Cuando usuario selecciona mostrar todos
                    {
                        lstPaginada = lst;
                    }

                    break;
                case "MoverPaginacionSiguiente":
                    int paginacionASaltarAdelante = filtro.NumeroPagina - 1;

                    lst = AplicarFiltros(filtro);
                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > 0)
                    {
                        lstPaginada = lst.Skip(paginacionASaltarAdelante * filtro.CantidadPaginacion).Take(filtro.CantidadPaginacion).ToList();
                    }
                    break;
                case "MoverPaginacionAnterior":
                    int paginacionASaltarAtras = filtro.NumeroPagina - 1;

                    lst = AplicarFiltros(filtro);
                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > -1)
                    {
                        lstPaginada = lst.Skip(paginacionASaltarAtras * filtro.CantidadPaginacion).Take(filtro.CantidadPaginacion).ToList();
                    }
                    break;
                case "MoverPaginaInicio":
                    filtro.NumeroPagina = 1;

                    lst = AplicarFiltros(filtro);
                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > -1)
                    {
                        lstPaginada = lst.Skip(0).Take(filtro.CantidadPaginacion).ToList();
                    }
                    break;
                case "MoverPaginaFinal":
                    int paginacionASaltarAdelante2 = filtro.NumeroPagina - 1;

                    lst = AplicarFiltros(filtro);
                    cantResults = lst.Count;
                    if (filtro.CantidadPaginacion > 0)
                    {
                        lstPaginada = lst.Skip(paginacionASaltarAdelante2 * filtro.CantidadPaginacion).Take(filtro.CantidadPaginacion).ToList();
                    }
                    break;
            }

            try
            {
                //lst = NotificacionDAO.ObtenerNotificacionesPorUsuario(filtro.IdUsuario);
                foreach (NOTIFICACION notificacion in lstPaginada)
                {
                    NotificacionDTO objDTO = new NotificacionDTO();

                    objDTO.IdNotificacion = notificacion.IDNOTIFICACION;
                    objDTO.Nombre = notificacion.NOMBRE;
                    objDTO.Mensaje = notificacion.MENSAJE;
                    objDTO.FechaHoraInicio = (DateTime)notificacion.FECHAHORAINICIO;
                    objDTO.StrFechaInicio = objDTO.FechaHoraInicio.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
                    if (notificacion.FECHAHORAFIN != null)
                    {
                        objDTO.FechaHoraFin = (DateTime)notificacion.FECHAHORAFIN;
                        objDTO.StrFechaFin = objDTO.FechaHoraFin.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);

                    }
                    else //En caso la fecha final de la notificación sea nula.
                    {
                        objDTO.StrFechaFin = "      ---";
                    }

                    objDTO.EstadoNotificacion = notificacion.ESTADOVIGENCIA.ToString(); //se debe agregar campo estadoNotificacion
                    //objDTO.IdEstadoNotificacion = (int)notificacion.IDESTADONOTIFICACION;
                    objDTO.IdUsuario = notificacion.IDUSUARIO;
                    objDTO.EstadoVigencia = (Boolean)notificacion.ESTADOVIGENCIA;
                    objDTO.TotalRegistrosBusqueda = cantResults;
                    DateTime fechaHoraActual = DateTime.Now;
                    IFormatProvider culture = new System.Globalization.CultureInfo("es-CL", true);

                    DateTime dtFechaActual = DateTime.Parse(fechaHoraActual.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);
                    DateTime dtFechaFinNotificacion = DateTime.Parse(objDTO.FechaHoraFin.ToString(), culture, System.Globalization.DateTimeStyles.AssumeLocal);

                    //DateTime _fechaActual = DateTime.ParseExact(dt.ToString(), "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                    if (dtFechaActual > dtFechaFinNotificacion && notificacion.FECHAHORAFIN != null)
                    {
                        //if (objDTO.IdEstadoNotificacion == 1)
                        objDTO.EstadoNotificacion = "Histórica";
                        if (objDTO.IdEstadoNotificacion == 1)
                        {
                            // Actualizar(objDTO); Agregar método para actualizar el estado de la notificacion
                        }
                    }
                    else
                    {
                        // else if (objDTO.IdEstadoNotificacion == 2)
                        objDTO.EstadoNotificacion = "Activa";
                    }



                    lstDto.Add(objDTO);
                }
            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);
            }
            return lstDto;

        }
        public static int ObtenerTotRegNotificaciones()
        {
            return NotificacionDAO.ObtenerTotNotificaciones();
        }
        private static List<NOTIFICACION> ObtenerListadoNotificaciones()
        {
            return NotificacionDAO.ObtenerTodasLasNotificaciones();
        }
        /// <summary>
        /// Función privada permite obtener listado de notificaciones, que puede ser filtrado por
        /// distintos parámetros, se utiliza clase FiltroBusquedaNotificacion.
        /// <param name="filtro"></param>
        /// <returns>List<NOTIFICACION></returns>
        /// </summary>
        /// 
        public static string ValidaTextoPalabrasProhibidas(string texto)
        {
            string result = string.Empty;
            HashSet<string> palabrasProhibidasEncontradas = new HashSet<string>();

            palabrasProhibidasEncontradas = BuscarPalabraProhibida(texto);
            foreach (string palabra in palabrasProhibidasEncontradas)
            {
                result = result + " " + palabra;
            }
            return result;
        }
        private static List<NOTIFICACION> AplicarFiltros(FiltroBusquedaNotificacion filtro)
        {

            List<NOTIFICACION> lst = new List<NOTIFICACION>();

            //Busqueda 0 Completa
            if (String.IsNullOrEmpty(filtro.NombreNotificacion) &&
                    ((String.IsNullOrEmpty(filtro.FechaInicio)) && (String.IsNullOrEmpty(filtro.FechaFin)) &&
                    (String.IsNullOrEmpty(filtro.IdUsuario)) &&
                    (filtro.IdEstadoNotificacion <= 0) &&
                     filtro.IdSistemaEmisorAsoc <= 0))
            {
                return NotificacionDAO.ObtenerTodasLasNotificaciones();
            }
            //Busqueda 1 Completa
            if (!String.IsNullOrEmpty(filtro.NombreNotificacion) &&
                    ((!String.IsNullOrEmpty(filtro.FechaInicio)) && (!String.IsNullOrEmpty(filtro.FechaFin)) &&
                    (!String.IsNullOrEmpty(filtro.IdUsuario) && filtro.IdUsuario != "-1") &&
                    (filtro.IdEstadoNotificacion > 0)))
            //(filtro.IdSistemaEmisorAsoc > 0)))
            {

                return NotificacionDAO.HacerBusquedaUno(filtro);
            }

            //Busqueda 2 --  sin busqueda por sistema asociado.
            if (!String.IsNullOrEmpty(filtro.NombreNotificacion) &&
                    ((!String.IsNullOrEmpty(filtro.FechaInicio)) && (!String.IsNullOrEmpty(filtro.FechaFin)) &&
                    (filtro.IdEstadoNotificacion > 0) &&
                    (!String.IsNullOrEmpty(filtro.IdUsuario) && filtro.IdUsuario != "-1")))
            {
                return NotificacionDAO.HacerBusquedaDos(filtro);

            }
            //busqueda 3 --sin busqueda de sistema emisor y usuario.
            if (!String.IsNullOrEmpty(filtro.NombreNotificacion) &&
                    ((!String.IsNullOrEmpty(filtro.FechaInicio)) && (!String.IsNullOrEmpty(filtro.FechaFin)) &&
                    (filtro.IdEstadoNotificacion > 0)))
            {
                return NotificacionDAO.HacerBusquedaTres(filtro);

            }
            //busqueda 4 --sin busqueda de sistema emisor,  usuario y estado notificacion
            if (!String.IsNullOrEmpty(filtro.NombreNotificacion) &&
                    ((!String.IsNullOrEmpty(filtro.FechaInicio)) && (!String.IsNullOrEmpty(filtro.FechaFin))))
            {
                return NotificacionDAO.HacerBusquedaCuatro(filtro);

            }
            //busqueda 5 -- sólo 1 filtro / busqueda por nombre de notificacion
            if (!String.IsNullOrEmpty(filtro.NombreNotificacion))
            {
                return NotificacionDAO.ObtenerNotificacionesPorNombre(filtro.NombreNotificacion);
            }
            //busqueda 6 -- sólo 1 filtro / busqueda por fecha
            if (!String.IsNullOrEmpty(filtro.FechaInicio) && (!String.IsNullOrEmpty(filtro.FechaFin)))
            {
                DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

                return NotificacionDAO.ObtenerNotificacionesPorFecha(_fechaInicio, _fechaFin);
            }
            //busqueda 7 -- sólo 1 filtro / busqueda por idUsuario
            if (!String.IsNullOrEmpty(filtro.IdUsuario))
            {
                return NotificacionDAO.ObtenerNotificacionesPorUsuario(filtro.IdUsuario);
            }
            //busqueda 8 -- sólo 1 filtro / busqueda por id estado de notificación
            if (filtro.IdEstadoNotificacion > 0)
            {
                return NotificacionDAO.ObtenerNotificacionesPorEstado(filtro.IdEstadoNotificacion);
            }
            //busqueda 9 -- sólo 1 filtro / busqueda por id de sistema emisor
            if (filtro.IdSistemaEmisorAsoc > 0)
            {
                return NotificacionDAO.ObtenerNotificacionesPorSistema(filtro.IdSistemaEmisorAsoc);
            }




            if (!String.IsNullOrEmpty(filtro.NombreNotificacion))
            {
                lst = NotificacionDAO.ObtenerNotificacionesPorNombre(filtro.NombreNotificacion);
            }
            if (!String.IsNullOrEmpty(filtro.IdUsuario) && filtro.IdUsuario != "-1")
            {
                lst = NotificacionDAO.ObtenerNotificacionesPorUsuario(filtro.IdUsuario);
            }

            if ((!String.IsNullOrEmpty(filtro.FechaInicio)) && (!String.IsNullOrEmpty(filtro.FechaFin)))
            {
                // 2009 - 05 - 08
                try
                {
                    DateTime _fechaInicio = DateTime.ParseExact(filtro.FechaInicio + " 00:00:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime _fechaFin = DateTime.ParseExact(filtro.FechaFin + " 23:59:00,000", "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

                    lst = NotificacionDAO.ObtenerNotificacionesPorFecha(_fechaInicio, _fechaFin);
                }
                catch (Exception)
                {
                }
            }
            if (filtro.IdEstadoNotificacion > 0)
            {
                lst = NotificacionDAO.ObtenerNotificacionesPorEstado(filtro.IdEstadoNotificacion);
            }
            return lst;
        }

        private static HashSet<string> BuscarPalabraProhibida(string texto)
        {
            List<PalabraProhibidaDTO> palabrasProhibidas = PalabraProhibidaDAO.ObtenerListaPalabras();
            HashSet<string> palabrasProhibidasEncontradas = new HashSet<string>();

            string textoUpperCase = texto.ToUpper();

            //validacion de palabras prohibidas en asunto
            foreach (PalabraProhibidaDTO palabraProhibida in palabrasProhibidas)
            {
                if (textoUpperCase.Contains(palabraProhibida.Palabra.ToUpper()))
                {
                    palabrasProhibidasEncontradas.Add(palabraProhibida.Palabra);
                }
            }
            return palabrasProhibidasEncontradas;
        }

        #endregion
    }
}
