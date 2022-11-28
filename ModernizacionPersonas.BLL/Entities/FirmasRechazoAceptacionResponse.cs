using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class FirmasRechazoAceptacionResponse
    {
        public TomadorCotizacion Tomador { get; set; }
        public IEnumerable<DirectorCotizacion> Firmas { get; set; }
        public bool OcultarDirector { get; set; }
    }
}
