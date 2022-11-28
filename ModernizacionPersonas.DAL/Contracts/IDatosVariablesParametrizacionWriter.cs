using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosVariablesParametrizacionWriter
    {
        Task GuardarVariableAsync(VariableSlipParametrizacion model);
        Task ActualizarVariableAsync(VariableSlipParametrizacion model);
        Task DesactivarVariableAsync(int codigoVariable, string Usuario);
        Task EliminarVariableAsync(int codigoVariable, string Usuario);
    }
}
