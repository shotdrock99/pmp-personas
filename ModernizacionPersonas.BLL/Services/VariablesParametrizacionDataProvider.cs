using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class VariablesParametrizacionDataProvider
    {
        private readonly IDatosVariablesParametrizacionReader datosVariablesParametrizacionReader;
        private readonly IDatosVariablesParametrizacionWriter datosVariablesParametrizacionWriter;

        public VariablesParametrizacionDataProvider()
        {
            this.datosVariablesParametrizacionReader = new DatosVariablesParametrizacionTableReader();
            this.datosVariablesParametrizacionWriter = new DatosVariablesParametrizacionTableWriter();
        }

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesSlipAsync()
        {
            try
            {
                var variablesSlip = await this.datosVariablesParametrizacionReader.GetVariablesAsync();
                return variablesSlip;
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesParametrizacionDataProvider :: GetVariablesParametrizacionSlipAsync", ex);
            }
        }

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesSlipByCodigoTextoAsync(int codigoTexto)
        {
            try
            {
                var variablesSlip = await this.datosVariablesParametrizacionReader.GetVariablesTextoAsync(codigoTexto);
                return variablesSlip;
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesParametrizacionDataProvider :: GetVariablesParametrizacionSlipAsync", ex);
            }
        }

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetUnusedVariablesSlipAsync()
        {
            try
            {
                var variablesSlip = await this.datosVariablesParametrizacionReader.GetUnusedVariablesAsync();
                return variablesSlip;
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesParametrizacionDataProvider :: GetVariablesParametrizacionSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateVariableSlipAsync(VariableSlipParametrizacion variableSlip)
        {
            try
            {
                await this.datosVariablesParametrizacionWriter.ActualizarVariableAsync(variableSlip);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesParametrizacionDataProvider :: UpdateVariableSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> CreateVariableSlipAsync(VariableSlipParametrizacion variableSlip)
        {
            try
            {
                await this.datosVariablesParametrizacionWriter.GuardarVariableAsync(variableSlip);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesParametrizacionDataProvider :: CreateVariableSlipAsync", ex);
            }
        }
    }
}
