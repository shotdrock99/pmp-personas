using ModernizacionPersonas.Entities;

namespace ModernizacionPersonas.DAL.Entities
{
    public class GetTomadorResponse : DbActionResponse
    {
        public Tomador Tomador { get; set; }
    }
}
