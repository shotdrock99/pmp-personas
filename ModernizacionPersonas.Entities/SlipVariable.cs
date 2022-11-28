namespace ModernizacionPersonas.Entities
{
    public partial class SlipVariable
    {
        public int CodigoSeccion { get; set; }
        public int CodigoVariable { get; set; }
        public string Nombre { get; set; }
        public string TipoDato { get; set; }
        public string Valor { get; set; }
        public decimal ValorMaximo { get; set; }
        public int Activo { get; set; }
    }
}

