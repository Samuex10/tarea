using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    /// <summary>Contrato de la vista de actividades (MVP). Sin HTML ni SQL.</summary>
    public interface IActividadesView
    {
        UsuarioDto UsuarioActual { get; }

        // Formulario de actividad (la vista los expone como texto; el Presenter los interpreta)
        string ActividadId { get; }
        string Titulo { get; }
        string Descripcion { get; }
        string FechaInicio { get; }
        string FechaFin { get; }
        string Lugar { get; }
        string Cupos { get; }
        string TipoId { get; }
        string Estado { get; }

        // Filtros de búsqueda
        string FiltroTexto { get; }
        string FiltroDesde { get; }
        string FiltroHasta { get; }
        string FiltroTipoId { get; }
        string FiltroEstado { get; }

        void MostrarTipos(IList<TipoActividadDto> tipos);
        void MostrarActividades(IList<ActividadDto> actividades);
        void CargarFormulario(ActividadDto actividad);
        void LimpiarFormulario();
        void MostrarMensaje(string mensaje, bool esError);
    }
}
