using System.Collections.Generic;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class TipoActividadRepository : ITipoActividadRepository
    {
        public List<TipoActividadDto> Listar()
        {
            const string sql = "SELECT TipoId, Nombre FROM dbo.TipoActividad ORDER BY Nombre;";
            var lista = new List<TipoActividadDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        lista.Add(new TipoActividadDto { TipoId = r.GetInt32(0), Nombre = r.GetString(1) });
                }
            }
            return lista;
        }
    }
}
