using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    /// <summary>
    /// Reglas de reseñas: solo estudiantes inscritos, la actividad ya debe haber comenzado,
    /// calificación entera de 1 a 5 y una sola reseña por estudiante y actividad.
    /// </summary>
    public class ResenaService
    {
        private const int MaxComentario = 1000;

        private readonly IResenaRepository _resenas;
        private readonly IActividadRepository _actividades;
        private readonly IInscripcionRepository _inscripciones;

        public ResenaService()
            : this(new ResenaRepository(), new ActividadRepository(), new InscripcionRepository()) { }

        internal ResenaService(IResenaRepository resenas, IActividadRepository actividades, IInscripcionRepository inscripciones)
        {
            _resenas = resenas;
            _actividades = actividades;
            _inscripciones = inscripciones;
        }

        public bool PuedeResenar(ActividadDto a, UsuarioDto u, bool inscrito, bool yaReseno)
        {
            return a != null && u != null && u.Rol == Roles.Estudiante
                && inscrito && !yaReseno && a.FechaInicio <= DateTime.Now;
        }

        public bool YaReseno(int actividadId, UsuarioDto usuario)
        {
            return usuario != null && _resenas.Existe(actividadId, usuario.UsuarioId);
        }

        public List<ResenaDto> ListarPorActividad(int actividadId)
        {
            return _resenas.ObtenerPorActividad(actividadId);
        }

        public ResultadoOperacion Agregar(int actividadId, int calificacion, string comentario, UsuarioDto usuario)
        {
            if (usuario == null || usuario.Rol != Roles.Estudiante)
                return ResultadoOperacion.Rechazado("Solo los estudiantes pueden publicar reseñas.");

            var errores = new List<string>();
            if (calificacion < 1 || calificacion > 5)
                errores.Add("La calificación debe estar entre 1 y 5.");
            if (!string.IsNullOrEmpty(comentario) && comentario.Length > MaxComentario)
                errores.Add("El comentario no puede superar " + MaxComentario + " caracteres.");
            if (errores.Count > 0) return ResultadoOperacion.Fallido(errores);

            var a = _actividades.ObtenerPorId(actividadId);
            if (a == null) return ResultadoOperacion.Rechazado("La actividad no existe.");
            if (a.FechaInicio > DateTime.Now)
                return ResultadoOperacion.Rechazado("Solo puede reseñar actividades que ya comenzaron.");
            if (!_inscripciones.EstaInscrito(actividadId, usuario.UsuarioId))
                return ResultadoOperacion.Rechazado("Solo pueden reseñar quienes estuvieron inscritos en la actividad.");
            if (_resenas.Existe(actividadId, usuario.UsuarioId))
                return ResultadoOperacion.Rechazado("Ya publicó una reseña para esta actividad.");

            int id = _resenas.Insertar(new ResenaDto
            {
                ActividadId = actividadId,
                UsuarioId = usuario.UsuarioId,
                Calificacion = calificacion,
                Comentario = comentario == null ? null : comentario.Trim(),
                Fecha = DateTime.Now
            });
            return ResultadoOperacion.Correcto(id, "Reseña publicada correctamente.");
        }
    }
}
