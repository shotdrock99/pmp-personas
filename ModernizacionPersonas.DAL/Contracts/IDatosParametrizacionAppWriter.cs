﻿using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public interface IDatosParametrizacionAppWriter
    {
        Task EditarValorAppAsync(int codigoVariable, string valorApp);
    }
}
