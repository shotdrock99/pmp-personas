using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class Cotizacion
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public string NumeroCotizacion { get; set; }
        public CotizacionState Estado { get; set; }
        public string UsuarioNotificado { get; set; }
        public CotizacionCompleteness State { get; set; }

        public void MoveNextState()
        {
            State.NextState();
        }

        public void MovePreviousState()
        {
            State.PreviousState();
        }
    }
}
