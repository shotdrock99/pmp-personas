using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionStateWriter
    {
        private readonly IDatosCotizacionWriter datosCotizacionWriter;

        public CotizacionStateWriter()
        {
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
        }

        public async Task<DbActionResponse> UpdateCotizacionStateAsync(int codigoCotizacion, CotizacionState newState)
        {
            var response = await this.datosCotizacionWriter.CambiarEstadoAsync(codigoCotizacion, newState);
            return response;
        }
    }
}
