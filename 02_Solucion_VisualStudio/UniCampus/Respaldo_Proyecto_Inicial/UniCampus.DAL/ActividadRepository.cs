using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    /// <summary>Acceso a datos de actividades. Solo ADO.NET con consultas parametrizadas.</summary>
    public class ActividadRepository : IActividadRepository
    {
        private const string Columnas =
            "a.ActividadId, a.Titulo, a.Descripcion, a.FechaInicio, a.FechaFin, a.Lugar, a.Cupos, a.Estado, a.TipoId, t.Nombre, " +
            "(SELECT COUNT(1) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') ";

        private const string Desde =
            "FROM dbo.ActividadAcademica a LEFT JOIN dbo.TipoActividad t ON t.TipoId = a.TipoId ";

        public int Insertar(ActividadDto a)
        {
            const string sql = @"
                INSERT INTO dbo.ActividadAcademica
                    (Titulo, Descripcion, FechaInicio, FechaFin, Lugar, Cupos, Estado, TipoId)
                OUTPUT INSERTED.ActividadId
                VALUES
                    (@Titulo, @Descripcion, @FechaInicio, @FechaFin, @Lugar, @Cupos, @Estado, @TipoId);";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                AgregarParametros(cmd, a);
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int Actualizar(ActividadDto a)
        {
            const string sql = @"
                UPDATE dbo.ActividadAcademica
                SET Titulo = @Titulo, Descripcion = @Descripcion, FechaInicio = @FechaInicio,
                    FechaFin = @FechaFin, Lugar = @Lugar, Cupos = @Cupos, Estado = @Estado, TipoId = @TipoId
                WHERE ActividadId = @ActividadId;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                AgregarParametros(cmd, a);
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = a.ActividadId;
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int CambiarEstado(int actividadId, string estado)
        {
            const string sql = "UPDATE dbo.ActividadAcademica SET Estado = @Estado WHERE ActividadId = @ActividadId;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = estado;
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int Eliminar(int actividadId)
        {
            const string sql = "DELETE FROM dbo.ActividadAcademica WHERE ActividadId = @ActividadId;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public bool TieneDependencias(int actividadId)
        {
            const string sql = @"
                SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.Inscripcion WHERE ActividadId = @ActividadId)
                              OR EXISTS (SELECT 1 FROM dbo.Resena WHERE ActividadId = @ActividadId)
                            THEN 1 ELSE 0 END;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        public ActividadDto ObtenerPorId(int actividadId)
        {
            const string sql = "SELECT " + Columnas + Desde + "WHERE a.ActividadId = @ActividadId;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    return r.Read() ? Mapear(r) : null;
                }
            }
        }

        public List<ActividadDto> Buscar(FiltroActividades f)
        {
            if (f == null) f = new FiltroActividades();

            const string sql = "SELECT " + Columnas + Desde + @"
                WHERE (@Texto IS NULL OR a.Titulo LIKE @Texto ESCAPE N'\' OR a.Descripcion LIKE @Texto ESCAPE N'\')
                  AND (@Desde IS NULL OR a.FechaInicio >= @Desde)
                  AND (@Hasta IS NULL OR a.FechaInicio < @Hasta)
                  AND (@TipoId IS NULL OR a.TipoId = @TipoId)
                  AND (@Estado IS NULL OR a.Estado = @Estado)
                  AND (@Solo = 0 OR a.Estado IN (N'Publicada', N'Finalizada'))
                ORDER BY a.FechaInicio DESC, a.ActividadId DESC;";

            var resultado = new List<ActividadDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                string texto = string.IsNullOrWhiteSpace(f.Texto) ? null : "%" + EscaparLike(f.Texto.Trim()) + "%";
                cmd.Parameters.Add("@Texto", SqlDbType.NVarChar, 300).Value = (object)texto ?? DBNull.Value;
                cmd.Parameters.Add("@Desde", SqlDbType.DateTime2).Value = (object)f.FechaDesde ?? DBNull.Value;
                cmd.Parameters.Add("@Hasta", SqlDbType.DateTime2).Value =
                    f.FechaHasta.HasValue ? (object)f.FechaHasta.Value.Date.AddDays(1) : DBNull.Value;
                cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = (object)f.TipoId ?? DBNull.Value;
                cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(f.Estado) ? DBNull.Value : (object)f.Estado;
                cmd.Parameters.Add("@Solo", SqlDbType.Bit).Value = f.SoloVisiblesParaEstudiante;

                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) resultado.Add(Mapear(r));
                }
            }
            return resultado;
        }

        private static string EscaparLike(string s)
        {
            return s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
        }

        private static void AgregarParametros(SqlCommand cmd, ActividadDto a)
        {
            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 120).Value = a.Titulo;
            cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, 1000).Value =
                string.IsNullOrWhiteSpace(a.Descripcion) ? DBNull.Value : (object)a.Descripcion;
            cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime2).Value = a.FechaInicio;
            cmd.Parameters.Add("@FechaFin", SqlDbType.DateTime2).Value = a.FechaFin;
            cmd.Parameters.Add("@Lugar", SqlDbType.NVarChar, 150).Value =
                string.IsNullOrWhiteSpace(a.Lugar) ? DBNull.Value : (object)a.Lugar;
            cmd.Parameters.Add("@Cupos", SqlDbType.Int).Value = a.Cupos;
            cmd.Parameters.Add("@Estado", SqlDbType.NVarChar, 20).Value = a.Estado;
            cmd.Parameters.Add("@TipoId", SqlDbType.Int).Value = (object)a.TipoId ?? DBNull.Value;
        }

        private static ActividadDto Mapear(SqlDataReader r)
        {
            return new ActividadDto
            {
                ActividadId = r.GetInt32(0),
                Titulo = r.GetString(1),
                Descripcion = r.IsDBNull(2) ? "" : r.GetString(2),
                FechaInicio = r.GetDateTime(3),
                FechaFin = r.GetDateTime(4),
                Lugar = r.IsDBNull(5) ? "" : r.GetString(5),
                Cupos = r.GetInt32(6),
                Estado = r.GetString(7),
                TipoId = r.IsDBNull(8) ? (int?)null : r.GetInt32(8),
                TipoNombre = r.IsDBNull(9) ? "" : r.GetString(9),
                Inscritos = r.GetInt32(10)
            };
        }
    }
}
