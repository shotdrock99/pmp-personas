using PersonasServiceReference;
using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class AmparoGrupoAsegurado
    {
        public int CodigoAmparoGrupoAsegurado { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public int CodigoAmparo { get; set; }
        public List<OpcionValorAsegurado> OpcionesValores { get; set; }
        public EdadAmparoGrupoAsegurado EdadesGrupo { get; set; }
        public int CodigoGrupoAmparo { get; set; }
        public Amparo AmparoInfo { get; set; }
        public Modalidad Modalidad {set; get;}

        public AmparoGrupoAsegurado()
        {
            this.OpcionesValores = new List<OpcionValorAsegurado>();
            this.EdadesGrupo = new EdadAmparoGrupoAsegurado();
            this.Modalidad = new Modalidad();
        }
    }
}
