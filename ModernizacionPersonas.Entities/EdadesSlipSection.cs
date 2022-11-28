using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class EdadesSlipSection
    {
        public string Seccion { get; set; }
        public EdadAmparoGrupoAsegurado edades { get; set; }
        public EdadesSlipSection()
        {
            Seccion = "INFORMACION GENERAL DEL RAMO";
            edades = new EdadAmparoGrupoAsegurado();
        }
    }

}
