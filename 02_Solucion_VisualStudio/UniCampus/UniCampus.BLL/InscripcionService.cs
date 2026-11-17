using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class InscripcionService
    {
        private readonly IInscripcionRepository _inscripciones;
        private readonly IActividadRepository _actividades;

        public InscripcionService() : this(new InscripcionRepository(), new ActividadRepository()) { }

        internal InscripcionService(IInscripcionRepository inscripciones, IActividadRepository actividades)
        {
            _inscripciones = inscripciones;
            _actividades = actividades;
        }

        public bool PuedeInscribirse(ActividadDto a, UsuarioDto u, bool yaInscrito)
        {
            return a != null && u != null && u.Rol == Roles.Estudiante && !yaInscrito
                && a.Estado == EstadosActividad.Publicada
                && a.FechaInicio > DateTime.Now
                && a.CuposDisponibles > 0;
        }

        public bool PuedeCancelar(ActividadDto a, UsuarioDto u, bool yaInscrito)
        {
            return a != null && u != null && u.Rol == Roles.Estudiante && yaInscrito && a.FechaInicio > DateTime.Now;
        }

        public ResultadoOperacion Inscribir(int actividadId, UsuarioDto usuario)
        {
            if (usuario == null || usuario.Rol != Roles.Estudiante)
                return ResultadoOperacion.Rechazado("Solo los estudiantes pueden inscribirse a una actividad.");
            if (actividadId <= 0)
                return ResultadoOperacion.Rechazado("La actividad no existe.");

            switch (_inscripciones.Inscribir(actividadId, usuario.UsuarioId, DateTime.Now))
            {
                case ResultadoInscripcion.Ok:
                    return ResultadoOperacion.Correcto(actividadId, "Inscripción realizada correctamente.");
                case ResultadoInscripcion.ActividadNoExiste:
                    return ResultadoOperacion.Rechazado("La actividad no existe.");
                case ResultadoInscripcion.NoPublicada:
                    return ResultadoOperacion.Rechazado("La actividad no está disponible para inscripción.");
                case ResultadoInscripcion.YaIniciada:
                    return ResultadoOperacion.Rechazado("La actividad ya inició; no es posible inscribirse.");
                case ResultadoInscripcion.SinCupos:
                    return ResultadoOperacion.Rechazado("No hay cupos disponibles para esta actividad.");
                case ResultadoInscripcion.Duplicada:
                    return ResultadoOperacion.Rechazado("Ya está inscrito en esta actividad.");
                default:
                    return ResultadoOperacion.Rechazado("No fue posible completar la inscripción.");
            }
        }

        public ResultadoOperacion Cancelar(int actividadId, UsuarioDto usuario)
        {
            if (usuario == null || usuario.Rol != Roles.Estudiante)
                return ResultadoOperacion.Rechazado("Solo los estudiantes pueden cancelar su inscripción.");

            var a = _actividades.ObtenerPorId(actividadId);
            if (a == null) return ResultadoOperacion.Rechazado("La actividad no existe.");
            if (a.FechaInicio <= DateTime.Now)
                return ResultadoOperacion.Rechazado("No se puede cancelar la inscripción de una actividad que ya inició.");

            return _inscripciones.Cancelar(actividadId, usuario.UsuarioId) > 0
                ? ResultadoOperacion.Correcto(actividadId, "Inscripción cancelada.")
                : ResultadoOperacion.Rechazado("No tiene una inscripción activa en esta actividad.");
        }

        public bool EstaInscrito(int actividadId, UsuarioDto usuario)
        {
            return usuario != null && _inscripciones.EstaInscrito(actividadId, usuario.UsuarioId);
        }

        public List<InscripcionDto> MisInscripciones(UsuarioDto usuario)
        {
            if (usuario == null) return new List<InscripcionDto>();
            return _inscripciones.ObtenerPorUsuario(usuario.UsuarioId);
        }

        public List<InscripcionDto> ListarInscritos(int actividadId, UsuarioDto usuario)
        {
            if (!ActividadService.EsAdmin(usuario)) return new List<InscripcionDto>();
            return _inscripciones.ObtenerPorActividad(actividadId);
        }
    }
}
