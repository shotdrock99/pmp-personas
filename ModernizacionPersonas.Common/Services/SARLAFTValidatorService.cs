using ParametrizacionServiceReference;
using SARLAFTServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Common.Services
{
    public class SARLAFTValidatorService
    {
        SARLAFTServiceReference.ServiceClient clientService;
        private bool enableValidationListaClinton = true;
        private bool enableValidationCSNU = false;

        public SARLAFTValidatorService()
        {
            clientService = ServiceConnectionFactory.GetSARLAFTServiceClient();
        }

        private async Task<ValidateListaClintonResponse> ValidateListaClinton(int tipoDocumento, string numeroDocumento)
        {
            var result = await this.clientService.ConsultarListaClintonAsync(tipoDocumento, numeroDocumento);
            return new ValidateListaClintonResponse
            {
                Data = result.ToList(),
                IsValid = result.Count() == 0
            };
        }

        private async Task<ValidateListaCSNUResponse> ValidateListaCSNUAsync(string apellido1, string apellido2, string nombre1, string nombre2)
        {
            var result = await this.clientService.ValidarListaCSNUAsync(apellido1, apellido2, nombre1, nombre2);
            return new ValidateListaCSNUResponse
            {
                Data = result,
                IsValid = string.Equals(result.StrVeto, "OK")
            };
        }

        public async Task<bool> ValidateAsync(int codigoTipoDocumento, string numeroDocumento, string primerNombre, string segundoNombre, string primerApellido, string segundoApellido)
        {
            var resultValidationListaClinton = true;
            var resultValidationCSNU = true;
            if (this.enableValidationListaClinton)
            {
                var validation1 = await this.ValidateListaClinton(codigoTipoDocumento, numeroDocumento);
                resultValidationListaClinton = validation1.IsValid;
            }

            if (resultValidationListaClinton && this.enableValidationCSNU)
            {
                if (primerNombre != null || primerApellido != null)
                {
                    var validation2 = await this.ValidateListaCSNUAsync(primerApellido, segundoApellido, primerNombre, segundoNombre);
                    resultValidationCSNU = validation2.IsValid;
                }
            }

            return resultValidationListaClinton && resultValidationCSNU;
        }

        public object Validate(object codigoTipoDocumento)
        {
            throw new NotImplementedException();
        }
    }

    public class ValidateListaClintonResponse
    {
        public List<ListaClinton> Data { get; set; }
        public bool IsValid { get; set; }
    }

    public class ValidateListaCSNUResponse
    {
        public ListaCsnu Data { get; set; }
        public bool IsValid { get; set; }
    }
}
