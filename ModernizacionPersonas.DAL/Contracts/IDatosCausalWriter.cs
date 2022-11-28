using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosCausalWriter
    {
        Task GuardarCausalAsync(Causal causal);
        Task ActualizarCausalAsync(Causal causal);
        Task EliminarCausalAsync(int codigoCausal, string usuario);
    }
}
