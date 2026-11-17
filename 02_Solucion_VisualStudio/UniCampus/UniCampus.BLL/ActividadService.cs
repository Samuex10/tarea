using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>Reglas de negocio de actividades. No conoce controles ASPX ni escribe SQL.</summary>
    public class ActividadService
    {
        private const int MaxTitulo = 120, MaxDescripcion = 1000, MaxLugar = 150;

        private readonly IActividadRepository _actividades;
        private readonly ITipoActividadRepository _tipos;

        public ActividadService() : this(new ActividadRepository(), new TipoActividadRepository()) { }

        internal ActividadService(IActividadRepository actividades, ITipoActividadRepository tipos)
        {
            _actividades = actividades;
            _tipos = tipos;
        }

        /// <summary>Deja lista la base de datos (se llama al iniciar la aplicación).</summary>
        public static void PrepararBaseDeDatos()
        {
            BaseDatosInicializador.Asegurar();
        }

        /// <summary>Informe de conexión para la página local de diagnóstico.</summary>
        public static string Diagnosticar()
        {
            return BaseDatosInicializador.Diagnosticar();
        }

        public static bool EsAdmin(UsuarioDto u)
        {
            return u != null && u.Rol == Roles.Administrador;
        }

        public List<TipoActividadDto> ListarTipos()
        {
            return _tipos.Listar();
        }

        /// <summary>Busca actividades. Los estudiantes solo ven Publicada y Finalizada.</summary>
        public List<ActividadDto> Buscar(FiltroActividades filtro, UsuarioDto usuario)
        {
            if (usuario == null) return new List<ActividadDto>();
            if (filtro == null) filtro = new FiltroActividades();
            filtro.SoloVisiblesParaEstudiante = !EsAdmin(usuario);
            return _actividades.Buscar(filtro);
        }

        public ActividadDto ObtenerDetalle(int actividadId, UsuarioDto usuario)
        {
            if (usuario == null || actividadId <= 0) return null;
            var a = _actividades.ObtenerPorId(actividadId);
            if (a == null) return null;
            if (!EsAdmin(usuario) && a.Estado != EstadosActividad.Publicada && a.Estado != EstadosActividad.Finalizada)
                return null;
            return a;
        }

        public ResultadoOperacion Registrar(ActividadDto actividad, UsuarioDto usuario)
        {
            if (!EsAdmin(usuario))
                return ResultadoOperacion.Rechazado("No tiene permisos para registrar actividades.");

            var errores = Validar(actividad);
            if (actividad != null && actividad.FechaInicio != default(DateTime) && actividad.FechaInicio < DateTime.Today)
                errores.Add("La fecha de inicio no puede ser anterior a hoy.");
            if (errores.Count > 0)
                return ResultadoOperacion.Fallido(errores);

            actividad.Titulo = actividad.Titulo.Trim();
            int id = _actividades.Insertar(actividad);
            return ResultadoOperacion.Correcto(id, "Actividad registrada correctamente.");
        }

        public ResultadoOperacion Actualizar(ActividadDto actividad, UsuarioDto usuario)
        {
            if (!EsAdmin(usuario))
                return ResultadoOperacion.Rechazado("No tiene permisos para modificar actividades.");
            if (actividad == null || actividad.ActividadId <= 0)
                return ResultadoOperacion.Rechazado("No se indicó qué actividad modificar.");

            var existente = _actividades.ObtenerPorId(actividad.ActividadId);
            if (existente == null)
                return ResultadoOperacion.Rechazado("La actividad ya no existe.");

            var errores = Validar(actividad);
            if (actividad.Cupos < existente.Inscritos)
                errores.Add("El cupo no puede ser menor que el número de inscritos (" + existente.Inscritos + ").");
            if (errores.Count > 0)
                return ResultadoOperacion.Fallido(errores);

            actividad.Titulo = actividad.Titulo.Trim();
            int filas = _actividades.Actualizar(actividad);
            return filas > 0
                ? ResultadoOperacion.Correcto(actividad.ActividadId, "Actividad actualizada correctamente.")
                : ResultadoOperacion.Rechazado("No se pudo actualizar la actividad.");
        }

        /// <summary>Desactivar = marcar como Cancelada (no se pierde el historial).</summary>
        public ResultadoOperacion Desactivar(int actividadId, UsuarioDto usuario)
        {
            if (!EsAdmin(usuario))
                return ResultadoOperacion.Rechazado("No tiene permisos para desactivar actividades.");
            if (actividadId <= 0 || _actividades.ObtenerPorId(actividadId) == null)
                return ResultadoOperacion.Rechazado("La actividad no existe.");

            return _actividades.CambiarEstado(actividadId, EstadosActividad.Cancelada) > 0
                ? ResultadoOperacion.Correcto(actividadId, "Actividad desactivada (estado Cancelada).")
                : ResultadoOperacion.Rechazado("No se pudo desactivar la actividad.");
        }

        /// <summary>Elimina físicamente solo si no tiene inscripciones ni reseñas.</summary>
        public ResultadoOperacion Eliminar(int actividadId, UsuarioDto usuario)
        {
            if (!EsAdmin(usuario))
                return ResultadoOperacion.Rechazado("No tiene permisos para eliminar actividades.");
            if (actividadId <= 0 || _actividades.ObtenerPorId(actividadId) == null)
                return ResultadoOperacion.Rechazado("La actividad no existe.");
            if (_actividades.TieneDependencias(actividadId))
                return ResultadoOperacion.Rechazado(
                    "No se puede eliminar: la actividad tiene inscripciones o reseñas. Use Desactivar.");

            return _actividades.Eliminar(actividadId) > 0
                ? ResultadoOperacion.Correcto(actividadId, "Actividad eliminada correctamente.")
                : ResultadoOperacion.Rechazado("No se pudo eliminar la actividad.");
        }

        /// <summary>Validaciones de servidor (no dependen del navegador).</summary>
        public List<string> Validar(ActividadDto a)
        {
            var e = new List<string>();
            if (a == null) { e.Add("No se recibieron datos de la actividad."); return e; }

            if (string.IsNullOrWhiteSpace(a.Titulo)) e.Add("El título es obligatorio.");
            else if (a.Titulo.Trim().Length > MaxTitulo) e.Add("El título no puede superar " + MaxTitulo + " caracteres.");

            if (!string.IsNullOrEmpty(a.Descripcion) && a.Descripcion.Length > MaxDescripcion)
                e.Add("La descripción no puede superar " + MaxDescripcion + " caracteres.");
            if (!string.IsNullOrEmpty(a.Lugar) && a.Lugar.Length > MaxLugar)
                e.Add("El lugar no puede superar " + MaxLugar + " caracteres.");

            if (!a.TipoId.HasValue || a.TipoId.Value <= 0) e.Add("Seleccione el tipo de actividad.");

            if (a.FechaInicio == default(DateTime)) e.Add("La fecha de inicio es obligatoria.");
            if (a.FechaFin == default(DateTime)) e.Add("La fecha de fin es obligatoria.");
            if (a.FechaInicio != default(DateTime) && a.FechaFin != default(DateTime) && a.FechaFin < a.FechaInicio)
                e.Add("La fecha de fin no puede ser anterior a la de inicio.");

            if (a.Cupos <= 0) e.Add("El cupo debe ser un número mayor que cero.");

            if (Array.IndexOf(EstadosActividad.Todos, a.Estado) < 0)
                e.Add("El estado no es válido.");

            return e;
        }
    }
}
