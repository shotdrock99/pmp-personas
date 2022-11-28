using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common.Services;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class DatosIntermediariosProvider
    {
        private readonly IDatosIntermediarioWriter intermediarioWriter;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly SARLAFTValidatorService SARLAFTService;

        public DatosIntermediariosProvider()
        {
            this.intermediarioWriter = new DatosIntermediarioTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.SARLAFTService = new SARLAFTValidatorService();
        }

        public async Task<CreateActionResponse> InsertarDatosIntermediarioAsync(int codigoCotizacion, int version, Intermediario model)
        {
            var isValid = await this.SARLAFTService.ValidateAsync(model.CodigoTipoDocumento, model.NumeroDocumento, model.PrimerNombre, model.SegundoNombre, model.PrimerApellido, model.SegundoApellido);
            if (!isValid)
            {
                return new CreateActionResponse
                {
                    Status = ResponseStatus.Invalid,
                    ErrorCode = "2001",
                    ErrorMessage = "Esta operación no se puede realizar. Favor comunicarse con la Gerencia Oficial de Cumplimiento"
                };
            }

            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            // Inserta Intermediario
            var codigoIntermediario = await intermediarioWriter.CreateIntermediarioAsync(codigoCotizacion, model);
            // Actualizar el estado de la cotizacion
            if (informacionNegocio.CotizacionState < CotizacionState.OnIntermediarios)
            {
                await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnIntermediarios);
            }

            return new CreateActionResponse
            {
                CodigoCotizacion = codigoCotizacion,
                Codigo = codigoIntermediario
            };
        }

        public async Task ActualizarDatosIntermediarioAsync(int codigoCotizacion, int version, Intermediario model)
        {
            await this.intermediarioWriter.UpdateIntermediarioAsync(codigoCotizacion, model);
        }

        public async Task EliminarIntermediarioAsync(int codigoIntermediario)
        {
            await this.intermediarioWriter.DeleteIntermediarioAsync(codigoIntermediario);
        }
    }
}
