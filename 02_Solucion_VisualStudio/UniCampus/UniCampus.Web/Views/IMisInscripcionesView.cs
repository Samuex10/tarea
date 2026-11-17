using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.Web.Views
{
    public interface IMisInscripcionesView
    {
        UsuarioDto UsuarioActual { get; }
        void MostrarInscripciones(IList<InscripcionDto> inscripciones);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
