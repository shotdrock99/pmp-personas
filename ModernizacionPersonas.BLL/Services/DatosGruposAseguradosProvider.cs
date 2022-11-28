using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class DatosGruposAseguradosProvider
    {
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly IDatosAmparoGrupoAseguradoWriter amparoGrupoWriterService;
        private readonly IDatosGrupoAseguradoWriter gruposAseguradosWriter;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly IDatosEdadesWriter edadesWriterService;
        private readonly IDatosOpcionValorAseguradoWriter opcionesValoresWriter;
        private readonly IDatosOpcionValorAseguradoReader opcionesValoresReader;
        private readonly IDatosAseguradoWriter aseguradoWriterService;
        private readonly IDatosRangoGrupoAseguradoWriter rangosWriter;
        private readonly IDatosRangoGrupoAseguradoReader rangosReader;
        private readonly IDatosAmparoGrupoAseguradoReader amparosReader;
        private readonly IDatosAseguradoReader aseguradorReaderService;
        private readonly IDatosEdadesReader edadesReaderService;

        private readonly SISEAseguradosWriter SISEWriterService;
        private readonly SISEAseguradosSummaryProcessor SISEProcesor;
        private readonly DatosGruposAseguradosMapper gruposAseguradosMapper;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly RangosCotizacionUtilities rangosUtilities;

        public DatosGruposAseguradosProvider()
        {
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.amparoGrupoWriterService = new DatosAmparoGrupoAseguradoTableWriter();
            this.gruposAseguradosWriter = new DatosGrupoAseguradoTableWriter();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.edadesWriterService = new DatosEdadesTableWriter();
            this.opcionesValoresWriter = new DatosOpcionValorAseguradoTableWriter();
            this.opcionesValoresReader = new DatosOpcionValorAseguradoTableReader();
            this.aseguradoWriterService = new DatosAseguradoTableWriter();
            this.edadesReaderService = new DatosEdadesTableReader();

            this.SISEWriterService = new SISEAseguradosWriter();
            this.SISEProcesor = new SISEAseguradosSummaryProcessor();
            this.gruposAseguradosMapper = new DatosGruposAseguradosMapper();
            this.rangosWriter = new DatosRangoGrupoAseguradoTableWriter();
            this.rangosReader = new DatosRangoGrupoAseguradoTableReader();
            this.amparosReader = new DatosAmparoGrupoAseguradoTableReader();
            this.aseguradorReaderService = new DatosAseguradoTableReader();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.rangosUtilities = new RangosCotizacionUtilities(this.informacionNegocioReader, this.informacionPersonasReader);
        }

        public async Task<IEnumerable<GrupoAsegurado>> ObtenerGruposAseguradosAsync(int codigoCotizacion, int codigoRamo, int codigoSubramo, int codigoSector)
        {
            var tasks = new List<Task<GrupoAsegurado>>();
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var grupos = await gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            
            foreach (var g in grupos)
            {
                tasks.Add(FillGroup(g, amparos));
            }

            return await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<GrupoAsegurado>> ObtenerGruposAseguradosAsync(int codigoCotizacion, int codigoCotizacionOld, int codigoRamo, int codigoSubramo, int codigoSector)
        {
            var tasks = new List<Task<GrupoAsegurado>>();
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var grupos = await gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            var gruposOld = await gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacionOld);
            
            foreach (var g in grupos)
            {
                foreach(var go in gruposOld)
                {
                    tasks.Add(FillGroup(g, go, amparos));
                }
                
            }

            return await Task.WhenAll(tasks);
        }


        private async Task<GrupoAsegurado> FillGroup(GrupoAsegurado g, IEnumerable<PersonasServiceReference.Amparo> amparos)
        {
            var amparosGrupo = await this.amparosReader.LeerAmparoGrupoAseguradoAsync(g.CodigoGrupoAsegurado);


            foreach (var ag in amparosGrupo)
            {
                var edades = await this.edadesReaderService.LeerEdadesAsync(ag.CodigoGrupoAsegurado, ag.CodigoAmparo);
                var a = amparos.ToList().Find(x => x.CodigoAmparo == ag.CodigoAmparo);
                ag.AmparoInfo = a ?? throw new Exception($"El código amparo {ag.CodigoAmparo} no se encuentra parametrizado.");
                ag.CodigoGrupoAmparo = a.CodigoGrupoAmparo;
                var opciones = await this.opcionesValoresReader.LeerOpcionValorAseguradoAsync(ag.CodigoAmparoGrupoAsegurado);
                if (g.ConDistribucionAsegurados)
                {
                    g.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                    g.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                    g.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                }

                ag.OpcionesValores = opciones.ToList();
                ag.EdadesGrupo = edades;

            }

            g.AmparosGrupo = amparosGrupo.ToList();
            var rangos = await this.rangosReader.LeerRangoGrupoAseguradoAsync(g.CodigoGrupoAsegurado);
            

            g.RangosGrupo = rangos;

            return await Task.FromResult(g);
        }


        private async Task<GrupoAsegurado> FillGroup(GrupoAsegurado g, GrupoAsegurado go, IEnumerable<PersonasServiceReference.Amparo> amparos)
        {
            var amparosGrupo = await this.amparosReader.LeerAmparoGrupoAseguradoAsync(g.CodigoGrupoAsegurado);
            
            
            foreach (var ag in amparosGrupo)
            {
                var edades = await this.edadesReaderService.LeerEdadesAsync(ag.CodigoGrupoAsegurado, ag.CodigoAmparo);
                var a = amparos.ToList().Find(x => x.CodigoAmparo == ag.CodigoAmparo);
                ag.AmparoInfo = a ?? throw new Exception($"El código amparo {ag.CodigoAmparo} no se encuentra parametrizado.");
                ag.CodigoGrupoAmparo = a.CodigoGrupoAmparo;
                var opciones = await this.opcionesValoresReader.LeerOpcionValorAseguradoAsync(ag.CodigoAmparoGrupoAsegurado);
                if (g.ConDistribucionAsegurados)
                {
                    g.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                    g.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                    g.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                }
                ag.OpcionesValores = opciones.ToList();
                ag.EdadesGrupo = edades;
                
            }

            g.AmparosGrupo = amparosGrupo.ToList();
            var  rangos = await this.rangosReader.LeerRangoGrupoAseguradoAsync(go.CodigoGrupoAsegurado);
            
            g.RangosGrupo = rangos;

            return await Task.FromResult(g);
        }



        //grupo Asegurado
        public async Task<CreateActionResponse> CreateGrupoAseguradoAsync(int codigoCotizacion, int version, GrupoAsegurado model)
        {
            // TODO consultar una sola vez infomracion de neogio
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var codigoGrupoAsegurado = await this.gruposAseguradosWriter.CrearGrupoAseguradoAsync(model);
            // Actualizar el estado de la cotizacion
            if (informacionNegocio.CotizacionState < CotizacionState.OnGruposAsegurados)
            {
                await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnGruposAsegurados);
            }

            return new CreateActionResponse
            {
                CodigoCotizacion = codigoCotizacion,
                Codigo = codigoGrupoAsegurado
            };
        }

        public async Task UpdateGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model)
        {
            await this.gruposAseguradosWriter.UpdateNombreGrupoAseguradoAsync(codigoCotizacion, model);
        }

        public async Task ActualizarGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model)
        {
            await this.gruposAseguradosWriter.ActualizarGrupoAseguradoAsync(codigoCotizacion, model);
        }

        public async Task<ActionResponseBase> EliminarGrupoAseguradoAsync(int codigoCotizacion, int codigo)
        {
            // update cotizacion modified flag to true
            await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
            await this.gruposAseguradosWriter.EliminarGrupoAseguradoAsync(codigo);
            return new ActionResponseBase();
        }

        public async Task<ActionResponseBase> EliminarAseguradosAsync(int codigoCotizacion, int codigo)
        {
            // update cotizacion modified flag to true
            await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
            await this.aseguradoWriterService.EliminarAseguradosAsync(codigo);
            return new ActionResponseBase();
        }

        public async Task<InsertarValoresGrupoAseguradoResponse> InsertarValoresGrupoAseguradoAsync(int codigoCotizacion, int version, GrupoAsegurado grupoAsegurado)
        {
            var isValid = this.ValidateGroupAsegurado(grupoAsegurado);
            if (!isValid)
            {
                return new InsertarValoresGrupoAseguradoResponse { Status = ResponseStatus.Invalid };
            }

            var grupoAseguradoOriginal = this.gruposAseguradosReader.GetGrupoAseguradoAsync(grupoAsegurado.CodigoGrupoAsegurado).Result;

            // clean grupos asegurados
            await this.gruposAseguradosWriter.LimpiarGrupoAseguradoAsync(grupoAsegurado.CodigoGrupoAsegurado);

            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);

            var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector, grupoAsegurado.AmparosGrupo).Result;
            var minVal = esBasico.OpcionesValores.Where(x => x.ValorAsegurado > 0).Count() > 0 ? esBasico.OpcionesValores.Where(x => x.ValorAsegurado > 0).Min(x => x.ValorAsegurado) : grupoAseguradoOriginal.ValorMinAsegurado;
            var maxVal = esBasico.OpcionesValores.Max(x => x.ValorAsegurado) > 0 ? esBasico.OpcionesValores.Max(x => x.ValorAsegurado) : grupoAseguradoOriginal.ValorMaxAsegurado;
            
            var valorMin = grupoAseguradoOriginal.ValorMinAsegurado != minVal ? esBasico.OpcionesValores.Where(x => x.ValorAsegurado > 0).Min(x => x.ValorAsegurado) : grupoAseguradoOriginal.ValorMinAsegurado ;
            var valorMax = grupoAseguradoOriginal.ValorMaxAsegurado != maxVal ? esBasico.OpcionesValores.Max(x => x.ValorAsegurado) : grupoAseguradoOriginal.ValorMaxAsegurado;

            grupoAsegurado.ValorMinAsegurado = grupoAsegurado.CodigoTipoSuma == 1 ? grupoAsegurado.ValorMinAsegurado != valorMin ? valorMin : grupoAseguradoOriginal.ValorMinAsegurado : grupoAsegurado.ValorMinAsegurado ;
            grupoAsegurado.ValorMaxAsegurado = grupoAsegurado.CodigoTipoSuma == 1 ? grupoAsegurado.ValorMaxAsegurado != valorMax ? valorMax : grupoAseguradoOriginal.ValorMaxAsegurado : grupoAsegurado.ValorMaxAsegurado ;

            var asegurados = await this.aseguradorReaderService.LeerAseguradosAsync(grupoAsegurado.CodigoGrupoAsegurado);
            // asignar valores calculados de numero de asegurados y valor asegurado
            var asegurado = new Asegurado();
            
            grupoAsegurado.NumeroAsegurados = asegurados.Count() > 0 ? asegurados.Count() : grupoAsegurado.NumeroAsegurados;
            grupoAsegurado.EdadPromedioAsegurados = informacionNegocio.ConListaAsegurados ? asegurados.Count() > 0 ? (int)asegurados.Average(x => asegurado.CalculateAge(x.FechaNacimiento)) : grupoAsegurado.EdadPromedioAsegurados: grupoAsegurado.EdadPromedioAsegurados;

            var tieneSiniestralidad = informacionNegocio.CodigoTipoTasa1 == 5 || informacionNegocio.CodigoTipoTasa2 == 5;
            var args = new InsertarAseguradosArgs
            {
                CodigoCotizacion = codigoCotizacion,
                Version = version,
                CodigoGrupoAsegurados = grupoAsegurado.CodigoGrupoAsegurado,
                ValorAsegurado = grupoAsegurado.ValorAsegurado,
                NumeroAsegurados = grupoAsegurado.NumeroAsegurados,                
                EdadPromedioAsegurados = grupoAsegurado.EdadPromedioAsegurados,
                CodigoRamo = informacionNegocio.CodigoRamo,
                CodigoSubRamo = informacionNegocio.CodigoSubramo,
                CodigoSector = informacionNegocio.CodigoSector,
                CodigoTipoTasa = informacionNegocio.CodigoTipoTasa1,
                ConListaAsegurados = informacionNegocio.ConListaAsegurados,
                CodigoPerfilEdad = informacionNegocio.CodigoPerfilEdad,
                CodigoPerfilValor = informacionNegocio.CodigoPerfilValor,
                TipoEstructura = grupoAsegurado.TipoEstructura
            };

            
            args.Asegurados = DatosGrupoAseguradosUtilities.ConvertToAsegurados(asegurados);

            // Inserte los rangos configurados manualmente si aplica
            await this.InsertarRangosAsync(grupoAsegurado, args);

            var valorTotalOpcion1 = new OpcionValorAsegurado();
            var valorTotalOpcion2 = new OpcionValorAsegurado();
            var valorTotalOpcion3 = new OpcionValorAsegurado();

            if (esBasico.OpcionesValores.Count > 1)
            {
                valorTotalOpcion1 = esBasico.OpcionesValores[0];
                valorTotalOpcion2 = esBasico.OpcionesValores[1];
                valorTotalOpcion3 = esBasico.OpcionesValores[2];
            }
            else
            {
                valorTotalOpcion1 = esBasico.OpcionesValores.FirstOrDefault();
            }
                      
            

            var totalAseg1 = grupoAsegurado.ConDistribucionAsegurados ? valorTotalOpcion1.ValorAsegurado * valorTotalOpcion1.NumeroAsegurados : valorTotalOpcion1.ValorAsegurado * grupoAsegurado.NumeroAsegurados;
            var totalAseg2 = grupoAsegurado.ConDistribucionAsegurados ? valorTotalOpcion2.ValorAsegurado * valorTotalOpcion2.NumeroAsegurados : valorTotalOpcion2.ValorAsegurado * grupoAsegurado.NumeroAsegurados;
            var totalAseg3 = grupoAsegurado.ConDistribucionAsegurados ? valorTotalOpcion3.ValorAsegurado * valorTotalOpcion3.NumeroAsegurados : valorTotalOpcion3.ValorAsegurado * grupoAsegurado.NumeroAsegurados;

            var totalValorDistribucion = totalAseg1 + totalAseg2 + totalAseg3;

            if (grupoAsegurado.CodigoTipoSuma == 2)
            {
                if (grupoAseguradoOriginal.ValorAsegurado > 0)
                {
                    if (grupoAsegurado.ValorAsegurado == grupoAseguradoOriginal.ValorAsegurado / valorTotalOpcion1.NumeroSalarios)
                    {
                        grupoAsegurado.ValorAsegurado = grupoAseguradoOriginal.ValorAsegurado;
                    }
                    else
                    {
                        grupoAsegurado.ValorAsegurado = grupoAsegurado.CodigoTipoSuma == 1 ? valorTotalOpcion1.ValorAsegurado * grupoAsegurado.NumeroAsegurados : grupoAsegurado.CodigoTipoSuma == 5 ? grupoAsegurado.ValorAsegurado : valorTotalOpcion1.ValorAsegurado;
                    }
                }
                else
                {
                    grupoAsegurado.ValorAsegurado = grupoAsegurado.CodigoTipoSuma == 1 ? valorTotalOpcion1.ValorAsegurado * grupoAsegurado.NumeroAsegurados : grupoAsegurado.CodigoTipoSuma == 5 ? grupoAsegurado.ValorAsegurado : valorTotalOpcion1.ValorAsegurado;
                }
            }
            else
            {
                grupoAsegurado.ValorAsegurado = grupoAsegurado.CodigoTipoSuma == 1 ? grupoAsegurado.ConDistribucionAsegurados ? totalValorDistribucion :valorTotalOpcion1.ValorAsegurado * grupoAsegurado.NumeroAsegurados : grupoAsegurado.CodigoTipoSuma == 5 ?  grupoAsegurado.ValorAsegurado : valorTotalOpcion1.ValorAsegurado;
            }

            if (asegurados.Count() > 0 && grupoAsegurado.CodigoTipoSuma == 2)
            {
                grupoAsegurado.ValorAsegurado = asegurados.Sum(x => x.ValorAsegurado) * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios;
            }

            if (informacionNegocio.CodigoTipoTasa1 == 3)
            {
                var rangos = this.rangosReader.LeerRangoGrupoAseguradoAsync(grupoAsegurado.CodigoGrupoAsegurado).Result;
                grupoAsegurado.NumeroAsegurados = rangos.Sum(x => x.NumeroAsegurados);
            }

            if (informacionNegocio.CodigoTipoTasa1 == 3)
            {
                var rangos = this.rangosReader.LeerRangoGrupoAseguradoAsync(grupoAsegurado.CodigoGrupoAsegurado).Result;
                grupoAsegurado.NumeroAsegurados = rangos.Sum(x => x.NumeroAsegurados);
            }

            await this.gruposAseguradosWriter.ActualizarGrupoAseguradoAsync(grupoAsegurado);
            foreach (var grupo in grupoAsegurado.AmparosGrupo)
            {
                var codigoAmparoGrupoAsegurado = await amparoGrupoWriterService.CrearAmparoGrupoAseguradoAsync(grupo);
                var indice = 1;
                foreach (var opcion in grupo.OpcionesValores)
                {
                    opcion.CodigoAmparoGrupoAsegurado = codigoAmparoGrupoAsegurado;
                    opcion.IndiceOpcion = indice;
                    opcion.NumeroAsegurados = opcion.IndiceOpcion == 1 ? grupoAsegurado.AseguradosOpcion1 : opcion.IndiceOpcion == 2 ? grupoAsegurado.AseguradosOpcion2 : grupoAsegurado.AseguradosOpcion3;
                    // opcion.ValorAsegurado = 0;
                    if (asegurados.Count() > 0 && grupoAsegurado.CodigoTipoSuma == 2)
                    {
                        opcion.ValorAsegurado = asegurados.Sum(x => x.ValorAsegurado) * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios;
                    }
                    opcion.ValorAsegurado = opcion.ValorAseguradoDias > 0 ? opcion.ValorAseguradoDias : opcion.ValorAsegurado;
                    await opcionesValoresWriter.CrearOpcionValorAseguradoAsync(opcion);

                    indice++;
                }

                await edadesWriterService.InsertarEdadAmparoAsync(grupo.CodigoGrupoAsegurado, grupo.CodigoAmparo, grupo.EdadesGrupo);
            }

            var currentState = informacionNegocio.CotizacionState;
            // Actualizar estado de la cotizacion 
            if (!tieneSiniestralidad && informacionNegocio.CotizacionState < CotizacionState.OnGruposAsegurados)
            {
                await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnGruposAsegurados);
            }

            // Insertar asegurados en SISE
            args.CodigoPerfilEdad = informacionNegocio.CodigoPerfilEdad;
            args.CodigoPerfilValor = informacionNegocio.CodigoPerfilValor;
            var insertarSISEResponse = await this.InsertarAseguradosSISEAsync(args);

            // update cotizacion modified flag to true
            await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);

           

            return new InsertarValoresGrupoAseguradoResponse
            {
                ErrorMessage = insertarSISEResponse.ErrorMessage,
                CodigoCotizacion = codigoCotizacion,
                CodigoGrupoAsegurado = args.CodigoGrupoAsegurados
            };
        }

        private bool ValidateGroupAsegurado(GrupoAsegurado grupoAsegurado)
        {
            var result = true;
            if (grupoAsegurado.AmparosGrupo.Count == 0)
            {
                result = false;
            }

            return result;
        }

        private async Task InsertarRangosAsync(GrupoAsegurado model, InsertarAseguradosArgs args)
        {
            if (!args.ConListaAsegurados)
            {
                // inserte los rangos si tiene y el tipo de tasa es Tipo rango edades Id 3
                if (model.RangosGrupo.ToList().Count > 0 && args.CodigoTipoTasa == 3)
                {
                    this.CrearRangosAsync(model.RangosGrupo).Wait();
                    var valorAsegurado = model.RangosGrupo.Sum(x => x.ValorAsegurado);
                    var numeroAsegurados = model.RangosGrupo.Sum(x => x.NumeroAsegurados);

                    args.ValorAsegurado = valorAsegurado;
                    args.NumeroAsegurados = numeroAsegurados;
                    args.Rangos = model.RangosGrupo;
                }
            }
            else
            {
                // Asigne rangos preconfigurados en sise segun informacion de la lista de asegurados
                await this.AsignarRangosAsync(args);
            }
        }

        private async Task CrearRangosAsync(IEnumerable<Rango> rangos)
        {
            foreach (var rango in rangos)
            {
                var responseRango = await rangosWriter.CrearRangoGrupoAseguradoAsync(rango);
            }
        }

        private async Task AsignarRangosAsync(InsertarAseguradosArgs args)
        {
            if (args.ConListaAsegurados)
            {
                var informacionRangosEdad = await this.rangosUtilities.ObtenerRangosPerfilEdadAsync(args.CodigoPerfilEdad);
                var informacionRangosValor = await this.rangosUtilities.ObtenerRangosPerfilValorAsync(args.CodigoPerfilValor);

                if (informacionRangosEdad != null)
                {
                    // calcule rangos de edades y valores según perfiles seleccioandos
                    var rangosEdades = this.rangosUtilities.ConfigurarRangosEdades(args.CodigoGrupoAsegurados, args.Asegurados, informacionRangosEdad.Rangos);
                    await this.CrearRangosAsync(rangosEdades);
                    args.Rangos = rangosEdades;
                }

                if (informacionRangosValor != null)
                {
                    var rangosValores = this.rangosUtilities.ConfigurarRangosValores(args.CodigoGrupoAsegurados, args.Asegurados, informacionRangosValor.Rangos);
                    // TODO validar si debe guardar configuracion de rangos de valores
                    //await this.CrearRangosAsync(rangosValores);
                }
            }
        }

        public async Task<InsertarBloqueAseguradosResponse> InsertarAseguradosFromFileAsync(int codigoCotizacion, InsertarAseguradosArgs args)
        {
            if (args.Asegurados.Count() == 0)
            {
                return InsertarBloqueAseguradosResponse.CreateValidEmpty();
            }

            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                args.ConListaAsegurados = informacionNegocio.ConListaAsegurados;
                // Insertar asegurados en DB
                var insertarAseguradosResponse = await this.aseguradoWriterService.InsertarBloqueAseguradosAsync(args.Asegurados.ToList(), args.CodigoGrupoAsegurados);
                args.EdadPromedioAsegurados = insertarAseguradosResponse.EdadPromedio;
                // Consultar asegurados
                var asegurados = await this.aseguradorReaderService.LeerAseguradosAsync(args.CodigoGrupoAsegurados);
                args.Asegurados = DatosGrupoAseguradosUtilities.ConvertToAsegurados(asegurados);
                // Insertar asegurados en SISE
                args.CodigoPerfilValor = informacionNegocio.CodigoPerfilValor;
                args.CodigoPerfilEdad = informacionNegocio.CodigoPerfilEdad;
                //Posible filtro de asegurados vetados SISE
                args.Asegurados = args.Asegurados.Where(x => !x.VetadoSarlaft).ToList();
                await this.InsertarAseguradosSISEAsync(args);
                // update cotizacion modified flag to true
                await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
                return new InsertarBloqueAseguradosResponse
                { 
                    TotalAsegurados = insertarAseguradosResponse.TotalAsegurados,
                    EdadPromedio = insertarAseguradosResponse.EdadPromedio,
                    RegistrosDuplicados = insertarAseguradosResponse.RegistrosDuplicados,
                    RegistrosProcesados = insertarAseguradosResponse.RegistrosProcesados
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"CotizacionPersonasWriter :: InsertarAseguradosFromFileAsync {ex}", ex);
            }
        }

        private async Task<ActionResponseBase> InsertarAseguradosSISEAsync(InsertarAseguradosArgs args)
        {
            try
            {
                var codigoCotizacion = args.CodigoCotizacion;
                var version = args.Version;
                // Consultar amparos
                var response = await gruposAseguradosMapper.ConsultarGrupoAseguradoAsync(codigoCotizacion, version, args.CodigoGrupoAsegurados);
                var amparoBasicoNoAdicional = await this.ObtenerAmparoBasicoNoAdicionalAsync(args.CodigoRamo, args.CodigoSubRamo, args.CodigoSector, response.AmparosGrupo);
                // Agrege los registro de asegurados por opcion
                this.AggregateAseguradosConValorAsegurado(amparoBasicoNoAdicional, args);

                // Crear objeto con valores de ejecución del SP de SISE
                // Consultar amparos
                // Objeto tipo SISEInsertar
                var SISEListadoAseguradosArgs = new SISEListadoAseguradosArgs
                {
                    Amparos = response.AmparosGrupo,
                    CodigoCotizacion = args.CodigoCotizacion,
                    CodigoGrupoAsegurados = args.CodigoGrupoAsegurados,
                    CodigoRamo = args.CodigoRamo,
                    CodigoSubramo = args.CodigoSubRamo,
                    CodigoSector = args.CodigoSector,
                    CodigoTipoSumaAsegurada = args.CodigoTipoSumaAsegurada,
                    CodigoTipoTasa = args.CodigoTipoTasa,
                    ConListaAsegurados = args.ConListaAsegurados,
                    VersionCotizacion = 1,
                    Asegurados = args.Asegurados,
                    ValorAsegurado = args.ValorAsegurado,
                    NumeroAsegurados = args.NumeroAsegurados,
                    Rangos = args.Rangos,
                    CodigoPerfil1 = args.CodigoPerfilEdad,
                    CodigoPerfil2 = args.CodigoPerfilValor,
                    EdadPromedio = args.EdadPromedioAsegurados,
                    TipoEstructura = args.TipoEstructura
                };

                // Limpiar registros relacionados en SISE 
                this.SISEWriterService.RemoverAseguradosAsync(SISEListadoAseguradosArgs).Wait();
                if (args.CodigoTipoTasa == 3 && args.Rangos.Count() == 0 && !args.ConListaAsegurados)
                {
                    throw new Exception("El perfil de edades no puede estar vacio si la tasa seleccionada es Tasa por rango edades.");
                }

                // Insertar asegurados en SISE
                var responseSISE = await this.SISEWriterService.InsertarAseguradosAsync(SISEListadoAseguradosArgs);
                return new ActionResponseBase
                {
                    Status = ResponseStatus.Valid,
                    ErrorMessage = responseSISE.Message
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"CotizacionPersonasWriter :: InsertarAseguradosSISEAsync.", ex);
            }
        }

        private async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, ICollection<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }

        private void AggregateAseguradosConValorAsegurado(AmparoGrupoAsegurado amparo, InsertarAseguradosArgs args)
        {
            if (args.Asegurados.Count() == 0 && !args.ConListaAsegurados)
            {
                args.Asegurados.Add(new Asegurado
                {
                    NumeroDocumento = "",
                    Edad = args.EdadPromedioAsegurados,
                    //ValorAsegurado = (args.NumeroAsegurados * opcion.ValorAsegurado)
                    ValorAsegurado = args.ValorAsegurado
                });
            }
        }

        private void AggregateAseguradosPorRango(InsertarAseguradosArgs args)
        {
            var rangoIdx = 0;
            foreach (var rango in args.Rangos)
            {
                args.Asegurados.Add(new Asegurado
                {
                    NumeroDocumento = rangoIdx.ToString(),
                    Edad = this.CalcularPromedioEdadRango(rango),
                    ValorAsegurado = rango.ValorAsegurado
                });

                rangoIdx++;
            }
        }

        private int CalcularPromedioEdadRango(Rango rango)
        {
            return rango.EdadMinAsegurado + rango.EdadMaxAsegurado / 2;
        }

        private void AggregateAsegurados(AmparoGrupoAsegurado amparo, InsertarAseguradosArgs args)
        {
            if (args.Asegurados.Count() == 0 && !args.ConListaAsegurados)
            {
                args.Asegurados.Add(new Asegurado
                {
                    NumeroDocumento = "",
                    Edad = args.EdadPromedioAsegurados
                });
            }
        }
    }
}
