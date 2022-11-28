using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public partial class ExpedicionArgs
    {
        public string[] To { get; set; }
        public string Observaciones { get; set; }
    }

    public partial class EdadesGrupos
    {
        public int EdadMinima { get; set; }
        public int EdadMaxima { get; set; }
        public int EdadMaximaPermanencia { get; set; }
    }
}
