using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class EdadAmparoGrupoAsegurado
    {
        public int CodigoEdadPermanencia { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public int CodigoAmparo { get; set; }
        public int EdadMinAsegurado { get; set; }
        public int EdadMaxAsegurado { get; set; }
        public int edadMaxPermanencia { get; set; }
        public int DiasCarencia { get; set; }
    }
}
