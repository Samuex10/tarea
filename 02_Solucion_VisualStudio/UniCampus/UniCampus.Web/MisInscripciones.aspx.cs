using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class MisInscripciones : PaginaBase, IMisInscripcionesView
    {
        private MisInscripcionesPresenter _presenter;

        protected override string[] RolesPermitidos { get { return new[] { Roles.Estudiante }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new MisInscripcionesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        protected void gvInscripciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Cancelar"
                && int.TryParse(Convert.ToString(e.CommandArgument), NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
            {
                _presenter.Cancelar(id);
            }
        }

        public void MostrarInscripciones(IList<InscripcionDto> inscripciones)
        {
            gvInscripciones.DataSource = inscripciones;
            gvInscripciones.DataBind();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            PintarMensaje(lblMensaje, mensaje, esError);
        }
    }
}
