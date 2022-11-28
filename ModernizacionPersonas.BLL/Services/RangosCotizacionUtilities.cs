using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class RangosCotizacionUtilities
    {
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosPersonasReader informacionPersonasReader;

        public RangosCotizacionUtilities(IDatosInformacionNegocioReader informacionNegocioReader, IDatosPersonasReader informacionPersonasReader)
        {
            this.informacionNegocioReader = informacionNegocioReader;
            this.informacionPersonasReader = informacionPersonasReader;
        }

        public List<Rango> ConfigurarRangosEdades(int codigoGrupoAsegurado, List<Asegurado> asegurados, IEnumerable<RangoEdad> rangos)
        {
            var result = new List<Rango>();
            foreach (var r in rangos)
            {
                var aseguradoMax = 0;
                var filtered = asegurados.Where(x => x.Edad >= r.EdadDesde && x.Edad <= r.EdadHasta && !x.VetadoSarlaft);
                if(filtered.Count() > 0) {
                    aseguradoMax = filtered.Max(x => x.Edad);
                }
                
                var valorAsegurado = filtered.Sum(x => x.ValorAsegurado);
                var count_aseg = filtered.Count();
                result.Add(new Rango
                {
                    CodigoGrupoAsegurado = codigoGrupoAsegurado,
                    EdadMinAsegurado = r.EdadDesde,
                    EdadMaxAsegurado = r.EdadHasta,
                    NumeroAsegurados = count_aseg,
                    ValorAsegurado = (decimal)valorAsegurado,
                    EdadMaxRango = aseguradoMax
                    
                });
            }

            return result;
        }

        public List<Rango> ConfigurarRangosValores(int codigoGrupoAsegurado, List<Asegurado> asegurados, IEnumerable<RangoValor> rangos)
        {
            var result = new List<Rango>();
            foreach (var r in rangos)
            {
                var count_aseg = asegurados.Where(x => (decimal)x.ValorAsegurado >= r.ValorDesde && (decimal)x.ValorAsegurado <= r.ValorHasta).Count();
                result.Add(new Rango
                {
                    CodigoGrupoAsegurado = codigoGrupoAsegurado,
                    NumeroAsegurados = count_aseg,
                });
            }

            return result;
        }

        public async Task<ObtenerRangosPerfilEdadResponse> ObtenerRangosPerfilEdadAsync(int codigoPerfilEdad)
        {
            if (codigoPerfilEdad == 0)
            {
                return null;
            }

            var edadMinima = 0;
            var edadMaxima = 0;
            var rangos = await this.informacionPersonasReader.TraerRangosPorPerfilEdadAsync(codigoPerfilEdad);
            if (rangos.Count() > 0)
            {
                edadMinima = rangos.FirstOrDefault().EdadDesde;
                edadMaxima = rangos.LastOrDefault().EdadHasta;
            }

            return new ObtenerRangosPerfilEdadResponse
            {
                Rangos = rangos,
                ValorMinimo = edadMinima,
                ValorMaximo = edadMaxima
            };
        }

        public async Task<ObtenerRangosPerfilValorResponse> ObtenerRangosPerfilValorAsync(int codigoPerfilValor)
        {
            if (codigoPerfilValor == 0)
            {
                return null;
            }

            decimal valorDesde = 0;
            decimal valorHasta = 0;
            var rangos = await this.informacionPersonasReader.TraerRangosPorPerfilValorAsync(codigoPerfilValor);
            if (rangos.Count() > 0)
            {
                valorDesde = rangos.FirstOrDefault().ValorDesde;
                valorHasta = rangos.LastOrDefault().ValorHasta;
            }

            return new ObtenerRangosPerfilValorResponse
            {
                Rangos = rangos,
                ValorMinimo = valorDesde,
                ValorMaximo = valorHasta
            };
        }

        internal Task ObtenerRangosPerfilEdadAsync(object codigoPerfilEdad)
        {
            throw new NotImplementedException();
        }
    }
}
