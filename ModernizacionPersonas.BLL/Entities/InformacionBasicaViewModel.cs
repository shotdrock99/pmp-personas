namespace ModernizacionPersonas.BLL.Entities
{
    public class InformacionBasicaViewModel
    {
        public int CodigoSucursal { get; set; }
        public PersonasServiceReference.Sucursal Sucursal { get; set; }
        public int CodigoRamo { get; set; }
        public PersonasServiceReference.Ramo Ramo { get; set; }
        public int CodigoSubramo { get; set; }
        public PersonasServiceReference.SubRamo Subramo { get; set; }
        public int CodigoZona { get; set; }
    }
}
