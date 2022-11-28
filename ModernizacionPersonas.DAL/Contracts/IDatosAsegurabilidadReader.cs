using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosAsegurabilidadReader
    {
        Task<List<Asegurabilidad>> LeerAsegurabilidadAsync(int codigoCotizacion);        
    }
}
