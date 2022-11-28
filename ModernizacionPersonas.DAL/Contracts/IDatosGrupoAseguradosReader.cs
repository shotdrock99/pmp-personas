using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosGrupoAseguradosReader
    {
        Task<GrupoAsegurado> ConsultarGrupoAsegurado(int codigoGrupoAsegurado);
    }
}
