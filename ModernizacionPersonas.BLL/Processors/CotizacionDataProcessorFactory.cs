using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using System;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionDataProcessorFactory
    {
        private IResumenDataProcessor2 processor;

        public IResumenDataProcessor Resolve(int codigoTipoTasa1, int codigoTipoTasa2, int codigoTipoSumaAsegurada)
        {
            var codigoTipoTasa = codigoTipoTasa1 == 5 ? codigoTipoTasa2 : codigoTipoTasa2 == 5 ? codigoTipoTasa1 : codigoTipoTasa1;
            if (codigoTipoTasa.Equals(2) || codigoTipoTasa.Equals(4))
            {
                return new TasaPorEdadesValoresPrimasDataProcessor(codigoTipoTasa, codigoTipoSumaAsegurada);
            }
            else
            {
                return new ValoresPrimasDataProcessor(codigoTipoTasa, codigoTipoSumaAsegurada);
            }
        }

        public IResumenDataProcessor2 Resolve(Entities.CotizacionDataProcessorArgs args)
        {
            switch (args.TipoSumaAsegurada.CodigoTipoSumaAsegurada)
            {
                // Suma fija
                case 1:
                    this.processor = new SFCotizacionDataProcessor(args);
                    break;
                // Multiplo de sueldos
                case 2:
                    this.processor = new MSCotizacionDataProcessor(args);
                    break;
                // Suma variable por asegurado
                case 3:
                    this.processor = new SVPACotizacionDataProcessor(args);
                    break;
                // Suma fija y múltiplo de sueldos
                case 5:
                    this.processor = new SFMSCotizacionDataProcessor(args);
                    break;
                // Saldo Deudores-Ahorros-Aportes
                case 6:
                    this.processor = new SDAACotizacionDataProcessor(args);
                    break;
                // SMMLV
                case 10:
                    this.processor = new SMMLVCotizacionDataProcessor(args);
                    break;
                default:
                    this.processor = new DefaultResumenDataProcessor(args);
                    break;
            }

            return this.processor;
        }

        internal IResumenDataProcessor2 GetProcessor()
        {
            return this.processor;
        }
    }
}
