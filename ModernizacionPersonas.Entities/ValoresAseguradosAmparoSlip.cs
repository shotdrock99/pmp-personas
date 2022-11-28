using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class ValoresAseguradosAmparoSlip
    {
        public int CodigoTipoSumaAsegurada { get; set; }
        public int CountOpciones { get; set; }
        public string NombreAmparo { get; set; }
        public List<OpcionValoresAseguradosAmparoSlip> Opciones { get; set; }
        public ValoresAseguradosAmparoSlip()
        {
            Opciones = new List<OpcionValoresAseguradosAmparoSlip>();
        }
    }

    public partial class OpcionValoresAseguradosAmparoSlip
    {
        public decimal ValorAseguradoIndividual { get; set; }        
        public decimal ValorAsegurado { get; set; }
        public decimal tasaMensual { get; set; }
        public int TipoValor { get; set; }
        public decimal ValorDiario { get; set; }
        public decimal NumeroDias { get; set; }
        public bool TablaValoresDiarios { get; set; }
    }
}
