using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.ORM;

namespace Minvu.Notificaciones.IData.DAO
{
    public class PalabraProhibidaDAO
    {

        public static bool CrearPalabraProhibida(PalabraProhibidaDTO palabra)
        {
            try
            {
               
                using (NotificacionesEntities context = new NotificacionesEntities())
                {
                    USUARIO usuario = context.USUARIO.Find(palabra.IdUsuario);

                    PALABRAS_PROHIBIDAS palabrasProhibidas = new PALABRAS_PROHIBIDAS();
                    palabrasProhibidas.IDUSUARIO = palabra.IdUsuario;
                    palabrasProhibidas.PALABRA = palabra.Palabra.ToLower();
                    palabrasProhibidas.FECHAINGRESO = DateTime.Now;//palabra.FechaIngreso;
                    palabrasProhibidas.IDUSUARIOMODIFICACION = palabra.IdUsuarioModi;
                    palabrasProhibidas.FECHAMODIFICACION = null;
                    palabrasProhibidas.ESTADOVIGENCIA = true; // palabra.EstadoVigencia;

                    palabrasProhibidas.USUARIO = usuario;

                    context.PALABRAS_PROHIBIDAS.Add(palabrasProhibidas);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public static bool EditarPalabraProhibida(PALABRAS_PROHIBIDAS entidad)
        {
            try
            {
                using (NotificacionesEntities context = new NotificacionesEntities())
                {

                    PALABRAS_PROHIBIDAS palabrasProhibidas = context.PALABRAS_PROHIBIDAS.Find(entidad.IDPALABRAPROHIBIDA);
                    palabrasProhibidas.IDUSUARIO = entidad.IDUSUARIO;
                    palabrasProhibidas.PALABRA = entidad.PALABRA.ToLower();
                    palabrasProhibidas.FECHAINGRESO = entidad.FECHAINGRESO;
                    palabrasProhibidas.IDUSUARIOMODIFICACION = entidad.IDUSUARIOMODIFICACION;
                    palabrasProhibidas.FECHAMODIFICACION = entidad.FECHAMODIFICACION;
                    palabrasProhibidas.ESTADOVIGENCIA = entidad.ESTADOVIGENCIA;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public static bool ElimiarPalabraProhibida(int idPalabra)
        {
            try
            {
                using (NotificacionesEntities context = new NotificacionesEntities())
                {
                    PALABRAS_PROHIBIDAS palabrasProhibidas = context.PALABRAS_PROHIBIDAS.Find(idPalabra);
                    context.PALABRAS_PROHIBIDAS.Remove(palabrasProhibidas);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public static PalabraProhibidaDTO ObtenerPalabraProhibida(string _palabra)
        {
            try
            {
                using (NotificacionesEntities context = new NotificacionesEntities())
                {
                    PALABRAS_PROHIBIDAS palabrasProhibidas = new PALABRAS_PROHIBIDAS();
                    PalabraProhibidaDTO dto = new PalabraProhibidaDTO();

                    try
                    {
                        palabrasProhibidas = context.PALABRAS_PROHIBIDAS.Where(palabra => palabra.PALABRA.ToUpper() == _palabra.ToUpper()).First();

                        dto.IdPalabra = palabrasProhibidas.IDPALABRAPROHIBIDA;
                        dto.IdUsuario = palabrasProhibidas.IDUSUARIO;
                        dto.Palabra = palabrasProhibidas.PALABRA;
                        dto.FechaIngreso = (DateTime)palabrasProhibidas.FECHAINGRESO;
                        dto.IdUsuarioModi = palabrasProhibidas.IDUSUARIOMODIFICACION;
                        //dto.FechaModi = (DateTime)palabrasProhibidas.FECHAMODIFICACION;
                        dto.EstadoVigencia = (bool)palabrasProhibidas.ESTADOVIGENCIA;
                        dto.AuxTotalRegistrosBusqueda = 1;
                    }
                    catch (Exception)
                    {
                        dto = null;
                    }
                  
                    return dto;
                }
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public static List<PalabraProhibidaDTO> ObtenerListaPalabras()
        {

            try
            {
                using (NotificacionesEntities context = new NotificacionesEntities())
                {
                    List<PalabraProhibidaDTO> ListPalabrasProhibidasDTO = new List<PalabraProhibidaDTO>();
                    var list = context.PALABRAS_PROHIBIDAS.ToList();

                    ListPalabrasProhibidasDTO = list.Select(l => new PalabraProhibidaDTO
                    {
                        IdPalabra = l.IDPALABRAPROHIBIDA,
                        IdUsuario = l.IDUSUARIO,
                        Palabra = l.PALABRA,
                        FechaIngreso = (DateTime)l.FECHAINGRESO,
                        IdUsuarioModi = l.IDUSUARIOMODIFICACION,
                        FechaModi = (DateTime)l.FECHAINGRESO,
                        EstadoVigencia = (bool)l.ESTADOVIGENCIA,
                        AuxTotalRegistrosBusqueda = list.Count

                    }).ToList();

                    return ListPalabrasProhibidasDTO;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
                throw;
            }
        }

        public static bool ModificarPalabraProhibida(PalabraProhibidaDTO palabra)
        {
            try
            {
                using (NotificacionesEntities context = new NotificacionesEntities())
                {
                    PALABRAS_PROHIBIDAS palabrasProhibidas = context.PALABRAS_PROHIBIDAS.Find(palabra.IdPalabra);
                    palabrasProhibidas.PALABRA = palabra.Palabra.ToLower();
                    palabrasProhibidas.IDUSUARIOMODIFICACION = palabra.IdUsuarioModi;
                    palabrasProhibidas.FECHAMODIFICACION = palabra.FechaModi;
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
