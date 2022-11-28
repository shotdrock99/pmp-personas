using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosCausalReader
    {
        Task<IEnumerable<Causal>> GetCausales();
        Task<Causal> GetCausalId(int codigoCausal);
        Task<IEnumerable<Causal>> GetCausalesAceptacionAsync();
        Task<IEnumerable<Causal>> LeerCausalesRechazoAsync();
    }
}
