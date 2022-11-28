using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    internal class DefaultResumenDataProcessor : CotizacionDataProcessorBase, IResumenDataProcessor2
    {
        public DefaultResumenDataProcessor(Entities.CotizacionDataProcessorArgs args)
            : base(args.CodigoCotizacion, args.IBNR, args.FactorG)
        {
            throw new System.Exception($"No hay implementacion para el tipo de tasa código {args.CodigoTipoTasa}");
        }

        public decimal CalcularValorAseguradoTotal(GrupoAsegurado grupo, OpcionValorAsegurado opcion, bool conListaAsegurados)
        {
            throw new System.NotImplementedException();
        }

        public Task<GrupoAseguradoResumen> BuildGrupoAseguradoResumen(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            throw new System.NotImplementedException();
        }

        public Task<ValorAseguradoOpcionResumen> ProcessOpcionAmparoAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo, AmparoGrupoAsegurado amparo, OpcionValorAsegurado opcion)
        {
            throw new System.NotImplementedException();
        }

        public Task<PrimasGrupoAsegurado> CalcularPrimasGrupoAseguradoFichaTecnicaAsync(InformacionNegocio informacionNegocio, GrupoAsegurado grupo)
        {
            throw new System.NotImplementedException();
        }

        public Task<InformacionSiniestralidad> BuildSiniestralidadDataAsync(int codigoCotizacion, int codigoRamo, IEnumerable<GrupoAseguradoFichaTecnica> gruposAsegurados, bool tieneTasaSiniestralidad)
        {
            throw new System.NotImplementedException();
        }
    }
}