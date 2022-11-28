using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTextosParametrizacionReader
    {
        Task<IEnumerable<TextoSlip>> LeerTextosParametrizacionAsync();
        Task<TextoSlip> LeerTextoParametrizacionAsync(int codigoTextoParametrizacion);
        Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionAmparoAsync(int codigoAmparo);
        Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionRamoAsync(int codigoRamo, int codigoSubramo);
        Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionSeccionAsync(int codigoSeccion);
        Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionSectorAsync(int codigoSector);
    }
}
