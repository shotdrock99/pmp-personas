using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosSlipReader
    {
        Task<IEnumerable<VariableValorSlip>> LeerValoresSlipAsync(int codigoCotizacion, int codigoRamo, int codigoSector, int codigoSubramo);
        Task<GetTextosSeccionSlipResponse> LeerTextosSlipAsync(int codigoCotizacion, int codigoRamo, int codigoSector, int codigoSubramo);
        Task<AmparoSlip> LeerTextoAmparoAsync(int codigoCotizacion, int codigoAmparo, int codigoRamo, int codigoSector, int codigoSubramo);
        Task<Clausula> LeerTextoClausulaAsync(int codigoCotizacion, int codigoSeccion, int codigoRamo, int codigoSector, int codigoSubramo);
    }
}
