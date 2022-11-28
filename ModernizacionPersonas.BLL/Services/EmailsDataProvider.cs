using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class EmailsDataProvider
    {
        private readonly IDatosParametrizacionEmailReader datosParametrizacionEmailReader;
        private readonly IDatosParametrizacionEmailWriter datosParametrizacionEmailWriter;

        public EmailsDataProvider()
        {
            this.datosParametrizacionEmailReader = new DatosParametrizacionEmailTableReader();
            this.datosParametrizacionEmailWriter = new DatosParametrizacionEmailTableWriter();
        }

        public async Task<EmailParametrizacion> GetTextosEmailByTemplate(int codigoTemplate, int codigoSeccion)
        {
            try
            {
                var textoTemplate = await this.datosParametrizacionEmailReader.LeerParametrizacionEmailCodigoAsync(codigoSeccion, codigoTemplate);
                return textoTemplate;
            }
            catch (Exception ex)
            {
                throw new Exception("EmailsDaraProvider :: GetTextosEmailByTemplate", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateTextoEmailAsync(EmailParametrizacion email)
        {
            try
            {
                await this.datosParametrizacionEmailWriter.ActualizarEmailParametrizacionAsync(email);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("EmailsDaraProvider :: UpdateTextoEmailAsync", ex);
            }
        }
    }
}
