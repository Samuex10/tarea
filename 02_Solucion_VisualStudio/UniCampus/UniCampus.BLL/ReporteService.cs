using System.Collections.Generic;
using UniCampus.DAL;
using UniCampus.Entities;

namespace UniCampus.BLL
{
    public class ReporteService
    {
        private readonly IReporteRepository _reportes;

        public ReporteService() : this(new ReporteRepository()) { }

        internal ReporteService(IReporteRepository reportes)
        {
            _reportes = reportes;
        }

        /// <summary>Resumen por actividad (solo administradores).</summary>
        public List<ResumenActividadDto> Resumen(UsuarioDto usuario)
        {
            if (!ActividadService.EsAdmin(usuario)) return new List<ResumenActividadDto>();
            return _reportes.ObtenerResumen();
        }
    }
}
