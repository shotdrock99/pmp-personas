using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosOpcionValorAseguradoWriter
    {
        Task<int> CrearOpcionValorAseguradoAsync(OpcionValorAsegurado model);
        Task UpdateOpcionValorAseguradoAsync(int codigoOpcionValorAsegurado, OpcionValorAsegurado model);
        Task UpdateTasaRiesgoAmparoAsync(int codigoOpcionValorAsegurado, decimal tasaRiesgo);
        Task UpdateTasasPrimasAmparoAsync(int codigoOpcionValorAsegurado, decimal tasaRiesgo, decimal tasaComercial, decimal prima);
        Task UpdatePonderacionAmparoAsync(int codigoOpcionValorAsegurado, int indiceOpcion, decimal ponderacion);
        Task DeleteOpcionValorAseguradoAsync(int codigoOpcionValorAsegurado);
    }
}