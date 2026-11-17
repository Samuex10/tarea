using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class ResenaRepository : IResenaRepository
    {
        public int Insertar(ResenaDto r)
        {
            const string sql = @"
                INSERT INTO dbo.Resena (ActividadId, UsuarioId, Calificacion, Comentario, FechaCreacion)
                OUTPUT INSERTED.ResenaId
                VALUES (@ActividadId, @UsuarioId, @Calificacion, @Comentario, @Fecha);";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = r.ActividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = r.UsuarioId;
                cmd.Parameters.Add("@Calificacion", SqlDbType.TinyInt).Value = (byte)r.Calificacion;
                cmd.Parameters.Add("@Comentario", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(r.Comentario) ? DBNull.Value : (object)r.Comentario;
                cmd.Parameters.Add("@Fecha", SqlDbType.DateTime2).Value = r.Fecha;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Existe(int actividadId, int usuarioId)
        {
            const string sql = "SELECT COUNT(1) FROM dbo.Resena WHERE ActividadId = @ActividadId AND UsuarioId = @UsuarioId;";
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public List<ResenaDto> ObtenerPorActividad(int actividadId)
        {
            const string sql = @"
                SELECT r.ResenaId, r.ActividadId, r.UsuarioId, u.Nombre, r.Calificacion, r.Comentario, r.FechaCreacion
                FROM dbo.Resena r
                INNER JOIN dbo.Usuario u ON u.UsuarioId = r.UsuarioId
                WHERE r.ActividadId = @ActividadId
                ORDER BY r.FechaCreacion DESC;";
            var lista = new List<ResenaDto>();
            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@ActividadId", SqlDbType.Int).Value = actividadId;
                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ResenaDto
                        {
                            ResenaId = rd.GetInt32(0),
                            ActividadId = rd.GetInt32(1),
                            UsuarioId = rd.GetInt32(2),
                            UsuarioNombre = rd.GetString(3),
                            Calificacion = rd.GetByte(4),
                            Comentario = rd.IsDBNull(5) ? "" : rd.GetString(5),
                            Fecha = rd.GetDateTime(6)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
