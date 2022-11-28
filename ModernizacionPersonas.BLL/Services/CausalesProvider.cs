using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CausalesProvider
    {
        private readonly IDatosCausalReader datosCausalReader;
        private readonly IDatosCausalWriter datosCausalWriter;


        public CausalesProvider()
        {
            this.datosCausalReader = new DatosCausalTableReader();
            this.datosCausalWriter = new DatosCausalTableWriter();
        }

        public async Task<IEnumerable<Causal>> GetCausales()
        {
            try
            {
                var causales = await this.datosCausalReader.GetCausales();
                //var response = causales.Where(c => c.Activo == 1);
                return causales;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetCausalesAsync", ex);
            }
        }

        public async Task<ActionResponseBase> PostCausalAsync(Causal causal)
        {
            try
            {
                await this.datosCausalWriter.GuardarCausalAsync(causal);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("CausalesProvider :: PostCausal", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateCausal(Causal causal)
        {
            try
            {
                await this.datosCausalWriter.ActualizarCausalAsync(causal);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("CausalesProvider :: UpdateCausal", ex);
            }
        }

        public async Task<ActionResponseBase> DisableCausal(int codigoCausal, string userName)
        {
            try
            {
                var causal = await this.datosCausalReader.GetCausalId(codigoCausal);
                if (causal.Activo == 1)
                {
                    await this.datosCausalWriter.EliminarCausalAsync(codigoCausal, userName);
                }
                else
                {
                    await this.datosCausalWriter.EliminarCausalAsync(codigoCausal, userName);
                }
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("CausalesProvider :: DeleteCausal", ex);
            }
        }
    }
}
