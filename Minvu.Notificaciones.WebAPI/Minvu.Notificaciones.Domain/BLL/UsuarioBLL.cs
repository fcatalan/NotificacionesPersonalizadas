using Minvu.Notificaciones.DTO;
using Minvu.Notificaciones.IData.DAO;
using Minvu.Notificaciones.IData.ORM;
using Minvu.Notificaciones.Personalizadas.Entidades;
using Minvu.Security;
using Minvu.Security.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.Domain.BLL
{
    public class UsuarioBLL
    {

        public static Boolean CrearUsuario()
        {

            return false;
        }
        public static List<UsuarioDTO> ObtenerUsuarios()
        {
            string nombreUsuario = string.Empty;
            List<USUARIO> lst = new List<USUARIO>();
            List<UsuarioDTO> lstDto = new List<UsuarioDTO>();
            try
            {
                lst = UsuarioDAO.ObtenerUsurios();
                foreach (USUARIO usuario in lst)
                {
                    UsuarioDTO usuarioDTO = new UsuarioDTO();

                    usuarioDTO.NombreUsuario = usuario.IDUSUARIO;

                    lstDto.Add(usuarioDTO);
                }

            }
            catch (Exception ex)
            {
                IData.Log.Log.RegistrarError(ex, ex.Message, nombreUsuario);

            }
            return lstDto;
        }

        public static List<ResultadoAutocompletarDTO> BuscarUsuariosPorNombreUsuario(string parteNombre)
        {
            List<ResultadoAutocompletarDTO> usuariosDTO = new List<ResultadoAutocompletarDTO>();
            int cantMaxSugerencias = Convert.ToInt32(ConfigurationManager.AppSettings["CantMaxResultadosBusquedaUsuario"]);
            if (parteNombre != null
                && parteNombre.Length >= Convert.ToInt32(ConfigurationManager.AppSettings["LargoMinimoBusquedaUsuario"]))
            {
                List<string> usuarios = UsuarioDAO.ObtenerUsuariosPorNombreUsuario(parteNombre).Take(cantMaxSugerencias).ToList();
                foreach (string usuario in usuarios)
                {
                    ResultadoAutocompletarDTO resultadoAutocompleta = new ResultadoAutocompletarDTO();
                    resultadoAutocompleta.name = usuario;
                    MinvuUserInfoProvider userInfoProvider = new MinvuUserInfoProvider();

                    UserInfo userInfo = userInfoProvider.GetUserInfo(usuario, ConfigurationManager.AppSettings["Dominio"]);
                    resultadoAutocompleta.description = string.Format("{0} {1} {2}", userInfo.Name, userInfo.FatherName, userInfo.MotherName);
                    usuariosDTO.Add(resultadoAutocompleta);
                }
            }
            return usuariosDTO;
        }
        public static UsuarioDTO ObtenerUsuarioBD(string idUsuario)
        {
            USUARIO usuario = new USUARIO();
            UsuarioDTO objDto = new UsuarioDTO();
            usuario = UsuarioDAO.ObtenerUsuarioBD(idUsuario);

            if (usuario != null)
                objDto.NombreUsuario = idUsuario;
            else
            {
                GuardarUsuario(idUsuario);
                objDto.NombreUsuario = null;
            }

            return objDto;
        }
        public static bool GuardarUsuario(string idUsuario)
        {
            bool result = false;

            result = UsuarioDAO.GuardarUsuarioBD(idUsuario);
            if (result == false)
            {
                IData.Log.Log.RegistrarWarning("No se logró registrar en BD", idUsuario);
            }

            return result;
        }
    }
}
