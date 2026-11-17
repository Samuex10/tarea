using System;
using System.Collections.Generic;
using System.Globalization;
using UniCampus.BLL;
using UniCampus.Entities;
using UniCampus.Web.Views;

namespace UniCampus.Web.Presenters
{
    /// <summary>Coordina la vista de actividades y el servicio de negocio.</summary>
    public class ActividadesPresenter
    {
        private readonly IActividadesView _view;
        private readonly ActividadService _service;

        public ActividadesPresenter(IActividadesView view) : this(view, new ActividadService()) { }

        public ActividadesPresenter(IActividadesView view, ActividadService service)
        {
            _view = view;
            _service = service;
        }

        public void Cargar()
        {
            try
            {
                _view.MostrarTipos(_service.ListarTipos());
                Buscar();
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible cargar las actividades. Intente nuevamente.", true);
            }
        }

        public void Buscar()
        {
            try
            {
                var errores = new List<string>();
                var filtro = new FiltroActividades { Texto = _view.FiltroTexto, Estado = _view.FiltroEstado };

                DateTime? desde, hasta;
                if (!TryFechaOpcional(_view.FiltroDesde, out desde)) errores.Add("La fecha 'desde' no es válida.");
                if (!TryFechaOpcional(_view.FiltroHasta, out hasta)) errores.Add("La fecha 'hasta' no es válida.");
                filtro.FechaDesde = desde;
                filtro.FechaHasta = hasta;

                int tipo;
                if (int.TryParse(_view.FiltroTipoId, NumberStyles.Integer, CultureInfo.InvariantCulture, out tipo) && tipo > 0)
                    filtro.TipoId = tipo;

                if (desde.HasValue && hasta.HasValue && hasta.Value < desde.Value)
                    errores.Add("La fecha 'hasta' no puede ser anterior a la fecha 'desde'.");

                if (errores.Count > 0)
                {
                    _view.MostrarMensaje(string.Join(Environment.NewLine, errores), true);
                    return;
                }

                _view.MostrarActividades(_service.Buscar(filtro, _view.UsuarioActual));
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible consultar las actividades. Intente nuevamente.", true);
            }
        }

        public void Guardar()
        {
            try
            {
                var errores = new List<string>();
                var dto = new ActividadDto
                {
                    Titulo = _view.Titulo,
                    Descripcion = _view.Descripcion,
                    Lugar = _view.Lugar,
                    Estado = _view.Estado
                };

                int id, tipo, cupos;
                int.TryParse(_view.ActividadId, NumberStyles.Integer, CultureInfo.InvariantCulture, out id);
                dto.ActividadId = id;
                if (int.TryParse(_view.TipoId, NumberStyles.Integer, CultureInfo.InvariantCulture, out tipo)) dto.TipoId = tipo;

                DateTime inicio, fin;
                if (TryFecha(_view.FechaInicio, out inicio)) dto.FechaInicio = inicio;
                else errores.Add("La fecha de inicio no tiene un formato válido.");
                if (TryFecha(_view.FechaFin, out fin)) dto.FechaFin = fin;
                else errores.Add("La fecha de fin no tiene un formato válido.");
                if (int.TryParse(_view.Cupos, NumberStyles.Integer, CultureInfo.InvariantCulture, out cupos)) dto.Cupos = cupos;
                else errores.Add("El cupo debe ser un número entero.");

                if (errores.Count > 0)
                {
                    _view.MostrarMensaje(string.Join(Environment.NewLine, errores), true);
                    return;
                }

                var usuario = _view.UsuarioActual;
                var resultado = id > 0 ? _service.Actualizar(dto, usuario) : _service.Registrar(dto, usuario);
                if (resultado.Exito)
                {
                    _view.LimpiarFormulario();
                    Buscar();
                    _view.MostrarMensaje(resultado.Mensaje, false);
                }
                else
                {
                    _view.MostrarMensaje(resultado.TextoCompleto(), true);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("Ocurrió un error al guardar la actividad. Intente nuevamente.", true);
            }
        }

        public void Editar(int actividadId)
        {
            try
            {
                var a = _service.ObtenerDetalle(actividadId, _view.UsuarioActual);
                if (a == null) _view.MostrarMensaje("La actividad no existe.", true);
                else _view.CargarFormulario(a);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible cargar la actividad.", true);
            }
        }

        public void Desactivar(int actividadId)
        {
            Ejecutar(() => _service.Desactivar(actividadId, _view.UsuarioActual));
        }

        public void Eliminar(int actividadId)
        {
            Ejecutar(() => _service.Eliminar(actividadId, _view.UsuarioActual));
        }

        private void Ejecutar(Func<ResultadoOperacion> operacion)
        {
            try
            {
                var resultado = operacion();
                Buscar();
                _view.MostrarMensaje(resultado.TextoCompleto(), !resultado.Exito);
            }
            catch (Exception ex)
            {
                ErrorLog.Registrar(ex);
                _view.MostrarMensaje("No fue posible completar la operación. Intente nuevamente.", true);
            }
        }

        private static bool TryFecha(string texto, out DateTime valor)
        {
            return DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.None, out valor);
        }

        private static bool TryFechaOpcional(string texto, out DateTime? valor)
        {
            valor = null;
            if (string.IsNullOrWhiteSpace(texto)) return true;
            DateTime d;
            if (!TryFecha(texto, out d)) return false;
            valor = d;
            return true;
        }
    }
}
