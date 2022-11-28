using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System.Collections.Generic;

namespace ModernizacionPersonas.BLL.Entities
{
    public class UploadAseguradosResponse : ActionResponseBase
    {
        public List<Asegurado> Asegurados { get; set; }
        public int RegistrosProcesados { get; set; }
        public int RegistrosDuplicados { get; set; }
        public int TotalRegistros { get; set; }
        public long TotalAsegurados { get; set; }
        public int EdadPromedio { get; set; }
        public IEnumerable<UploadAseguradoError> Errores { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int TotalRegistrosValidos { get; internal set; }
    }
}
