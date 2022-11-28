using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosParametrizacionEmailWriter
    {
        Task GuardarEmailParametrizacionAsync(EmailParametrizacion model);
        Task ActualizarEmailParametrizacionAsync(EmailParametrizacion model);
        Task EliminarEmailParametrizacionAsync(int codigoEmailParametrizacion, string usuario);

    }
}
