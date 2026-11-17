using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniCampus.BLL;
using UniCampus.Entities;

namespace UniCampus.Tests
{
    [TestClass]
    public class UsuarioServiceTests
    {
        private static readonly byte[] Salt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        private static UsuarioService CrearServicio(bool activo = true)
        {
            var repo = new UsuarioRepoSimulado
            {
                Credencial = new CredencialUsuario
                {
                    Usuario = Datos.Estudiante(),
                    Salt = Salt,
                    Hash = SeguridadClave.CalcularHash("Clave123*", Salt),
                    Activo = activo
                }
            };
            return new UsuarioService(repo);
        }

        [TestMethod]
        public void Autenticar_ClaveCorrecta_Exito()
        {
            var r = CrearServicio().Autenticar("est@x.edu", "Clave123*");
            Assert.IsTrue(r.Exito);
            Assert.AreEqual(Roles.Estudiante, r.Usuario.Rol);
        }

        [TestMethod]
        public void Autenticar_ClaveIncorrecta_Falla()
        {
            Assert.IsFalse(CrearServicio().Autenticar("est@x.edu", "otra").Exito);
        }

        [TestMethod]
        public void Autenticar_UsuarioInactivo_Falla()
        {
            Assert.IsFalse(CrearServicio(false).Autenticar("est@x.edu", "Clave123*").Exito);
        }

        [TestMethod]
        public void Autenticar_UsuarioInexistente_MensajeGenerico()
        {
            var servicio = new UsuarioService(new UsuarioRepoSimulado { Credencial = null });
            var r = servicio.Autenticar("nadie@x.edu", "Clave123*");
            Assert.IsFalse(r.Exito);
            Assert.AreEqual("Correo o contraseña incorrectos.", r.Mensaje);
        }

        [TestMethod]
        public void Autenticar_CamposVacios_Falla()
        {
            Assert.IsFalse(CrearServicio().Autenticar("", "").Exito);
        }

        [TestMethod]
        public void SeguridadClave_HashDistintoSegunSalt()
        {
            var otraSal = new byte[16];
            CollectionAssert.AreNotEqual(
                SeguridadClave.CalcularHash("abc", Salt),
                SeguridadClave.CalcularHash("abc", otraSal));
        }
    }
}
