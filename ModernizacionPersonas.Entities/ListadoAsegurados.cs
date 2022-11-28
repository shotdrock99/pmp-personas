using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class ListadoAsegurados
    {
        public int CodigoGrupoAsegurado { get; set; }
        public ICollection<Asegurado> asegurados { get; set; }

       public ListadoAsegurados()
       {
           this.asegurados = new List<Asegurado>();
       }
    }    
}
