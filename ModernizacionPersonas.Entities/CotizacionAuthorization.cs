namespace ModernizacionPersonas.Entities
{
    public class CotizacionAuthorization
    {
        public int CodigoAutorizacion { get; set; }
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public int? CodigoGrupoAsegurado { get; set; }
        // public string NombreGrupoAsegurado { get; set; }
        public decimal CodigoSucursal { get; set; }
        public decimal CodigoRamo { get; set; }
        public decimal CodigoSubramo { get; set; }
        public decimal? CodigoAmparo { get; set; }
        public string CampoEntrada { get; set; }
        public decimal ValorEntrada { get; set; }
        public int CodigoTipoAutorizacion { get; set; }
        public bool RequiereAutorizacion { get; set; }
        public string CodigoUsuario { get; set; }
        public string MensajeValidacion { get; set; }
        public string TipoAutorizacion { get; set; }
        public string NombreSeccion { get; set; }
        public bool SiseAuth { get; set; }
    }       

    public enum TipoDelegacionAutorizacion
    {
        PorDelegacion = 1,
        Directa
    }

    public enum TipoAutorizacion
    {
        Pasiva,
        Activa
    }
    public enum EMethodHttp
    {
        /// <summary>
        /// Metodo POST que usulamente es usado para la creación de registros.
        /// </summary>
        POST = 1,
        /// <summary>
        /// Metodo GET que usualmente es usado para obtener información.
        /// </summary>
        GET = 2,
        /// <summary>
        /// Metodo DELETE que usualmente es usado para eliminar información.
        /// </summary>
        DELETE = 3,
        /// <summary>
        /// Metodo PUT que usualmente es usado para actualizar la información.
        /// </summary>
        PUT = 4,
        /// <summary>
        /// Metodo PATCH
        /// </summary>
        PATCH = 5
    }
}
