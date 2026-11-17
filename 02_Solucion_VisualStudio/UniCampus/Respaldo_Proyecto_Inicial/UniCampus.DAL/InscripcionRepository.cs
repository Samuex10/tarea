using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class InscripcionRepository : IInscripcionRepository
    {
        /// <summary>
        /// Inscripción atómica: bloquea la fila de la actividad (UPDLOCK, HOLDLOCK) dentro de una transacción,
        /// de modo que dos solicitudes simultáneas no puedan consumir el último cupo.
        /// </summary>
        public ResultadoInscripcion Inscribir(int actividadId, int usuarioId, DateTime ahora)
        {
            using (var cn = Conexion.Crear())
            {
                cn.Open();
                using (var tx = cn.BeginTransaction())
                {
                    string estado;
                    DateTime inicio;
                    int cupos;

                    using (var cmd = Comando(
                        "SELECT Estado, FechaInicio, Cupos FROM dbo.ActividadAcademica WITH (UPDLOCK, HOLDLOCK) WHERE ActividadId = @ActividadId;",
                        cn, tx))
                    {
                        cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (!r.Read()) return ResultadoInscripcion.ActividadNoExiste;
                            estado = r.GetString(0);
                            inicio = r.GetDateTime(1);
                            cupos = r.GetInt32(2);
                        }
                    }

                    if (estado != EstadosActividad.Publicada) return ResultadoInscripcion.NoPublicada;
                    if (inicio <= ahora) return ResultadoInscripcion.YaIniciada;

                    int? inscripcionId = null;
                    string estadoInscripcion = null;
                    using (var cmd = Comando(
                        "SELECT InscripcionId, Estado FROM dbo.Inscripcion WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;",
                        cn, tx))
                    {
                        cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                        cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                inscripcionId = r.GetInt32(0);
                                estadoInscripcion = r.GetString(1);
                            }
                        }
                    }
                    if (inscripcionId.HasValue && estadoInscripcion == "Activa")
                        return ResultadoInscripcion.Duplicada;

                    int inscritos;
                    using (var cmd = Comando(
                        "SELECT COUNT(1) FROM dbo.Inscripcion WHERE ActividadId = @ActividadId AND Estado = N'Activa';",
                        cn, tx))
                    {
                        cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                        inscritos = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    if (inscritos >= cupos) return ResultadoInscripcion.SinCupos;

                    if (inscripcionId.HasValue)
                    {
                        // Reactiva una inscripción cancelada (la restricción UNIQUE impide una segunda fila).
                        using (var cmd = Comando(
                            "UPDATE dbo.Inscripcion SET Estado = N'Activa', FechaInscripcion = @Ahora WHERE InscripcionId = @Id;",
                            cn, tx))
                        {
                            cmd.Parameters.Add("@Ahora", SqlDbType.DateTime2).Value = ahora;
                            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = inscripcionId.Value;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (var cmd = Comando(
                            "INSERT INTO dbo.Inscripcion (ActividadId, UsuarioId, FechaInscripcion, Estado) VALUES (@ActividadId, @UsuarioId, @Ahora, N'Activa');",
                            cn, tx))
                        {
                            cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                            cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                            cmd.Parameters.Add("@Ahora", SqlDbType.DateTime2).Value = ahora;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    tx.Commit();
                    return ResultadoInscripcion.Ok;
                }
            }
        }

        public int Cancelar(int actividadId, int usuarioId)
        {
            const string sql = @"
                UPDATE dbo.Inscripcion SET Estado = N'Cancelada'
                WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId AND Estado = N'Activa';";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public bool EstaInscrito(int actividadId, int usuarioId)
        {
            const string sql = @"
                SELECT COUNT(1) FROM dbo.Inscripcion
                WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId AND Estado = N'Activa';";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public List<InscripcionDto> ObtenerPorUsuario(int usuarioId)
        {
            const string sql = @"
                SELECT i.InscripcionId, i.ActividadId, a.Titulo, a.FechaInicio, a.Lugar, i.FechaInscripcion, i.Estado
                FROM dbo.Inscripcion i
                INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
                WHERE i.UsuarioId = @UsuarioId
                ORDER BY a.FechaInicio DESC;";
            var lista = new List<InscripcionDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(Mapear(r, false));
                }
            }
            return lista;
        }

        public List<InscripcionDto> ObtenerPorActividad(int actividadId)
        {
            const string sql = @"
                SELECT i.InscripcionId, i.ActividadId, a.Titulo, a.FechaInicio, a.Lugar, i.FechaInscripcion, i.Estado,
                       u.Nombre, u.Correo
                FROM dbo.Inscripcion i
                INNER JOIN dbo.ActividadAcademica a ON a.ActividadId = i.ActividadId
                INNER JOIN dbo.Usuario u ON u.UsuarioId = i.UsuarioId
                WHERE i.ActividadId = @ActividadId
                ORDER BY i.FechaInscripcion;";
            var lista = new List<InscripcionDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) lista.Add(Mapear(r, true));
                }
            }
            return lista;
        }

        private static SqlCommand Comando(string sql, SqlConnection cn, SqlTransaction tx)
        {
            return new SqlCommand(sql, cn, tx);
        }

        private static InscripcionDto Mapear(SqlDataReader r, bool conUsuario)
        {
            var d = new InscripcionDto
            {
                InscripcionId = r.GetInt32(0),
                ActividadId = r.GetInt32(1),
                ActividadTitulo = r.GetString(2),
                FechaInicio = r.GetDateTime(3),
                Lugar = r.IsDBNull(4) ? "" : r.GetString(4),
                FechaInscripcion = r.GetDateTime(5),
                Estado = r.GetString(6)
            };
            if (conUsuario)
            {
                d.UsuarioNombre = r.GetString(7);
                d.UsuarioCorreo = r.GetString(8);
            }
            return d;
        }
    }
}
