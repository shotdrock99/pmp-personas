namespace ModernizacionPersonas.DAL.Entities
{
    public class InsertarBloqueAseguradosResponse : DbActionResponse
    {
        public long TotalAsegurados { get; set; }
        public long RegistrosProcesados { get; set; }
        public long RegistrosDuplicados { get; set; }
        public int EdadPromedio { get; set; }
        public decimal ValorAsegurado { get; internal set; }
        public string Message { get; set; }

        public static InsertarBloqueAseguradosResponse CreateValidEmpty()
        {
            return new InsertarBloqueAseguradosResponse
            {
                TotalAsegurados = 0,
                RegistrosProcesados = 0,
                RegistrosDuplicados = 0,
                EdadPromedio = 0,
                Message = "El conteo de asegurados debe ser mayor a cero."
            };
        }
    }
}
