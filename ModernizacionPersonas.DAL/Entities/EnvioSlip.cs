using System;

namespace ModernizacionPersonas.DAL.Entities
{
    public class EnvioSlip
    {
        public int CodigoEnvio { set; get; }
        public int CodigoCotizacion { set; get; }
        public string Destinatarios { set; get; }
        public string DestinatariosOcultos { set; get; }
        public DateTime FechaEnvio { set; get; }
        public string Texto { set; get; }
    }
}
