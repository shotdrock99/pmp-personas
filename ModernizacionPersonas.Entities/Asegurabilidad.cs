namespace ModernizacionPersonas.Entities
{
    public partial class Asegurabilidad
    {
        public int CodigoAsegurabilidad { get; set; }
        public int EdadDesde { get; set; }
        public int EdadHasta { get; set; }
        public decimal ValorIndividualDesde { get; set; }
        public decimal ValorIndividualHasta { get; set; }
        public string Requisitos { get; set; }
    }
}

