using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CotizacionAuthorizationDTO
    {
        public int CodigoAutorizacion { get; set; }
        public int CodigoCotizacion { get; set; }
        public int Version { get; set; }
        public decimal CodigoSucursal { get; set; }
        public decimal CodigoRamo { get; set; }
        public decimal CodigoSubramo { get; set; }
        public int? CodigoGrupoAsegurado { get; set; }
        public string NombreGrupoAsegurado { get; set; }
        public decimal? CodigoAmparo { get; set; }
        public string NombreAmparo { get; set; }
        public string CampoEntrada { get; set; }
        public decimal ValorEntrada { get; set; }
        public int CodigoTipoAutorizacion { get; set; }
        public bool RequiereAutorizacion { get; set; }
        public string CodigoUsuario { get; set; }
        public string MensajeValidacion { get; set; }
        public string NombreSeccion { get; set; }
        public bool SiseAuth { get; set; }

        public IEnumerable<CotizacionAuthorizationDTO> Items { get; set; }

        internal static CotizacionAuthorizationDTO Create(CotizacionAuthorization item)
        {
            var result = new CotizacionAuthorizationDTO
            {
                CampoEntrada = item.CampoEntrada,
                CodigoAmparo = item.CodigoAmparo,
                CodigoAutorizacion = item.CodigoAutorizacion,
                CodigoCotizacion = item.CodigoCotizacion,
                CodigoGrupoAsegurado = item.CodigoGrupoAsegurado,
                CodigoRamo = item.CodigoRamo,
                CodigoSubramo = item.CodigoSubramo,
                CodigoSucursal = item.CodigoSucursal,
                CodigoTipoAutorizacion = item.CodigoTipoAutorizacion,
                CodigoUsuario = item.CodigoUsuario,
                MensajeValidacion = item.MensajeValidacion,
                NombreGrupoAsegurado = "",
                RequiereAutorizacion = item.RequiereAutorizacion,
                ValorEntrada = item.ValorEntrada,
                Version = item.Version,
                NombreSeccion = item.NombreSeccion
            };

            return result;
        }
    }
}
