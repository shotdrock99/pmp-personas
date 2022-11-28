using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class TextosSlipMockDataProvider : ITextosSlipDataProvider
    {
        private readonly IDatosTextosSlipReader datosTextoSlipReader;
        private readonly IDatosTextosSlipWriter datosTextoSlipWriter;

        public async Task<IEnumerable<TextoSlipViewModel>> GetTextosSlipAsync()
        {
            var result = new List<TextoSlipViewModel>();

            result.Add(new TextoSlipViewModel
            {
                Codigo = 1,
                CodigoAmparo = 0,
                NombreAmparo = "",
                CodigoRamo = 15,
                NombreRamo = "Ramo 1",
                CodigoSubramo = 1,
                NombreSubramo = "Subramo 1",
                CodigoSeccion = 42,
                NombreSeccion = "Seccion 1",
                Texto = "Amparar contra el riesgo de muerte a cada uno de los miembros del grupo asegurado, que ocurra dentro de la vigencia de la póliza, por una causa no excluida, incluyendo homicidio y suicidio desde el primer día de vigencia y cualquier patología diagnosticada por primera vez dentro de la vigencia de la póliza."
            });

            result.Add(new TextoSlipViewModel
            {
                Codigo = 5,
                CodigoAmparo = 2,
                NombreAmparo = "Amparo 2",
                CodigoRamo = 16,
                NombreRamo = "Ramo 2",
                CodigoSubramo = 1,
                NombreSubramo = "Subramo 2",
                CodigoSeccion = 42,
                NombreSeccion = "Seccion 2",
                Texto = "Aseguradora Solidaria de Colombia Ampara contra el riesgo de Muerte e Incapacidad Total y Permanente a las personas naturales deudoras del Tomador de la Póliza; de manera que el valor de la obligación o crédito quede cancelado cuando se presente un evento amparado."
            });

            return await Task.FromResult(result);
        }

        public async Task<ActionResponseBase> UpdateTextoSlipAsync(TextoSlip model)
        {
            try
            {
                await this.datosTextoSlipWriter.UpdateTextoAsync(model);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("TextosSlipDataProvider :: UpdateTextoSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> CreateTextoSlipAsync(TextoSlip model)
        {
            try
            {
                await this.datosTextoSlipWriter.CreateTextoAsync(model);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("TextosSlipDataProvider :: CreateTextoSlipAsync", ex);
            }
        }
    }
}
