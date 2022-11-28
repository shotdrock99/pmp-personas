using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class DatosGrupoAseguradoAseguradosUploader
    {
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private IAseguradosMapper aseguradosMapper;
        private readonly RangosCotizacionUtilities rangosUtilities;
        private readonly DatosGruposAseguradosProvider gruposAseguradosProvider;
        private readonly DatosOpcionValorAseguradoTableReader opcionesProvider;

        private IEnumerable<UploadAseguradoError> errores;
        private int totalRegistros;
        private int totalRegistrosValidos;

        public DatosGrupoAseguradoAseguradosUploader()
        {
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.gruposAseguradosProvider = new DatosGruposAseguradosProvider();
            this.opcionesProvider = new DatosOpcionValorAseguradoTableReader();
            this.rangosUtilities = new RangosCotizacionUtilities(this.informacionNegocioReader, this.informacionPersonasReader);
        }

        public async Task<UploadAseguradosResponse> UploadAseguradosAsync(int codigoCotizacion, int codigoGrupoAsegurados, int numeroSalarios, decimal valorMin, decimal valorMax, int edadMinimaAsegurado, int edadMaximaAsegurado, IFormFile file, int tipoEstructura)
        {
            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var codigoPerfilEdad = informacionNegocio.CodigoPerfilEdad;
                var codigoPerfilValor = informacionNegocio.CodigoPerfilValor;
                

                var grupos = await this.gruposAseguradosProvider.ObtenerGruposAseguradosAsync(codigoCotizacion, informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
                
                var grupo = grupos.Where(x => x.CodigoGrupoAsegurado == codigoGrupoAsegurados).FirstOrDefault();
                grupo.ValorMinAsegurado = valorMin;
                grupo.ValorMaxAsegurado = valorMax;

                await this.gruposAseguradosProvider.ActualizarGrupoAseguradoAsync(codigoCotizacion, grupo);


                var amparoBasico = grupo.AmparosGrupo.Where(x => !x.AmparoInfo.SiNoAdicional && x.AmparoInfo.SiNoBasico).FirstOrDefault();
                

                var valorMinimoGrupoAsegurados = valorMin;
                var valorMaximoGrupoAsegurados = valorMax;

                var edadMinimaAmparoBasico = edadMinimaAsegurado;
                var edadMaximaAmparoBasico = edadMaximaAsegurado;

                decimal valorMinimoGrupoAsegurado = 0;
                decimal valorMaximoPerfil = 0;
                var edadMinima = 0;
                var edadMaxima = 999;

                var informacionRangosEdad = await this.rangosUtilities.ObtenerRangosPerfilEdadAsync(codigoPerfilEdad);
                if (informacionRangosEdad != null)
                {
                    edadMinima = informacionRangosEdad.ValorMinimo;
                    edadMaxima = informacionRangosEdad.ValorMaximo;
                }

                var informacionRangosValor = await this.rangosUtilities.ObtenerRangosPerfilValorAsync(codigoPerfilValor);
                if (informacionRangosValor != null)
                {
                    // TODO validar si se debe tomar valores max/min de grupo asegurado
                    valorMinimoGrupoAsegurado = informacionRangosValor.ValorMinimo;
                    valorMaximoPerfil = informacionRangosValor.ValorMaximo;
                }

                var mapAseguradosArgs = new MapAseguradosViewModelArgs
                {
                    CodigoGrupoAsegurados = codigoGrupoAsegurados,
                    File = file,
                    ValorMinimoPerfil = valorMinimoGrupoAsegurado,
                    ValorMaximoPerfil = valorMaximoPerfil,
                    ValorMinimoGrupoAsegurado = valorMinimoGrupoAsegurados,
                    ValorMaximoGrupoAsegurado = valorMaximoGrupoAsegurados,
                    EdadMinimaPerfil = edadMinima,
                    EdadMaximaPerfil = edadMaxima,
                    // TODO no es posible obtener los valores del amparo basico del grupo, por que los amparos del grupo aun no han sido guardados
                    EdadMinimaAmparoBasico = edadMinimaAmparoBasico,
                    EdadMaximaAmparoBasico = edadMaximaAmparoBasico,
                    NumeroSalarios = numeroSalarios,
                    TipoEstructura = tipoEstructura                    
                };

                var asegurados = await this.MapAseguradosAsync(mapAseguradosArgs);
                var insertarAseguradosArgs = new InsertarAseguradosArgs
                {
                    CodigoCotizacion = codigoCotizacion,
                    Version = 0,
                    CodigoGrupoAsegurados = codigoGrupoAsegurados,
                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                    CodigoRamo = informacionNegocio.CodigoRamo,
                    CodigoSubRamo = informacionNegocio.CodigoSubramo,
                    CodigoSector = informacionNegocio.CodigoSector,
                    // NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                    CodigoTipoTasa = informacionNegocio.CodigoTipoTasa1,
                    Asegurados = asegurados,
                    NumeroAsegurados = asegurados.Count(),
                    TipoEstructura = tipoEstructura
                    //Rangos = 
                };

                var valorAsegurado = asegurados.Where(x=> x.VetadoSarlaft != true).Sum(x => x.ValorAsegurado);
                double edadPromedio = 0;
                if (asegurados.Count > 0)
                {
                    edadPromedio = asegurados.Where(x => x.VetadoSarlaft != true).Average(x => x.Edad);
                }
                var registrosProcesados = asegurados.Where(x => !x.VetadoSarlaft).Count();
                
                var response = await this.gruposAseguradosProvider.InsertarAseguradosFromFileAsync(codigoCotizacion, insertarAseguradosArgs);
                return new UploadAseguradosResponse
                {
                    Status = ResponseStatus.Valid,
                    Asegurados = asegurados,
                    //EdadPromedio = (int)response.EdadPromedio,
                    EdadPromedio = (int)edadPromedio,
                    ValorAsegurado = valorAsegurado,
                    RegistrosDuplicados = (int)response.RegistrosDuplicados,
                    RegistrosProcesados = registrosProcesados,
                    TotalRegistros = this.totalRegistros,
                    TotalRegistrosValidos = this.totalRegistrosValidos,
                    TotalAsegurados = response.TotalAsegurados,
                    Errores = this.errores
                };
            }
            catch (Exception ex)
            {
                throw new Exception("DatosGrupoAseguradoAseguradosUploader :: UploadAsegurados", ex);
            }
        }

        private async Task<List<Asegurado>> MapAseguradosAsync(MapAseguradosViewModelArgs args)
        {
            var factory = new UploadAseguradosMapperFactory();
            var fileStream = args.File.OpenReadStream();

            this.aseguradosMapper = factory.GetMapper(args, fileStream);
            var mapResponse = await this.aseguradosMapper.MapAsync();
            this.errores = this.aseguradosMapper.GetErrores();
            this.totalRegistros = mapResponse.TotalRegistros;
            this.totalRegistrosValidos = mapResponse.TotalRegistrosValidos;

            double edadPromedio = 0;
            if (mapResponse.Asegurados.Count() > 0)
            {
                edadPromedio = Math.Round(mapResponse.Asegurados.Select(x => x.Edad).Average());
            }

            var asegurados = mapResponse.Asegurados.ToList();
            return asegurados;
        }
    }
}
