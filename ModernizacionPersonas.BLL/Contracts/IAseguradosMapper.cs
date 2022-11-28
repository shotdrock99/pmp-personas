using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public interface IAseguradosMapper
    {
        Task<MapAseguradosResponse> MapAsync();
        IEnumerable<UploadAseguradoError> GetErrores();        
    }

    public class MapAseguradosResponse
    {
        public int TotalRegistros { get; set; }
        public IEnumerable<Asegurado> Asegurados { get; set; }
        public int TotalRegistrosValidos { get; internal set; }
    }
}
