using System.Collections.Generic;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class ReporteRepository : IReporteRepository
    {
        public List<ResumenActividadDto> ObtenerResumen()
        {
            const string sql = @"
                SELECT a.ActividadId, a.Titulo, a.Estado, a.Cupos,
                       (SELECT COUNT(1) FROM dbo.Inscripcion i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa'),
                       (SELECT COUNT(1) FROM dbo.Resena r WHERE r.ActividadId = a.ActividadId),
                       (SELECT AVG(CAST(r.Calificacion AS FLOAT)) FROM dbo.Resena r WHERE r.ActividadId = a.ActividadId)
                FROM dbo.ActividadAcademica a
                ORDER BY a.FechaInicio DESC;";
            var lista = new List<ResumenActividadDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new ResumenActividadDto
                        {
                            ActividadId = r.GetInt32(0),
                            Titulo = r.GetString(1),
                            Estado = r.GetString(2),
                            Cupos = r.GetInt32(3),
                            Inscritos = r.GetInt32(4),
                            TotalResenas = r.GetInt32(5),
                            PromedioCalificacion = r.IsDBNull(6) ? (double?)null : r.GetDouble(6)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
