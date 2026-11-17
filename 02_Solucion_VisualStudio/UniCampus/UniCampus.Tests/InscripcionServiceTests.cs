using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniCampus.BLL;
using UniCampus.Entities;

namespace UniCampus.Tests
{
    [TestClass]
    public class InscripcionServiceTests
    {
        private InscripcionRepoSimulado _inscripciones;
        private ActividadRepoSimulado _actividades;
        private InscripcionService _servicio;

        [TestInitialize]
        public void Preparar()
        {
            _inscripciones = new InscripcionRepoSimulado();
            _actividades = new ActividadRepoSimulado { Existente = Datos.ActividadValida() };
            _servicio = new InscripcionService(_inscripciones, _actividades);
        }

        [TestMethod]
        public void Inscribir_Estudiante_Correcto()
        {
            Assert.IsTrue(_servicio.Inscribir(10, Datos.Estudiante()).Exito);
        }

        [TestMethod]
        public void Inscribir_Administrador_EsRechazado()
        {
            Assert.IsFalse(_servicio.Inscribir(10, Datos.Admin()).Exito);
        }

        [TestMethod]
        public void Inscribir_SinCupos_MensajeClaro()
        {
            _inscripciones.Resultado = ResultadoInscripcion.SinCupos;
            var r = _servicio.Inscribir(10, Datos.Estudiante());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual("No hay cupos disponibles para esta actividad.", r.Mensaje);
        }

        [TestMethod]
        public void Inscribir_Duplicada_MensajeClaro()
        {
            _inscripciones.Resultado = ResultadoInscripcion.Duplicada;
            var r = _servicio.Inscribir(10, Datos.Estudiante());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual("Ya está inscrito en esta actividad.", r.Mensaje);
        }

        [TestMethod]
        public void Inscribir_ActividadInexistente_EsRechazada()
        {
            _inscripciones.Resultado = ResultadoInscripcion.ActividadNoExiste;
            Assert.IsFalse(_servicio.Inscribir(999, Datos.Estudiante()).Exito);
        }

        [TestMethod]
        public void Inscribir_IdInvalido_EsRechazado()
        {
            Assert.IsFalse(_servicio.Inscribir(0, Datos.Estudiante()).Exito);
        }

        [TestMethod]
        public void PuedeInscribirse_CasosLimite()
        {
            var est = Datos.Estudiante();
            var a = Datos.ActividadValida();
            Assert.IsTrue(_servicio.PuedeInscribirse(a, est, false));
            Assert.IsFalse(_servicio.PuedeInscribirse(a, est, true), "ya inscrito");
            Assert.IsFalse(_servicio.PuedeInscribirse(a, Datos.Admin(), false), "administrador");

            a.Inscritos = a.Cupos;
            Assert.IsFalse(_servicio.PuedeInscribirse(a, est, false), "sin cupos");

            a = Datos.ActividadValida();
            a.Estado = EstadosActividad.Cancelada;
            Assert.IsFalse(_servicio.PuedeInscribirse(a, est, false), "cancelada");

            a = Datos.ActividadValida();
            a.FechaInicio = DateTime.Now.AddHours(-1);
            Assert.IsFalse(_servicio.PuedeInscribirse(a, est, false), "ya inició");
        }

        [TestMethod]
        public void Cancelar_ActividadYaIniciada_EsRechazada()
        {
            _actividades.Existente.FechaInicio = DateTime.Now.AddHours(-2);
            var r = _servicio.Cancelar(10, Datos.Estudiante());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _inscripciones.Canceladas);
        }

        [TestMethod]
        public void Cancelar_ActividadFutura_Cancela()
        {
            Assert.IsTrue(_servicio.Cancelar(10, Datos.Estudiante()).Exito);
            Assert.AreEqual(1, _inscripciones.Canceladas);
        }

        [TestMethod]
        public void Cancelar_SinInscripcionActiva_EsRechazada()
        {
            _inscripciones.FilasCanceladas = 0;
            Assert.IsFalse(_servicio.Cancelar(10, Datos.Estudiante()).Exito);
        }
    }
}
