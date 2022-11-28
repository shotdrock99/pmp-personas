using Microsoft.AspNetCore.Http;

namespace ModernizacionPersonas.DAL.Entities
{
    public class MapAseguradosViewModelArgs
    {
        public int CodigoGrupoAsegurados { get; set; }
        public IFormFile File { get; set; }
        public decimal ValorMinimoPerfil { get; set; }
        public decimal ValorMaximoPerfil { get; set; }
        public decimal ValorMinimoGrupoAsegurado { get; set; }
        public decimal ValorMaximoGrupoAsegurado { get; set; }
        public int EdadMinimaPerfil { get; set; }
        public int EdadMaximaPerfil { get; set; }
        public int EdadMinimaAmparoBasico { get; set; }
        public int EdadMaximaAmparoBasico { get; set; }
        public int NumeroSalarios { get; set; }
        public int TipoEstructura { get; set; }
    }

}
