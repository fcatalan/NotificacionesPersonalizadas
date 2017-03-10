using Minvu.Notificaciones.IData.ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minvu.Notificaciones.IData.DAO
{
    public class UsuarioDAO
    {

        public static List<USUARIO> ObtenerUsurios()
        {
            List<USUARIO> usuarios = new List<USUARIO>();

            using (ContextoBD contexto = new ContextoBD())
            {
                usuarios = contexto.USUARIO.ToList();
            }
            return usuarios;
        }

        public static List<string> ObtenerUsuariosPorNombreUsuario(string parteNombre)
        {
            using (ContextoBD contexto = new ContextoBD())
            {
                return contexto.ENVIO.Select(e => e.IDUSUARIO).Where(e => e.Contains(parteNombre)).Distinct().ToList();
            }
        }
        public static USUARIO ObtenerUsuarioBD(string idusuario)
        {
            USUARIO usuario = new USUARIO();
            try
            {
                using (ContextoBD contexto = new ContextoBD())
                {
                    usuario = contexto.USUARIO.Where(usu => usu.IDUSUARIO == idusuario).First();
                    if (usuario != null)
                    {
                        usuario.FECHAULTIMOINGRESO = DateTime.Now;
                    }
                    contexto.SaveChanges();
                }
            }
            catch (Exception)
            {
                usuario = null;
            }

            return usuario;
        }
        public static bool GuardarUsuarioBD(string idUsuario)
        {
            bool result = false;
            DateTime fechaActual = DateTime.Now;
            USUARIO usuario = new USUARIO();

            using (ContextoBD contexto = new ContextoBD())
            {
                usuario.IDUSUARIO = idUsuario;
                usuario.FECHAULTIMOINGRESO = fechaActual;
                contexto.USUARIO.Add(usuario);

                contexto.SaveChanges();
                result = true;
            }

            return result;
        }
    }
}
