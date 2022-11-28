using ModernizacionPersonas.Entities;
using System.Collections.Generic;

namespace ModernizacionPersonas.DAL.Entities
{
    public class InsertarAseguradosArgs
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public int CodigoSucursal { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubRamo { get; set; }
        public int CodigoSector { get; set; }
        public List<Asegurado> Asegurados { get; set; }
        public int CodigoGrupoAsegurados { get; set; }
        public int CodigoTipoSumaAsegurada { get; set; }
        public int CodigoTipoTasa { get; set; }
        public bool ConListaAsegurados { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public int EdadPromedioAsegurados { get; set; }
        public IEnumerable<Rango> Rangos { get; set; }
        public int CodigoPerfilValor { get; set; }
        public int CodigoPerfilEdad { get; set; }
        public int TipoEstructura { get; set; }

        public InsertarAseguradosArgs()
        {
            this.Asegurados = new List<Asegurado>();
            this.Rangos = new List<Rango>();
        }
    }
}
