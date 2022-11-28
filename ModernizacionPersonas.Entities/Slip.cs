using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Entities
{
    public class Slip
    {
        public int CodigoRamo { get; set; }

        public int CodigoSubramo { get; set; }
        public PageHeaderInfo InfoEncabezado { get; set; }
        public string ImagenProductoUri { get; set; }
        public string Descripcion { get; set; }
        public string Ciudad { get; set; }
        public DateTime Fecha { get; set; }
        public TomadorSlip Tomador { get; set; }
        public TomadorSlip TomadorIntermediario { get; set; }
        public DirigidoSlip DirigidoSlip { get; set; }
        public string Asunto { get; set; }
        public string TipoPoliza { get; set; }
        public int DiasVigencia { get; set; }
        public string DiasVigenciaWords { get; set; }
        public string NombreSucursal { get; set; }
        public VigenciaSlip Vigencia { get; set; }
        public InfoGeneralTextosSeccionSlip InfoGeneral { get; set; }
        public AmparosTextosSeccionSlip Amparos { get; set; }
        public GruposAseguradosSlip GruposAsegurados { get; set; }
        public EdadesSlipSection Edades { get; set; }
        public ClausulasTextosSeccionSlip Clausulas { get; set; }
        public CondicionesTextosSeccionSlip Condiciones {get; set;}
        public DisposicionesTextosSeccionSlip Disposiciones { get; set; }
        public IEnumerable<string> InformacionEnvio { get; set; }
    }

    public class VigenciaSlip
    {
        public DateTime? Desde { get; set; }

        public DateTime? Hasta { get; set; }
    }

    public class PageHeaderInfo
    {
        public string HeaderImageUri { get; set; }
        public string FooterImageUri { get; set; }
        public string Imagen1Uri { get; set; }
        public string Imagen2Uri { get; set; }
    }
}
