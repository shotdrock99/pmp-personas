using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class DatosGruposAseguradosMapper
    {
        private readonly IDatosAmparoGrupoAseguradoReader amparoGrupoAseguradoReaderService;
        private readonly IDatosGruposAseguradoReader grupoAseguradoReaderService;
        private readonly IDatosEdadesReader edadesReaderService;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReaderService;
        private readonly IDatosRangoGrupoAseguradoReader rangoReaderService;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosPersonasReader informacionPersonasReader;

        public DatosGruposAseguradosMapper()
        {
            this.grupoAseguradoReaderService = new DatosGruposAseguradosTableReader();
            this.amparoGrupoAseguradoReaderService = new DatosAmparoGrupoAseguradoTableReader();
            this.edadesReaderService = new DatosEdadesTableReader();
            this.opcionValorReaderService = new DatosOpcionValorAseguradoTableReader();
            this.rangoReaderService = new DatosRangoGrupoAseguradoTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
        }

        private async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, IEnumerable<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }

        public async Task<GrupoAsegurado> ConsultarGrupoAseguradoAsync(int codigoCotizacion, int version, int codigoGrupoAsegurado)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var grupoAsegurado = await this.grupoAseguradoReaderService.GetGrupoAseguradoAsync(codigoGrupoAsegurado);
            var amparosGrupo = await this.amparoGrupoAseguradoReaderService.LeerAmparoGrupoAseguradoAsync(codigoGrupoAsegurado);
                     
            // consultar rangos de grupo asegurado
            var rangosGrupo = await this.rangoReaderService.LeerRangoGrupoAseguradoAsync(codigoGrupoAsegurado);
            grupoAsegurado.RangosGrupo = rangosGrupo;
            
            foreach (AmparoGrupoAsegurado amparo in amparosGrupo)
            {
                var opcionValores = await this.opcionValorReaderService.LeerOpcionValorAseguradoAsync(amparo.CodigoAmparoGrupoAsegurado);
                if(grupoAsegurado.ConDistribucionAsegurados)
                {
                    grupoAsegurado.AseguradosOpcion1 = opcionValores.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                    grupoAsegurado.AseguradosOpcion2 = opcionValores.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                    grupoAsegurado.AseguradosOpcion3 = opcionValores.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                }
                

                foreach (OpcionValorAsegurado opcion in opcionValores)
                {
                    amparo.OpcionesValores.Add(opcion);
                }

                var amparoinfo = await this.informacionPersonasReader.TraerAmparoxCodigoAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, amparo.CodigoAmparo, informacionNegocio.CodigoSector);
                amparo.AmparoInfo = amparoinfo;
                amparo.Modalidad = amparoinfo.Modalidad;

                var informacionEdadesAmparo = await this.edadesReaderService.LeerEdadesAsync(amparo.CodigoGrupoAsegurado, amparo.CodigoAmparo);
                amparo.EdadesGrupo = informacionEdadesAmparo;
                grupoAsegurado.AmparosGrupo.Add(amparo);
            }

            if (amparosGrupo.Count() > 0)
            {
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, amparosGrupo).Result;
                var valorTotalOpcion1 = esBasico.OpcionesValores.FirstOrDefault();
                if (grupoAsegurado.CodigoTipoSuma == 2 && grupoAsegurado.ValorAsegurado > 0)
                {
                    grupoAsegurado.ValorAsegurado = grupoAsegurado.ValorAsegurado / valorTotalOpcion1.NumeroSalarios;
                }
            }


            grupoAsegurado.RangosGrupo = rangosGrupo;
            return grupoAsegurado;
        }
    }

    public class DatosGrupoAseguradosUtilities
    {
        public static List<Asegurado> ConvertToAsegurados(IEnumerable<Asegurado> asegurados)
        {
            var result = new List<Asegurado>();
            foreach (var a in asegurados)
            {
                result.Add(new Asegurado(a.CodigoAsegurado, a.NumeroDocumento, a.FechaNacimiento, a.ValorAsegurado, a.VetadoSarlaft));
            }

            return result;
        }

        public  decimal CalcularTasaGeneral(decimal sumaPrimasGrupos , decimal sumaValoresGrupos )
        {
            var result = sumaPrimasGrupos / sumaValoresGrupos * 1000;
            result = Math.Round(result, 4);

            return result;
        }
    }
}
