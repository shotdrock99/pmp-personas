using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosParametrizacionAppReader
    {
        Task<IEnumerable<ParametrizacionApp>> GetValoresVariablesApppAsync();
        Task<IEnumerable<ParametrizacionApp>> GetValorVariableAsync(int codigoParametriacion);
    }
}
