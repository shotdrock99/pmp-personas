using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class SeccionesSlipDataProvider
    {
        private readonly IDatosSeccionSlipReader datosSeccionSlipReader;
        private readonly IDatosSeccionSlipWriter datosSeccionSlipWriter;

        public SeccionesSlipDataProvider()
        {
            this.datosSeccionSlipReader = new DatosSeccionSlipTableReader();
            this.datosSeccionSlipWriter = new DatosSeccionSlipTableWriter();
        }

        public async Task<IEnumerable<SeccionSlip>> GetSeccionesSlipAsync()
        {
            try
            {
                var seccionesSlip = await this.datosSeccionSlipReader.GetSeccionesAsync();
                return seccionesSlip;
            }
            catch (Exception ex)
            {
                throw new Exception("SeccionesSlipDataProvider :: GetSeccionesSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateSeccionSlipAsync(SeccionSlip seccionSlip)
        {
            try
            {
                await this.datosSeccionSlipWriter.ActualizarSeccionAsync(seccionSlip);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("SeccionesSlipDataProvider :: UpdateSeccionSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> CreateSeccionSlipAsync(SeccionSlip seccionSlip)
        {
            try
            {
                await this.datosSeccionSlipWriter.GuardarSeccionAsync(seccionSlip);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("SeccionesSlipDataProvider :: CreateSeccionSlipAsync", ex);
            }
        }
    }
}
