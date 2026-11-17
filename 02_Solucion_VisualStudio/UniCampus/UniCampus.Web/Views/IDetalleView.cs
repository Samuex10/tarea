using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IDetalleView
    {
        UsuarioDto UsuarioActual { get; }
        int ActividadIdSolicitada { get; }
        string Calificacion { get; }
        string Comentario { get; }

        void MostrarActividad(ActividadDto actividad);
        void MostrarNoEncontrada();
        void MostrarAcciones(bool puedeInscribirse, bool puedeCancelar, bool puedeResenar);
        void MostrarResenas(IList<ResenaDto> resenas, double? promedio);
        void MostrarInscritos(IList<InscripcionDto> inscritos);
        void LimpiarResena();
        void MostrarMensaje(string mensaje, bool esError);
    }
}
