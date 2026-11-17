using System;
using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.Tests
{
    /// <summary>Repositorios simulados en memoria: permiten probar las reglas de negocio sin SQL Server.</summary>
    internal class ActividadRepoSimulado : IActividadRepository
    {
        public ActividadDto Existente;
        public bool Dependencias;
        public int Insertados, Actualizados, CambiosEstado, Eliminados;
        public FiltroActividades UltimoFiltro;

        public int Insertar(ActividadDto a) { Insertados++; return 1; }
        public int Actualizar(ActividadDto a) { Actualizados++; return 1; }
        public int CambiarEstado(int id, string estado) { CambiosEstado++; return 1; }
        public int Eliminar(int id) { Eliminados++; return 1; }
        public bool TieneDependencias(int id) { return Dependencias; }
        public ActividadDto ObtenerPorId(int id) { return Existente; }
        public List<ActividadDto> Buscar(FiltroActividades f) { UltimoFiltro = f; return new List<ActividadDto>(); }
    }

    internal class TipoRepoSimulado : ITipoActividadRepository
    {
        public List<TipoActividadDto> Listar() { return new List<TipoActividadDto>(); }
    }

    internal class InscripcionRepoSimulado : IInscripcionRepository
    {
        public ResultadoInscripcion Resultado = ResultadoInscripcion.Ok;
        public bool Inscrito;
        public int FilasCanceladas = 1;
        public int Canceladas;

        public ResultadoInscripcion Inscribir(int actividadId, int usuarioId, DateTime ahora) { return Resultado; }
        public int Cancelar(int actividadId, int usuarioId) { Canceladas++; return FilasCanceladas; }
        public bool EstaInscrito(int actividadId, int usuarioId) { return Inscrito; }
        public List<InscripcionDto> ObtenerPorUsuario(int usuarioId) { return new List<InscripcionDto>(); }
        public List<InscripcionDto> ObtenerPorActividad(int actividadId) { return new List<InscripcionDto>(); }
    }

    internal class ResenaRepoSimulado : IResenaRepository
    {
        public bool ExisteResena;
        public int Insertadas;

        public int Insertar(ResenaDto r) { Insertadas++; return 1; }
        public bool Existe(int actividadId, int usuarioId) { return ExisteResena; }
        public List<ResenaDto> ObtenerPorActividad(int actividadId) { return new List<ResenaDto>(); }
    }

    internal class UsuarioRepoSimulado : IUsuarioRepository
    {
        public CredencialUsuario Credencial;
        public CredencialUsuario ObtenerCredencialPorCorreo(string correo) { return Credencial; }
    }

    internal static class Datos
    {
        public static UsuarioDto Admin()
        {
            return new UsuarioDto { UsuarioId = 1, Nombre = "Admin", Correo = "admin@x.edu", Rol = Roles.Administrador };
        }

        public static UsuarioDto Estudiante()
        {
            return new UsuarioDto { UsuarioId = 2, Nombre = "Est", Correo = "est@x.edu", Rol = Roles.Estudiante };
        }

        public static ActividadDto ActividadValida()
        {
            return new ActividadDto
            {
                ActividadId = 10,
                Titulo = "Taller de prueba",
                Descripcion = "Descripcion",
                FechaInicio = DateTime.Today.AddDays(10).AddHours(8),
                FechaFin = DateTime.Today.AddDays(10).AddHours(12),
                Lugar = "Laboratorio 3",
                Cupos = 30,
                Estado = EstadosActividad.Publicada,
                TipoId = 1,
                Inscritos = 0
            };
        }
    }
}
