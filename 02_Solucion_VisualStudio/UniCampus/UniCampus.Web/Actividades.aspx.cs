using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    /// <summary>Vista (MVP): solo muestra datos y delega las acciones al Presenter.</summary>
    public partial class Actividades : PaginaBase, IActividadesView
    {
        private ActividadesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ActividadesPresenter(this);
            gvActividades.Columns[9].Visible = EsAdmin;   // columna "Acciones": solo administrador
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlFormulario.Visible = EsAdmin;
            pnlFiltroEstado.Visible = EsAdmin;
            if (!IsPostBack) _presenter.Cargar();
        }

        protected void btnGuardar_Click(object sender, EventArgs e) { _presenter.Guardar(); }
        protected void btnCancelarEdicion_Click(object sender, EventArgs e) { LimpiarFormulario(); }
        protected void btnBuscar_Click(object sender, EventArgs e) { _presenter.Buscar(); }

        protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            txtFiltroTexto.Text = txtFiltroDesde.Text = txtFiltroHasta.Text = string.Empty;
            ddlFiltroTipo.SelectedIndex = 0;
            ddlFiltroEstado.SelectedIndex = 0;
            _presenter.Buscar();
        }

        protected void gvActividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
                return;
            switch (e.CommandName)
            {
                case "Editar": _presenter.Editar(id); break;
                case "Desactivar": _presenter.Desactivar(id); break;
                case "Eliminar": _presenter.Eliminar(id); break;
            }
        }

        // ----- IActividadesView -----
        public string ActividadId { get { return hfActividadId.Value; } }
        public string Titulo { get { return txtTitulo.Text; } }
        public string Descripcion { get { return txtDescripcion.Text; } }
        public string FechaInicio { get { return txtFechaInicio.Text; } }
        public string FechaFin { get { return txtFechaFin.Text; } }
        public string Lugar { get { return txtLugar.Text; } }
        public string Cupos { get { return txtCupos.Text; } }
        public string TipoId { get { return ddlTipo.SelectedValue; } }
        public string Estado { get { return ddlEstado.SelectedValue; } }

        public string FiltroTexto { get { return txtFiltroTexto.Text; } }
        public string FiltroDesde { get { return txtFiltroDesde.Text; } }
        public string FiltroHasta { get { return txtFiltroHasta.Text; } }
        public string FiltroTipoId { get { return ddlFiltroTipo.SelectedValue; } }
        public string FiltroEstado { get { return ddlFiltroEstado.SelectedValue; } }

        public void MostrarTipos(IList<TipoActividadDto> tipos)
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("Seleccione...", ""));
            ddlFiltroTipo.Items.Clear();
            ddlFiltroTipo.Items.Add(new ListItem("Todos", ""));
            foreach (var t in tipos)
            {
                string valor = t.TipoId.ToString(CultureInfo.InvariantCulture);
                ddlTipo.Items.Add(new ListItem(t.Nombre, valor));
                ddlFiltroTipo.Items.Add(new ListItem(t.Nombre, valor));
            }
        }

        public void MostrarActividades(IList<ActividadDto> actividades)
        {
            gvActividades.DataSource = actividades;
            gvActividades.DataBind();
        }

        public void CargarFormulario(ActividadDto a)
        {
            hfActividadId.Value = a.ActividadId.ToString(CultureInfo.InvariantCulture);
            txtTitulo.Text = a.Titulo;
            txtDescripcion.Text = a.Descripcion;
            txtFechaInicio.Text = a.FechaInicio.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            txtFechaFin.Text = a.FechaFin.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            txtLugar.Text = a.Lugar;
            txtCupos.Text = a.Cupos.ToString(CultureInfo.InvariantCulture);
            Seleccionar(ddlTipo, a.TipoId.HasValue ? a.TipoId.Value.ToString(CultureInfo.InvariantCulture) : "");
            Seleccionar(ddlEstado, a.Estado);
            lblModo.Text = "Editando la actividad #" + a.ActividadId.ToString(CultureInfo.InvariantCulture);
            btnGuardar.Text = "Actualizar";
            btnCancelarEdicion.Visible = true;
            PintarMensaje(lblMensaje, "", false);
        }

        public void LimpiarFormulario()
        {
            hfActividadId.Value = string.Empty;
            txtTitulo.Text = txtDescripcion.Text = txtFechaInicio.Text =
                txtFechaFin.Text = txtLugar.Text = txtCupos.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
            ddlEstado.SelectedIndex = 0;
            lblModo.Text = "Nueva actividad";
            btnGuardar.Text = "Guardar";
            btnCancelarEdicion.Visible = false;
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            PintarMensaje(lblMensaje, mensaje, esError);
        }

        private static void Seleccionar(DropDownList lista, string valor)
        {
            lista.ClearSelection();
            var item = lista.Items.FindByValue(valor);
            if (item != null) item.Selected = true;
        }
    }
}
