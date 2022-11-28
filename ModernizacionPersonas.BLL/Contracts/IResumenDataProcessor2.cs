using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Contracts
{
    public interface IResumenDataProcessor2
    {
        Task<GrupoAseguradoResumen> BuildGrupoAseguradoResumen(InformacionNegocio informacionNegocio, GrupoAsegurado grupo);
        decimal CalcularValorAseguradoTotal(GrupoAsegurado grupo, OpcionValorAsegurado opcion, bool conListaAsegurados);
        Task<PrimasGrupoAsegurado> CalcularPrimasGrupoAseguradoFichaTecnicaAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo);
        Task<IEnumerable<ProyeccionFinanciera>> CalcularProyeccionFinancieraGrupoAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo);
        Task<InformacionSiniestralidad> BuildSiniestralidadDataAsync(int codigoCotizacion, int codigoRamo, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados, bool tieneTasaSiniestralidad);
    }

    public class ResumenDataProcessorResult
    {
    }
}
