using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface ILoginView
    {
        string Correo { get; }
        string Clave { get; }
        void MostrarMensaje(string mensaje, bool esError);
        void IniciarSesion(UsuarioDto usuario);
    }
}
