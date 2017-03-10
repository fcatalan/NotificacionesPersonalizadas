using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.Personalizadas.Entidades;
using System.Configuration;
using System.Globalization;
using Minvu.Security.Entities;
using Minvu.Security;

namespace Minvu.Notificaciones.Domain.BLL
{
    public class PalabraProhibidaBL
    {
        public bool CrearPalabrasProhibidas(PalabraProhibidaDTO ent)
        {
            bool resultCrear = false;
            try
            {
                Ticket tic = SingleSignOn.Authenticate();
                DateTime fecha = DateTime.Now;
                ent.IdUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;
                ent.FechaIngreso = fecha;
                ent.IdUsuarioModi = "";
                //Si la palabra no se encuentra registrada.
                if (PalabraProhibidaDAO.ObtenerPalabraProhibida(ent.Palabra) == null)
                {
                    resultCrear = PalabraProhibidaDAO.CrearPalabraProhibida(ent);
                }
                else //Si la palabra ya estaba creada.
                    resultCrear = false;

                return resultCrear;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }
        public PalabraProhibidaDTO ObtenerPalabraProhibida(string palabra)
        {
            try
            {

                return PalabraProhibidaDAO.ObtenerPalabraProhibida(palabra);
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        public List<PalabraProhibidaDTO> ListarPalabrasProhibidas(FiltroPalabraProhibida filtro)
        {
            try
            {
                List<PalabraProhibidaDTO> lstPaginada = new List<PalabraProhibidaDTO>();
                List<PalabraProhibidaDTO> lst = new List<PalabraProhibidaDTO>();
                int cantidadPaginacion = filtro.CantidadRegPorPaginacion;
                int numeroPagina = filtro.NumeroPagina;
                if (filtro.CantidadRegPorPaginacion > 0) {
                    int regSkip = numeroPagina - 1;
                    lst = PalabraProhibidaDAO.ObtenerListaPalabras();
                    lstPaginada = lst.Skip(regSkip * cantidadPaginacion).Take(cantidadPaginacion).ToList();
                }
                else
                {
                    lst = PalabraProhibidaDAO.ObtenerListaPalabras();
                    lstPaginada = lst;
                }
                return lstPaginada;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        public bool EliminarPalabra(int idPalabra)
        {
            PalabraProhibidaDAO.ElimiarPalabraProhibida(idPalabra);
            return true;
        }
        public bool ModificarPalabra(PalabraProhibidaDTO palabraProhibida)
        {
            //PalabraProhibidaDTO palabraProhibida = new PalabraProhibidaDTO();
            Ticket tic = SingleSignOn.Authenticate();

            palabraProhibida.FechaModi = DateTime.Now;
            palabraProhibida.IdUsuario = tic.MinvuPrincipal.MinvuIdentity.UserName;

            PalabraProhibidaDAO.ModificarPalabraProhibida(palabraProhibida);
            return true;

        }

    }
}
