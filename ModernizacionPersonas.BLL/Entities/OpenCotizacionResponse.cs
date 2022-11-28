using ModernizacionPersonas.DAL.Entities;

namespace ModernizacionPersonas.BLL.Entities
{
    public class OpenCotizacionResponse : ActionResponseBase
    {
        public CotizacionViewModel Data { get; set; }
        public string OwnerUserName { get; set; }
        public bool Readonly { get; set; }
    }
}
