using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosSeccionSlipWriter
    {
        Task GuardarSeccionAsync(SeccionSlip model);
        Task ActualizarSeccionAsync(SeccionSlip model);
        Task EliminarSeccionAsync(int codigoSeccion, string usuarioLog);

    }
}
