using System;
using System.Collections.Generic;
using UniCampus.Entities;

namespace UniCampus.DAL
{
    public interface IActividadRepository
    {
        int Insertar(ActividadDto actividad);
        int Actualizar(ActividadDto actividad);
        int CambiarEstado(int actividadId, string estado);
        int Eliminar(int actividadId);
        bool TieneDependencias(int actividadId);
        ActividadDto ObtenerPorId(int actividadId);
        List<ActividadDto> Buscar(FiltroActividades filtro);
    }

    public interface ITipoActividadRepository
    {
        List<TipoActividadDto> Listar();
    }

    public interface IUsuarioRepository
    {
        CredencialUsuario ObtenerCredencialPorCorreo(string correo);
    }

    public interface IInscripcionRepository
    {
        /// <summary>Inscribe en una transacción que verifica estado, fecha, duplicado y cupo.</summary>
        ResultadoInscripcion Inscribir(int actividadId, int usuarioId, DateTime ahora);
        int Cancelar(int actividadId, int usuarioId);
        bool EstaInscrito(int actividadId, int usuarioId);
        List<InscripcionDto> ObtenerPorUsuario(int usuarioId);
        List<InscripcionDto> ObtenerPorActividad(int actividadId);
    }

    public interface IResenaRepository
    {
        int Insertar(ResenaDto resena);
        bool Existe(int actividadId, int usuarioId);
        List<ResenaDto> ObtenerPorActividad(int actividadId);
    }

    public interface IReporteRepository
    {
        List<ResumenActividadDto> ObtenerResumen();
    }
}
