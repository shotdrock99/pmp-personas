using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class TextosSlipDataProvider : ITextosSlipDataProvider
    {
        private readonly IDatosTextosParametrizacionReader datosTextoSlipReader;
        private readonly IDatosTextosParametrizacionWriter datosTextoSlipWriter;
        private readonly IDatosPersonasReader personasReader;

        public TextosSlipDataProvider()
        {
            this.datosTextoSlipReader = new DatosTextoParametrizacionTableReader();
            this.datosTextoSlipWriter = new DatosTextoParametrizacionTableWriter();
            this.personasReader = new InformacionPersonasReader();
        }

        public async Task<IEnumerable<TextoSlipViewModel>> GetTextosSlipAsync()
        {
            try
            {
                var textos = await this.datosTextoSlipReader.LeerTextosParametrizacionAsync();
                var result = await this.AggregateTextosDataAsync(textos);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("TextosSlipDataProvider :: GetTextoesSlipAsync", ex);
            }
        }

        private async Task<List<TextoSlipViewModel>> AggregateTextosDataAsync(IEnumerable<TextoSlip> textos)
        {
            var result = new List<TextoSlipViewModel>();
            var ramos = await this.personasReader.TraerRamosAsync();
            foreach (var texto in textos)
            {
                var ramo = ramos.Where(x => x.CodigoRamo == texto.CodigoRamo).FirstOrDefault();
                var subramo = await this.personasReader.TraerSubRamoAsync(texto.CodigoRamo, texto.CodigoSubramo);
                var amparos = await this.personasReader.TraerAmparosAsync(texto.CodigoRamo, texto.CodigoSubramo, texto.CodigoSector);
                var sectores = await this.personasReader.TraerSectoresAsync(texto.CodigoRamo, texto.CodigoSubramo);
                var sector = sectores.Where(o => o.CodigoSector == texto.CodigoSector).FirstOrDefault();
                var amparo = amparos.Where(x => x.CodigoAmparo == texto.CodigoAmparo).FirstOrDefault();
                var nombreAmparo = amparo != null ? amparo.NombreAmparo : "";

                result.Add(new TextoSlipViewModel
                {
                    Codigo = texto.Codigo,
                    CodigoAmparo = texto.CodigoAmparo,
                    CodigoRamo = texto.CodigoRamo,
                    CodigoSeccion = texto.CodigoSeccion,
                    CodigoSubramo = texto.CodigoSubramo,
                    NombreAmparo = nombreAmparo,
                    NombreRamo = ramo == null ? " " : ramo.NombreAbreviado,
                    NombreSeccion = texto.NombreSeccion,
                    NombreSubramo = subramo.NombreSubRamo == null ? "" : subramo.NombreSubRamo,
                    Texto = texto.Texto,
                    Usuario = texto.Usuario,
                    FechaMovimiento = texto.FechaMovimiento,
                    Movimiento = texto.Movimiento,
                    CodigoSector = texto.CodigoSector,
                    NombreSector = sector == null ? "" : sector.NombreSector
                });
            }

            return result;
        }

        public async Task<ActionResponseBase> CreateTextoSlipAsync(TextoSlip model)
        {
            try
            {
                await this.datosTextoSlipWriter.CreateTextoParametrizacionAsync(model);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("TextosSlipDataProvider :: CreateTextoSlipAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateTextoSlipAsync(TextoSlip model)
        {
            try
            {
                await this.datosTextoSlipWriter.UpdateTextoParametrizacionAsync(model);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("TextosSlipDataProvider :: UpdateTextoSlipAsync", ex);
            }
        }
    }
}
