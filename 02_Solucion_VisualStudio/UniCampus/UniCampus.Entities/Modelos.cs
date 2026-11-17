using System;
using System.Globalization;

namespace UniCampus.Entities
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Estudiante = "Estudiante";
    }

    public static class EstadosActividad
    {
        public const string Borrador = "Borrador";
        public const string Publicada = "Publicada";
        public const string Cancelada = "Cancelada";
        public const string Finalizada = "Finalizada";
        public static readonly string[] Todos = { Borrador, Publicada, Cancelada, Finalizada };
    }

    public enum ResultadoInscripcion
    {
        Ok,
        ActividadNoExiste,
        NoPublicada,
        YaIniciada,
        SinCupos,
        Duplicada
    }

    [Serializable]
    public class UsuarioDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
    }

    /// <summary>Datos de autenticación de un usuario (hash y sal de la contraseña).</summary>
    public class CredencialUsuario
    {
        public UsuarioDto Usuario { get; set; }
        public byte[] Hash { get; set; }
        public byte[] Salt { get; set; }
        public bool Activo { get; set; }
    }

    public class TipoActividadDto
    {
        public int TipoId { get; set; }
        public string Nombre { get; set; }
    }

    public class InscripcionDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public string ActividadTitulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public string Lugar { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string Estado { get; set; }
        public string UsuarioNombre { get; set; }
        public string UsuarioCorreo { get; set; }
    }

    public class ResenaDto
    {
        public int ResenaId { get; set; }
        public int ActividadId { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class FiltroActividades
    {
        public string Texto { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int? TipoId { get; set; }
        public string Estado { get; set; }
        /// <summary>Si es true, solo se devuelven actividades Publicada o Finalizada (vista de estudiante).</summary>
        public bool SoloVisiblesParaEstudiante { get; set; }
    }

    public class ResumenActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Estado { get; set; }
        public int Cupos { get; set; }
        public int Inscritos { get; set; }
        public int TotalResenas { get; set; }
        public double? PromedioCalificacion { get; set; }

        public int CuposDisponibles { get { return Math.Max(0, Cupos - Inscritos); } }

        public string PromedioTexto
        {
            get
            {
                return PromedioCalificacion.HasValue
                    ? PromedioCalificacion.Value.ToString("0.0", CultureInfo.InvariantCulture)
                    : "-";
            }
        }
    }
}
