using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class ExpedicionWeb
    {
        public ExpedicionWeb()
        {
        }

        public InformacionNegocio InformacionNegocio { get; set; }
        public Resumen Resumen { get; set; }
        public FichaTecnica FichaTecnica { get; set; }
        public Slip Slip { get; set; }
    }
}
