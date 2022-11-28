using ModernizacionPersonas.DAL.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class UploadAseguradosExcelMapper : IAseguradosMapper
    {
        private Stream fileStream;

        public UploadAseguradosExcelMapper(MapAseguradosViewModelArgs args)
        {

        }

        public UploadAseguradosExcelMapper(MapAseguradosViewModelArgs args, Stream fileStream) : this(args)
        {
            this.fileStream = fileStream;
        }

        public IEnumerable<UploadAseguradoError> GetErrores()
        {
            throw new System.NotImplementedException();
        }

        public async Task<MapAseguradosResponse> MapAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}