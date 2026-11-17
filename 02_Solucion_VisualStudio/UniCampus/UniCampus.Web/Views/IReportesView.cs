using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IReportesView
    {
        UsuarioDto UsuarioActual { get; }
        void MostrarResumen(IList<ResumenActividadDto> resumen, string totales);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
