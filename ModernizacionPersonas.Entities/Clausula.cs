using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public partial class Clausula
    {
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoSeccion { get; set; }
        public string Nombre { get; set; }
        public List<SlipVariable> Variables { get; set; }
        public List<Asegurabilidad> Asegurabilidad { get; set; }
        public bool Activo { get; set; }
        [JsonProperty("descripcion")]
        public string DescripcionClausula { get; set; }

        public Clausula()
        {
            this.Variables = new List<SlipVariable>();
            this.Asegurabilidad = new List<Asegurabilidad>();
        }
    }
}

