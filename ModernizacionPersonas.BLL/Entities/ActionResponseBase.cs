using ModernizacionPersonas.Entities;
using Newtonsoft.Json;
using System;

namespace ModernizacionPersonas.BLL.Entities
{
    public class ActionResponseBase
    {
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        [JsonProperty("estado")]
        public int CodigoEstadoCotizacion { get; set; }
        public CotizacionState CotizacionState
        {
            get
            {
                return (CotizacionState)this.CodigoEstadoCotizacion;
            }
        }
        public string NumeroCotizacion { get; set; }
        public ResponseStatus Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Details { get; set; }

        public ActionResponseBase()
        {
            this.Status = ResponseStatus.Valid;
        }

        public ActionResponseBase(int codigoCotizacion, int version, string numeroCotizacion, string message)
        {
            this.CodigoCotizacion = codigoCotizacion;
            this.Version = version;
            this.NumeroCotizacion = numeroCotizacion;
            this.ErrorMessage = message;
            this.Status = ResponseStatus.Valid;
        }

        public static ActionResponseBase CreateInvalidResponse(string message)
        {
            return new ActionResponseBase
            {
                ErrorMessage = message,
                Status = ResponseStatus.Invalid
            };
        }
    }

    public class ActionResponseBase<T> : ActionResponseBase
    {
        public static T CreateValidResponse()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }

    public enum ResponseStatus
    {
        Valid = 1,
        Invalid
    }
}
