using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniCampus.BLL;
using UniCampus.Entities;

namespace UniCampus.Tests
{
    [TestClass]
    public class ActividadServiceTests
    {
        private ActividadRepoSimulado _repo;
        private ActividadService _servicio;

        [TestInitialize]
        public void Preparar()
        {
            _repo = new ActividadRepoSimulado();
            _servicio = new ActividadService(_repo, new TipoRepoSimulado());
        }

        [TestMethod]
        public void Validar_TituloVacio_DevuelveError()
        {
            var a = Datos.ActividadValida();
            a.Titulo = "   ";
            CollectionAssert.Contains(_servicio.Validar(a), "El título es obligatorio.");
        }

        [TestMethod]
        public void Validar_CupoCeroONegativo_DevuelveError()
        {
            var a = Datos.ActividadValida();
            a.Cupos = 0;
            CollectionAssert.Contains(_servicio.Validar(a), "El cupo debe ser un número mayor que cero.");
            a.Cupos = -5;
            CollectionAssert.Contains(_servicio.Validar(a), "El cupo debe ser un número mayor que cero.");
        }

        [TestMethod]
        public void Validar_FechaFinAnterior_DevuelveError()
        {
            var a = Datos.ActividadValida();
            a.FechaFin = a.FechaInicio.AddHours(-1);
            CollectionAssert.Contains(_servicio.Validar(a), "La fecha de fin no puede ser anterior a la de inicio.");
        }

        [TestMethod]
        public void Validar_SinTipo_DevuelveError()
        {
            var a = Datos.ActividadValida();
            a.TipoId = null;
            CollectionAssert.Contains(_servicio.Validar(a), "Seleccione el tipo de actividad.");
        }

        [TestMethod]
        public void Validar_ActividadCorrecta_NoTieneErrores()
        {
            Assert.AreEqual(0, _servicio.Validar(Datos.ActividadValida()).Count);
        }

        [TestMethod]
        public void Registrar_ComoEstudiante_EsRechazado()
        {
            var r = _servicio.Registrar(Datos.ActividadValida(), Datos.Estudiante());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _repo.Insertados);
        }

        [TestMethod]
        public void Registrar_Valida_ComoAdmin_Inserta()
        {
            var r = _servicio.Registrar(Datos.ActividadValida(), Datos.Admin());
            Assert.IsTrue(r.Exito);
            Assert.AreEqual(1, _repo.Insertados);
        }

        [TestMethod]
        public void Registrar_Invalida_NoInserta()
        {
            var a = Datos.ActividadValida();
            a.Cupos = 0;
            var r = _servicio.Registrar(a, Datos.Admin());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _repo.Insertados);
        }

        [TestMethod]
        public void Registrar_FechaInicioPasada_EsRechazada()
        {
            var a = Datos.ActividadValida();
            a.FechaInicio = DateTime.Today.AddDays(-2);
            a.FechaFin = DateTime.Today.AddDays(-2).AddHours(2);
            var r = _servicio.Registrar(a, Datos.Admin());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _repo.Insertados);
        }

        [TestMethod]
        public void Actualizar_CupoMenorQueInscritos_EsRechazado()
        {
            var existente = Datos.ActividadValida();
            existente.Inscritos = 20;
            _repo.Existente = existente;
            var cambio = Datos.ActividadValida();
            cambio.Cupos = 10;
            var r = _servicio.Actualizar(cambio, Datos.Admin());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _repo.Actualizados);
        }

        [TestMethod]
        public void Actualizar_Valida_Actualiza()
        {
            _repo.Existente = Datos.ActividadValida();
            var r = _servicio.Actualizar(Datos.ActividadValida(), Datos.Admin());
            Assert.IsTrue(r.Exito);
            Assert.AreEqual(1, _repo.Actualizados);
        }

        [TestMethod]
        public void Actualizar_ActividadInexistente_EsRechazada()
        {
            _repo.Existente = null;
            Assert.IsFalse(_servicio.Actualizar(Datos.ActividadValida(), Datos.Admin()).Exito);
        }

        [TestMethod]
        public void Eliminar_ConDependencias_EsRechazado()
        {
            _repo.Existente = Datos.ActividadValida();
            _repo.Dependencias = true;
            var r = _servicio.Eliminar(10, Datos.Admin());
            Assert.IsFalse(r.Exito);
            Assert.AreEqual(0, _repo.Eliminados);
        }

        [TestMethod]
        public void Eliminar_SinDependencias_Elimina()
        {
            _repo.Existente = Datos.ActividadValida();
            Assert.IsTrue(_servicio.Eliminar(10, Datos.Admin()).Exito);
            Assert.AreEqual(1, _repo.Eliminados);
        }

        [TestMethod]
        public void Desactivar_ComoEstudiante_EsRechazado()
        {
            _repo.Existente = Datos.ActividadValida();
            Assert.IsFalse(_servicio.Desactivar(10, Datos.Estudiante()).Exito);
            Assert.AreEqual(0, _repo.CambiosEstado);
        }

        [TestMethod]
        public void Buscar_ComoEstudiante_RestringeEstadosVisibles()
        {
            _servicio.Buscar(new FiltroActividades(), Datos.Estudiante());
            Assert.IsTrue(_repo.UltimoFiltro.SoloVisiblesParaEstudiante);
        }

        [TestMethod]
        public void Buscar_ComoAdmin_VeTodosLosEstados()
        {
            _servicio.Buscar(new FiltroActividades(), Datos.Admin());
            Assert.IsFalse(_repo.UltimoFiltro.SoloVisiblesParaEstudiante);
        }

        [TestMethod]
        public void ObtenerDetalle_EstudianteNoVeBorrador()
        {
            var a = Datos.ActividadValida();
            a.Estado = EstadosActividad.Borrador;
            _repo.Existente = a;
            Assert.IsNull(_servicio.ObtenerDetalle(10, Datos.Estudiante()));
            Assert.IsNotNull(_servicio.ObtenerDetalle(10, Datos.Admin()));
        }
    }
}
