using System;
using UniCampus.BLL;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class LoginPresenter
    {
        private readonly ILoginView _view;
        private readonly UsuarioService _usuarios;

        public LoginPresenter(ILoginView view) : this(view, new UsuarioService()) { }

        public LoginPresenter(ILoginView view, UsuarioService usuarios)
        {
            _view = view;
            _usuarios = usuarios;
        }

        public void Ingresar()
        {
            try
            {
                var resultado = _usuarios.Autenticar(_view.Correo, _view.Clave);
                if (resultado.Exito) _view.IniciarSesion(resultado.Usuario);
                else _view.MostrarMensaje(resultado.Mensaje, true);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible iniciar sesión. Intente nuevamente.", true);
            }
        }
    }
}
