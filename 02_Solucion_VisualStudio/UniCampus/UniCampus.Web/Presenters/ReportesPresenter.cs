using System;
using System.Linq;
using UniCampus.BLL;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class ReportesPresenter
    {
        private readonly IReportesView _view;
        private readonly ReporteService _service;

        public ReportesPresenter(IReportesView view) : this(view, new ReporteService()) { }

        public ReportesPresenter(IReportesView view, ReporteService service)
        {
            _view = view;
            _service = service;
        }

        public void Cargar()
        {
            try
            {
                var resumen = _service.Resumen(_view.UsuarioActual);
                string totales = "Actividades: " + resumen.Count
                    + "  |  Inscripciones activas: " + resumen.Sum(r => r.Inscritos)
                    + "  |  Reseñas: " + resumen.Sum(r => r.TotalResenas);
                _view.MostrarResumen(resumen, totales);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible generar el reporte. Intente nuevamente.", true);
            }
        }
    }
}
