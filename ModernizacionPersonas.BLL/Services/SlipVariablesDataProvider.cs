using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModernizacionPersonas.BLL.Services
{
    public class SlipVariablesDataProvider
    {
        public List<VariableValorSlip> variables { get; }

        public SlipVariablesDataProvider(IDatosSlipReader slipReader, int codigoCotizacion, int codigoRamo, int sector, int subramo)
        {
            this.variables = slipReader.LeerValoresSlipAsync(codigoCotizacion, codigoRamo, sector, subramo).Result.ToList();
        }

        public string ObtenerVariablePorCodigo(int codigoVariable)
        {
            var variable = this.variables.Where(x => x.CodigoVariable == codigoVariable).FirstOrDefault();
            if (variable != null)
            {
                return variable.ValorVariable;
            }

            return "SIN DATO";
        }

        public string ObtenerVariablePorNombre(NombreSlipVariable nombreVariable)
        {
            var codigoVariable = (int)nombreVariable;
            var variable = this.variables.Where(x => x.CodigoVariable == codigoVariable).FirstOrDefault();
            if (variable != null)
            {
                return variable.ValorVariable;
            }

            return string.Empty;
        }
    }
}
