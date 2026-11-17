using System;
using System.Collections.Generic;
using System.Globalization;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>Vista (MVP) del detalle de una actividad: inscripción y reseñas.</summary>
    public partial class Detalle : PaginaBase, IDetalleView
    {
        private DetallePresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new DetallePresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        protected void btnInscribirse_Click(object sender, EventArgs e) { _presenter.Inscribirse(); }
        protected void btnCancelarInscripcion_Click(object sender, EventArgs e) { _presenter.CancelarInscripcion(); }
        protected void btnPublicarResena_Click(object sender, EventArgs e) { _presenter.PublicarResena(); }

        // ----- IDetalleView -----
        public int ActividadIdSolicitada
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], NumberStyles.Integer, CultureInfo.InvariantCulture, out id) ? id : 0;
            }
        }

        public string Calificacion { get { return ddlCalificacion.SelectedValue; } }
        public string Comentario { get { return txtComentario.Text; } }

        public void MostrarActividad(ActividadDto a)
        {
            lblTitulo.Text = Server.HtmlEncode(a.Titulo);
            lblTipo.Text = Server.HtmlEncode(string.IsNullOrEmpty(a.TipoNombre) ? "Sin tipo" : a.TipoNombre);
            lblEstado.Text = Server.HtmlEncode(a.Estado);
            lblFechas.Text = a.FechaInicio.ToString("dd/MM/yyyy HH:mm") + " - " + a.FechaFin.ToString("dd/MM/yyyy HH:mm");
            lblLugar.Text = Server.HtmlEncode(string.IsNullOrEmpty(a.Lugar) ? "Por definir" : a.Lugar);
            lblCupos.Text = a.CuposDisponibles + " disponibles de " + a.Cupos + " (" + a.Inscritos + " inscritos)";
            lblDescripcion.Text = Server.HtmlEncode(string.IsNullOrEmpty(a.Descripcion) ? "Sin descripción." : a.Descripcion)
                .Replace(Environment.NewLine, "<br />");
            pnlContenido.Visible = true;
        }

        public void MostrarNoEncontrada()
        {
            pnlContenido.Visible = false;
            PintarMensaje(lblMensaje, "La actividad no existe o no está disponible.", true);
        }

        public void MostrarAcciones(bool puedeInscribirse, bool puedeCancelar, bool puedeResenar)
        {
            btnInscribirse.Visible = puedeInscribirse;
            btnCancelarInscripcion.Visible = puedeCancelar;
            pnlResena.Visible = puedeResenar;
        }

        public void MostrarResenas(IList<ResenaDto> resenas, double? promedio)
        {
            lblPromedio.Text = promedio.HasValue
                ? "Promedio: " + promedio.Value.ToString("0.0", CultureInfo.InvariantCulture) + " / 5 (" + resenas.Count + " reseñas)"
                : "Aún no hay reseñas.";
            gvResenas.DataSource = resenas;
            gvResenas.DataBind();
        }

        public void MostrarInscritos(IList<InscripcionDto> inscritos)
        {
            pnlInscritos.Visible = true;
            gvInscritos.DataSource = inscritos;
            gvInscritos.DataBind();
        }

        public void LimpiarResena()
        {
            ddlCalificacion.SelectedIndex = 0;
            txtComentario.Text = string.Empty;
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            PintarMensaje(lblMensaje, mensaje, esError);
        }
    }
}
