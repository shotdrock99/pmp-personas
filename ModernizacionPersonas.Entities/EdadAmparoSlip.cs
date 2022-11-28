using Newtonsoft.Json;

namespace ModernizacionPersonas.Entities
{
    public class EdadAmparoSlip
    {
        public string NombreAmparo { get; set; }
        public int EdadMinimaIngreso { get; set; }
        public int EdadMaximaIngreso { get; set; }
        public int EdadMaximaPermanencia { get; set; }
    }
}
