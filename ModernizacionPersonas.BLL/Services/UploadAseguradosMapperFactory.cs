using ModernizacionPersonas.DAL.Entities;

namespace ModernizacionPersonas.BLL.Services
{
    public class UploadAseguradosMapperFactory
    {
        public IAseguradosMapper GetMapper(MapAseguradosViewModelArgs args, System.IO.Stream fileStream)
        {
            switch (args.File.ContentType)
            {
                //case "text/csv":
                case "application/vnd.ms-excel":
                    return new UploadAseguradosCSVMapper(args, fileStream);
                //case "application/vnd.ms-excel":
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    return new UploadAseguradosExcelMapper(args, fileStream);
                default:
                    return new UploadAseguradosCSVMapper(args, fileStream);
            }
        }
    }
}
