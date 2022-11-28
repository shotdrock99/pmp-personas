namespace ModernizacionPersonas.Entities
{
    public partial class TomadorSlip
    {
        public int CodigoTomador { get; set; }
        public string Nombre { get; set; }
        public int CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int CodigoDepartamento { get; set; }
        public int CodigoCiudad { get; set; }
        public string Actividad { get; set; }
        public bool EsIntermediario { get; set; }
        public string  NombreTomadorSlip { get; set; }
    }
}

