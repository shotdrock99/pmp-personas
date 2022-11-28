using PersonasServiceReference;

namespace ModernizacionPersonas.BLL.Entities
{
    public class GrupoAseguradoViewModel
    {
        public int CodigoCotizacion { get; set; }
        public int CodigoGrupoAsegurado { get; set; }
        public string NombreGrupoAsegurado { get; set; }
        public int CodigoTipoSuma { get; set; }
        public TipoSumaAsegurada TipoSumaAsegurada { get; set; }
        public decimal ValorMinAsegurado { get; set; }
        public decimal ValorMaxAsegurado { get; set; }
        public int NumeroSalariosAsegurado { get; set; }
        public bool ConListaAsegurados { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public decimal PorcentajeAsegurados { get; set; }
        public bool Configured { get; set; }
    }
}
