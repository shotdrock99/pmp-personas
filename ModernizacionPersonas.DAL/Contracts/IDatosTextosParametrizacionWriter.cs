using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTextosParametrizacionWriter
    {
        Task <int> CreateTextoParametrizacionAsync(TextoSlip model);
        Task UpdateTextoParametrizacionAsync(TextoSlip model);
        Task DeleteTextoParametrizacionAsync(int codigoTextoParametrizacion, string usuario);

    }
}
