using System;
using System.Collections.Generic;
using UniCampus.Entities;
using UniCampus.Web.Presenters;
using UniCampus.Web.Views;

namespace UniCampus.Web
{
    public partial class Reportes : PaginaBase, IReportesView
    {
        private ReportesPresenter _presenter;

        protected override string[] RolesPermitidos { get { return new[] { Roles.Administrador }; } }

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new ReportesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) _presenter.Cargar();
        }

        public void MostrarResumen(IList<ResumenActividadDto> resumen, string totales)
        {
            lblTotales.Text = Server.HtmlEncode(totales);
            gvResumen.DataSource = resumen;
            gvResumen.DataBind();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            PintarMensaje(lblMensaje, mensaje, esError);
        }
    }
}
