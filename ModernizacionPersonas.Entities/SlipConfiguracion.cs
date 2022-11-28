using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    public class SlipConfiguracion
    {
        [JsonProperty("idCotizacion")]
        public int CodigoCotizacion { get; set; }
        public string NumeroCotizacion { get; set; }
        public int CodigoDepartamento { get; set; }
        public int CodigoCiudad { get; set; }
        public TomadorSlip Tomador { get; set; }
        public string Actividad { get; set; }
        public int DiasValidezCotizacion { get; set; }
        public string Condiciones { get; set; }
        public List<AmparoSlip> Amparos { get; set; }
        [JsonProperty("grupos")]
        public List<GruposAseguradosSlip> GruposAsegurados { get; set; }
        [JsonProperty("gruposList")]
        public List<GrupoAseguradoSlipList> GruposAseguradosList { get; set; }
        public List<Asegurabilidad> Asegurabilidad { get; set; }
        public List<Clausula> Clausulas { get; set; }
        public bool SnCambioClausulas { get; set; }

        public SlipConfiguracion()
        {
            this.Amparos = new List<AmparoSlip>();
            this.GruposAseguradosList = new List<GrupoAseguradoSlipList>();
            this.Clausulas = new List<Clausula>();
            this.Asegurabilidad = new List<Asegurabilidad>();
        }
    }

    public class GrupoAseguradoSlipList
    {
        public int CodigoGrupoAsegurado { get; set; }
        public string Nombre { get; set; }
        public List<AmparoSlip> Amparos { get; set; }
    }

    public partial class InfoGeneralTextosSeccionSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }

        [JsonProperty("secciones")]
        public List<TextosSeccionSlip> InfoGeneral { get; set; }
        public InfoGeneralTextosSeccionSlip()
        {
            Seccion = "INFORMACION GENERAL DEL RAMO";
            InfoGeneral = new List<TextosSeccionSlip>();
        }
    }

    public partial class AmparosTextosSeccionSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        public List<TextosSeccionSlip> Amparos { get; set; }
        public AmparosTextosSeccionSlip()
        {
            Seccion = "AMPAROS";
            Amparos = new List<TextosSeccionSlip>();
        }
    }

    public partial class GruposAseguradosSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        [JsonProperty("gruposAsegurados")]
        public List<GrupoAseguradoSlip> GruposAsegurados { get; set; }
        public GruposAseguradosSlip()
        {
            Seccion = "GRUPOS ASEGURADOS";
            GruposAsegurados = new List<GrupoAseguradoSlip>();
        }
    }

    public partial class ValorMaximoSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        [JsonProperty("secciones")]
        public string Texto { get; set; }
        public ValorMaximoSlip()
        {
            Seccion = "MAXIMO VALOR ASEGURADO INDIVIDUAL";
            Texto = "El valor asegurado por persona será el saldo insoluto de la deuda, sin superar en ningún caso la suma de ($var_suma_aseg_maxima), acumulada en uno o varios créditos.";
        }
    }

    public partial class ClausulasTextosSeccionSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        [JsonProperty("secciones")]
        public List<TextosSeccionSlip> Clausulas { get; set; }
        public IEnumerable<Asegurabilidad> Asegurabilidad { get; set; }
        public ClausulasTextosSeccionSlip()
        {
            Seccion = "CLAUSULAS ADICIONALES";
            Clausulas = new List<TextosSeccionSlip>();
        }
    }

    public partial class CondicionesTextosSeccionSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        [JsonProperty("secciones")]
        public string Condiciones { get; set; }
        public CondicionesTextosSeccionSlip()
        {
            Seccion = "CONDICIONES ESPECIALES";
        }
    }

    public partial class DisposicionesTextosSeccionSlip
    {
        [JsonProperty("tituloSeccion")]
        public string Seccion { get; set; }
        [JsonProperty("secciones")]
        public List<TextosSeccionSlip> Disposiciones { get; set; }
        public DisposicionesTextosSeccionSlip()
        {
            Seccion = "DISPOSICIONES FINALES";
            Disposiciones = new List<TextosSeccionSlip>();
        }
    }
}

