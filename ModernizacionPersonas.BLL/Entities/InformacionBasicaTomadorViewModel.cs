namespace ModernizacionPersonas.BLL.Entities
{
    public class InformacionBasicaTomadorViewModel
    {
        public int CodigoTomador { get; set; }
        public int CodigoTipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public int CodigoActividadEconomica { get; set; }
        public int CodigoPais { get; set; }
        public int CodigoDepartamento { get; set; }
        public int CodigoMunicipio { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string NombreContacto { get; set; }
        public string TelefonoContacto1 { get; set; }
        public string TelefonoContacto2 { get; set; }
        public string TomadorSlip { get; set; }
    }
}
