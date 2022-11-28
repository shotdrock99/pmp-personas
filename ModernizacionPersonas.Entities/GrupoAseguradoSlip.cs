using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class GrupoAseguradoSlip
    {
        public int CodigoGrupoAsegurado { get; set; }
        public string Nombre { get; set; }
        public bool snTasaMensual { get; set; }
        public List<ValoresAseguradosAmparoSlip> ValoresAmparos { get; set; }
        public ValorMaximoSlip valorMaximo { get; set; }
        public List<decimal> Tasas { get; set; }
        public List<EdadAmparoSlip> Edades { get; set; }
        public GrupoAseguradoSlip()
        {
            ValoresAmparos = new List<ValoresAseguradosAmparoSlip>();
            Edades = new List<EdadAmparoSlip>();
            valorMaximo = new ValorMaximoSlip();
        }
    }
}

