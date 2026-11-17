using System;
using UniCampus.BLL;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    public class MisInscripcionesPresenter
    {
        private readonly IMisInscripcionesView _view;
        private readonly InscripcionService _service;

        public MisInscripcionesPresenter(IMisInscripcionesView view) : this(view, new InscripcionService()) { }

        public MisInscripcionesPresenter(IMisInscripcionesView view, InscripcionService service)
        {
            _view = view;
            _service = service;
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarInscripciones(_service.MisInscripciones(_view.UsuarioActual));
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible cargar sus inscripciones. Intente nuevamente.", true);
            }
        }

        public void Cancelar(int actividadId)
        {
            try
            {
                var resultado = _service.Cancelar(actividadId, _view.UsuarioActual);
                Cargar();
                _view.MostrarMensaje(resultado.TextoCompleto(), !resultado.Exito);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible cancelar la inscripción. Intente nuevamente.", true);
            }
        }
    }
}
