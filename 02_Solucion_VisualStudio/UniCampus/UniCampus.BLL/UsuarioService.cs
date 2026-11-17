using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class ResultadoLogin
    {
        public bool Exito { get; set; }
        public UsuarioDto Usuario { get; set; }
        public string Mensaje { get; set; }
    }

    public class UsuarioService
    {
        private readonly IUsuarioRepository _usuarios;

        public UsuarioService() : this(new UsuarioRepository()) { }

        internal UsuarioService(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }

        public ResultadoLogin Autenticar(string correo, string clave)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrEmpty(clave))
                return new ResultadoLogin { Exito = false, Mensaje = "Ingrese su correo y su contraseña." };

            var cred = _usuarios.ObtenerCredencialPorCorreo(correo.Trim());
            if (cred == null || !cred.Activo || !SeguridadClave.Verificar(clave, cred.Salt, cred.Hash))
                return new ResultadoLogin { Exito = false, Mensaje = "Correo o contraseña incorrectos." };

            return new ResultadoLogin { Exito = true, Usuario = cred.Usuario, Mensaje = "Bienvenido." };
        }
    }
}
