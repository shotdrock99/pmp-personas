using System.Collections.Generic;

namespace ModernizacionPersonas.Entities
{
    public class GrupoAsegurado
    {
        public int CodigoCotizacion { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public string NombreGrupoAsegurado { get; set; }
        public int CodigoTipoSuma { get; set; }
        public decimal ValorMinAsegurado { get; set; }
        public decimal ValorMaxAsegurado { get; set; }
        public int NumeroSalariosAsegurado { get; set; }
        public List<AmparoGrupoAsegurado> AmparosGrupo { get; set; }
        //public bool ConListaAsegurados { get; set; }
        public bool ConDistribucionAsegurados { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public int AseguradosOpcion1 { get; set; }
        public int AseguradosOpcion2 { get; set; }
        public int AseguradosOpcion3 { get; set; }
        public int EdadPromedioAsegurados { get; set; }
        public decimal PorcentajeAsegurados { get; set; }
        public int NumeroPotencialAsegurados { get; set; }
        public int TipoEstructura { get; set; }
        public IEnumerable<Rango> RangosGrupo { get; set; }
        public GrupoAsegurado()
        {
            this.AmparosGrupo = new List<AmparoGrupoAsegurado>();
            this.RangosGrupo = new List<Rango>();
        }
    }

    class GrupoAseguradoViewModel
    {
        public int CodigoCotizacion { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public string NombreGrupoAsegurado { get; set; }
        public int CodigoTipoSuma { get; set; }
        // public TipoSumaAsegurada TipoSumaAsegurada { get; set; }
        public decimal ValorMinAsegurado { get; set; }
        public decimal ValorMaxAsegurado { get; set; }
        public int NumeroSalariosAsegurado { get; set; }
        public bool ConListaAsegurados { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public decimal PorcentajeAsegurados { get; set; }
    }
}
