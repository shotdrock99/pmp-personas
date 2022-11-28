using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModernizacionPersonas.DAL.Entities;

namespace ModernizacionPersonas.Api.Entities
{
    public class UploadAseguradosResponseViewModel
    {
        public long TotalAsegurados { get; set; }
        public int TotalRegistros { get; internal set; }
        public int RegistrosTotales { get; internal set; }
        public int RegistrosProcesados { get; internal set; }
        public int RegistrosDuplicados { get; internal set; }
        public int EdadPromedio { get; internal set; }
        public decimal ValorAsegurado { get; internal set; }
        public int PorcentajeAsegurados { get; internal set; }
        public int RegistrosFallidos { get; internal set; }
        public bool WithErrors { get; internal set; }
        public List<UploadAseguradoError> ErrorSummary { get; internal set; }
    }
}
