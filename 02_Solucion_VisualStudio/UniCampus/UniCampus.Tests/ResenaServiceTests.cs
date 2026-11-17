using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniCampus.BLL;
using UniCampus.Entities;

namespace UniCampus.Tests
{
    [TestClass]
    public class ResenaServiceTests
    {
        private ResenaRepoSimulado _resenas;
        private InscripcionRepoSimulado _inscripciones;
        private ActividadRepoSimulado _actividades;
        private ResenaService _servicio;

        [TestInitialize]
        public void Preparar()
        {
            var iniciada = Datos.ActividadValida();
            iniciada.FechaInicio = DateTime.Now.AddDays(-3);
            iniciada.FechaFin = DateTime.Now.AddDays(-3).AddHours(3);

            _resenas = new ResenaRepoSimulado();
            _inscripciones = new InscripcionRepoSimulado { Inscrito = true };
            _actividades = new ActividadRepoSimulado { Existente = iniciada };
            _servicio = new ResenaService(_resenas, _actividades, _inscripciones);
        }

        [TestMethod]
        public void Agregar_Valida_Inserta()
        {
            var r = _servicio.Agregar(10, 5, "Excelente", Datos.Estudiante());
            Assert.IsTrue(r.Exito);
            Assert.AreEqual(1, _resenas.Insertadas);
        }

        [TestMethod]
        public void Agregar_CalificacionFueraDeRango_EsRechazada()
        {
            Assert.IsFalse(_servicio.Agregar(10, 0, "x", Datos.Estudiante()).Exito);
            Assert.IsFalse(_servicio.Agregar(10, 6, "x", Datos.Estudiante()).Exito);
            Assert.AreEqual(0, _resenas.Insertadas);
        }

        [TestMethod]
        public void Agregar_ComentarioMuyLargo_EsRechazado()
        {
            Assert.IsFalse(_servicio.Agregar(10, 4, new string('a', 1001), Datos.Estudiante()).Exito);
        }

        [TestMethod]
        public void Agregar_SinInscripcion_EsRechazada()
        {
            _inscripciones.Inscrito = false;
            Assert.IsFalse(_servicio.Agregar(10, 4, "x", Datos.Estudiante()).Exito);
            Assert.AreEqual(0, _resenas.Insertadas);
        }

        [TestMethod]
        public void Agregar_ActividadNoIniciada_EsRechazada()
        {
            _actividades.Existente = Datos.ActividadValida();
            Assert.IsFalse(_servicio.Agregar(10, 4, "x", Datos.Estudiante()).Exito);
        }

        [TestMethod]
        public void Agregar_ReseñaDuplicada_EsRechazada()
        {
            _resenas.ExisteResena = true;
            Assert.IsFalse(_servicio.Agregar(10, 4, "x", Datos.Estudiante()).Exito);
            Assert.AreEqual(0, _resenas.Insertadas);
        }

        [TestMethod]
        public void Agregar_ComoAdministrador_EsRechazada()
        {
            Assert.IsFalse(_servicio.Agregar(10, 4, "x", Datos.Admin()).Exito);
        }

        [TestMethod]
        public void PuedeResenar_CasosLimite()
        {
            var est = Datos.Estudiante();
            var iniciada = _actividades.Existente;
            Assert.IsTrue(_servicio.PuedeResenar(iniciada, est, true, false));
            Assert.IsFalse(_servicio.PuedeResenar(iniciada, est, false, false), "no inscrito");
            Assert.IsFalse(_servicio.PuedeResenar(iniciada, est, true, true), "ya reseñó");
            Assert.IsFalse(_servicio.PuedeResenar(Datos.ActividadValida(), est, true, false), "no ha iniciado");
        }
    }
}
