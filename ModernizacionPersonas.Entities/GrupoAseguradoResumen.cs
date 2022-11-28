using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class GrupoAseguradoResumen
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public PersonasServiceReference.TipoSumaAsegurada TipoSumaAsegurada { get; set; }
        public bool ConListaAsegurados { get; set; }
        public int NumeroAsegurados { get; set; }
        public decimal EdadPromedio { get; set; }
        public bool ConDistribucionAsegurados { get; set; }
        public int AseguradosOpcion1 { get; set; }
        public int AseguradosOpcion2 { get; set; }
        public int AseguradosOpcion3 { get; set; }
        public IEnumerable<PersonasServiceReference.Amparo> Amparos { get; set; }
        public int NumeroOpciones { get; set; }
        public IEnumerable<ValorAseguradoOpcionResumen> Opciones { get; set; }
    }
}
