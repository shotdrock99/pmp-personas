using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosTextosSlipWriter
    {
        Task CreateTextoAsync(TextoSlip model);
        Task UpdateTextoAsync(TextoSlip model);
        Task DeleteTextoAsync(int codigoTexto);
    }
}
