using System.Configuration;
using System.Data.SqlClient;

namespace UniCampus.DAL
{
    internal static class Conexion
    {
        private static string _cadena;

        public static string Cadena
        {
            get
            {
                if (_cadena == null)
                {
                    var s = ConfigurationManager.ConnectionStrings["UniCampusDb"];
                    if (s == null)
                        throw new ConfigurationErrorsException("Falta la cadena de conexión 'UniCampusDb' en Web.config.");
                    _cadena = s.ConnectionString;
                }
                return _cadena;
            }
        }

        public static SqlConnection Crear()
        {
            return new SqlConnection(Cadena);
        }
    }
}
