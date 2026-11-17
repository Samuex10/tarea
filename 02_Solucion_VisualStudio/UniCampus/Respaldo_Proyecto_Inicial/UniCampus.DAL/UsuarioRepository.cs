using System.Data;
using System.Data.SqlClient;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public CredencialUsuario ObtenerCredencialPorCorreo(string correo)
        {
            const string sql = @"
                SELECT UsuarioId, Nombre, Correo, Rol, PasswordHash, PasswordSalt, Activo
                FROM dbo.Usuario
                WHERE Correo = @Correo;";

            using (var cn = Conexion.Crear())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@Correo", SqlDbType.NVarChar, 150).Value = correo;
                cn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new CredencialUsuario
                    {
                        Usuario = new UsuarioDto
                        {
                            UsuarioId = r.GetInt32(0),
                            Nombre = r.GetString(1),
                            Correo = r.GetString(2),
                            Rol = r.GetString(3)
                        },
                        Hash = r.IsDBNull(4) ? null : (byte[])r.GetValue(4),
                        Salt = r.IsDBNull(5) ? null : (byte[])r.GetValue(5),
                        Activo = r.GetBoolean(6)
                    };
                }
            }
        }
    }
}
