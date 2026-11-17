using System;
using System.Web;
using System.Web.UI.WebControls;
using UniCampus.Entities;

namespace UniCampus.Web
{
    /// <summary>Base de las páginas: exige sesión, controla el rol y ofrece utilidades de presentación.</summary>
    public abstract class PaginaBase : System.Web.UI.Page
    {
        protected virtual bool RequiereSesion { get { return true; } }

        /// <summary>Roles que pueden abrir la página; null = cualquier usuario con sesión.</summary>
        protected virtual string[] RolesPermitidos { get { return null; } }

        public UsuarioDto UsuarioActual
        {
            get { return Session["Usuario"] as UsuarioDto; }
        }

        protected string NombreUsuario { get { return UsuarioActual == null ? "" : UsuarioActual.Nombre; } }
        protected string RolUsuario { get { return UsuarioActual == null ? "" : UsuarioActual.Rol; } }
        protected bool EsAdmin { get { return UsuarioActual != null && UsuarioActual.Rol == Roles.Administrador; } }
        protected bool EsEstudiante { get { return UsuarioActual != null && UsuarioActual.Rol == Roles.Estudiante; } }

        protected override void OnInit(EventArgs e)
        {
            // Evita que el botón "atrás" muestre páginas privadas después de cerrar sesión.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (RequiereSesion)
            {
                var usuario = UsuarioActual;
                if (usuario == null)
                {
                    Response.Redirect("~/Login.aspx", true);
                    return;
                }

                var roles = RolesPermitidos;
                if (roles != null && Array.IndexOf(roles, usuario.Rol) < 0)
                {
                    Response.Redirect("~/Actividades.aspx", true);
                    return;
                }

                // Protección contra CSRF sobre el ViewState.
                ViewStateUserKey = Session.SessionID;
            }
            base.OnInit(e);
        }

        /// <summary>Muestra un mensaje codificado en HTML (previene XSS).</summary>
        protected void PintarMensaje(Label etiqueta, string mensaje, bool esError)
        {
            etiqueta.Text = Server.HtmlEncode(mensaje ?? "").Replace(Environment.NewLine, "<br />");
            etiqueta.CssClass = "mensaje " + (esError ? "error" : "ok");
        }
    }
}
