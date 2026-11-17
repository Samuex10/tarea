using System;
using System.Globalization;
using System.Linq;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class DetallePresenter
    {
        private readonly IDetalleView _view;
        private readonly ActividadService _actividades;
        private readonly InscripcionService _inscripciones;
        private readonly ResenaService _resenas;

        public DetallePresenter(IDetalleView view)
            : this(view, new ActividadService(), new InscripcionService(), new ResenaService()) { }

        public DetallePresenter(IDetalleView view, ActividadService actividades,
            InscripcionService inscripciones, ResenaService resenas)
        {
            _view = view;
            _actividades = actividades;
            _inscripciones = inscripciones;
            _resenas = resenas;
        }

        public void Cargar()
        {
            try
            {
                var usuario = _view.UsuarioActual;
                var a = _actividades.ObtenerDetalle(_view.ActividadIdSolicitada, usuario);
                if (a == null)
                {
                    _view.MostrarNoEncontrada();
                    return;
                }

                _view.MostrarActividad(a);

                bool inscrito = false, yaReseno = false;
                if (usuario.Rol == Roles.Estudiante)
                {
                    inscrito = _inscripciones.EstaInscrito(a.ActividadId, usuario);
                    yaReseno = _resenas.YaReseno(a.ActividadId, usuario);
                }
                _view.MostrarAcciones(
                    _inscripciones.PuedeInscribirse(a, usuario, inscrito),
                    _inscripciones.PuedeCancelar(a, usuario, inscrito),
                    _resenas.PuedeResenar(a, usuario, inscrito, yaReseno));

                var lista = _resenas.ListarPorActividad(a.ActividadId);
                _view.MostrarResenas(lista, lista.Count == 0 ? (double?)null : lista.Average(r => (double)r.Calificacion));

                if (usuario.Rol == Roles.Administrador)
                    _view.MostrarInscritos(_inscripciones.ListarInscritos(a.ActividadId, usuario));
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible cargar la actividad. Intente nuevamente.", true);
            }
        }

        public void Inscribirse()
        {
            Ejecutar(() => _inscripciones.Inscribir(_view.ActividadIdSolicitada, _view.UsuarioActual), false);
        }

        public void CancelarInscripcion()
        {
            Ejecutar(() => _inscripciones.Cancelar(_view.ActividadIdSolicitada, _view.UsuarioActual), false);
        }

        public void PublicarResena()
        {
            int calificacion;
            if (!int.TryParse(_view.Calificacion, NumberStyles.Integer, CultureInfo.InvariantCulture, out calificacion))
                calificacion = 0;
            Ejecutar(() => _resenas.Agregar(_view.ActividadIdSolicitada, calificacion, _view.Comentario, _view.UsuarioActual), true);
        }

        private void Ejecutar(Func<ResultadoOperacion> operacion, bool limpiarResena)
        {
            try
            {
                var resultado = operacion();
                if (resultado.Exito && limpiarResena) _view.LimpiarResena();
                Cargar();
                _view.MostrarMensaje(resultado.TextoCompleto(), !resultado.Exito);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible completar la operación. Intente nuevamente.", true);
            }
        }
    }
}
