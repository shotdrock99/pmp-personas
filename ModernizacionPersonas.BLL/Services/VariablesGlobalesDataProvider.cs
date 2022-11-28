using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class VariablesGlobalesDataProvider
    {
        private readonly IDatosParametrizacionAppReader datosParametrizacionAppReader;
        private readonly IDatosParametrizacionAppWriter datosParametrizacionAppWriter;

        public VariablesGlobalesDataProvider()
        {
            this.datosParametrizacionAppReader = new DatosParametrizacionAppTableReader();
            this.datosParametrizacionAppWriter = new DatosParametrizacionAppTableWriter();
        }

        public async Task<IEnumerable<ParametrizacionApp>> GetVariablesGlobalesAsync()
        {
            try
            {
                var variablesGlobales = await this.datosParametrizacionAppReader.GetValoresVariablesApppAsync();
                return variablesGlobales;
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesGlobalesDataProvider :: GetVariablesGlobalesAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateVariableGlobalAsync(ParametrizacionApp variable)
        {
            try
            {
                await this.datosParametrizacionAppWriter.EditarValorAppAsync(variable.CodigoVariable, variable.ValorVariable);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("VariablesGlobalesDataProvider :: UpdateVariableGlobalAsync",ex);
            }
        }
    }
}
