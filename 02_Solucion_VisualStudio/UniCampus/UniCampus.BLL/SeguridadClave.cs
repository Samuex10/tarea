using System.Security.Cryptography;

namespace UniCampus.BLL
{
    /// <summary>Hash de contraseñas con PBKDF2 (SHA-256). Nunca se guardan contraseñas en texto plano.</summary>
    public static class SeguridadClave
    {
        private const int Iteraciones = 10000;
        private const int LongitudHash = 32;

        public static byte[] CalcularHash(string clave, byte[] salt)
        {
            using (var kdf = new Rfc2898DeriveBytes(clave, salt, Iteraciones, HashAlgorithmName.SHA256))
            {
                return kdf.GetBytes(LongitudHash);
            }
        }

        public static bool Verificar(string clave, byte[] salt, byte[] hashEsperado)
        {
            if (clave == null || salt == null || hashEsperado == null) return false;
            return SonIguales(CalcularHash(clave, salt), hashEsperado);
        }

        // Comparación en tiempo constante.
        private static bool SonIguales(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++) diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }
    }
}
