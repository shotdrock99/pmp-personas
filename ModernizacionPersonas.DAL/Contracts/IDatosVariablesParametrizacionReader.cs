using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosVariablesParametrizacionReader
    {
        Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesAsync();
        Task<VariableSlipParametrizacion> GetVariableAsync(int codigoVariable);
        Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesTextoAsync(int codigoTexto);
        Task<IEnumerable<VariableSlipParametrizacion>> GetUnusedVariablesAsync();

    }
}
