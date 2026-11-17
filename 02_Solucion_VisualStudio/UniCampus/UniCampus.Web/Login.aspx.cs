using System;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>Vista (MVP) del inicio de sesión.</summary>
    public partial class Login : PaginaBase, ILoginView
    {
        private LoginPresenter _presenter;

        protected override bool RequiereSesion { get { return false; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new LoginPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && UsuarioActual != null)
                Response.Redirect("~/Actividades.aspx");
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            _presenter.Ingresar();
        }

        public string Correo { get { return txtCorreo.Text; } }
        public string Clave { get { return txtClave.Text; } }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            PintarMensaje(lblMensaje, mensaje, esError);
        }

        public void IniciarSesion(UsuarioDto usuario)
        {
            Session.Clear();
            Session["Usuario"] = usuario;
            Response.Redirect("~/Actividades.aspx");
        }
    }
}
