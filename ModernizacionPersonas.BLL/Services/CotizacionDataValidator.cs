using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.SISEServices;
using ModernizacionPersonas.DAL.Entities.SISEEntities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.SISEServices;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionDataValidator
    {
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private readonly IAuthorizationsDataWriter authorizationsWriter;
        private readonly IAuthorizationsDataReader authorizationsReader;
        private readonly IAuthorizationUsersDataWriter authorizationUsersWriter;
        private readonly IDatosInformacionNegocioReader informacionNegocioReaderService;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosAmparoGrupoAseguradoReader amparosReader;
        private readonly IDatosTasaOpcionReader tasaOpcionesReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValoresReader;

        private readonly SISEAuthorizationsProcessor authorizationsProcessor;
        private readonly SISEAuthorizationDataProvider siseAuthorizationsDataProvider;
        private readonly CotizacionDataProvider cotizacionDataProvider;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly FichaTecnicaDataProvider fichaTecnica;

        public CotizacionDataValidator()
        {
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.authorizationsProcessor = new SISEAuthorizationsProcessor();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.authorizationsWriter = new AuthorizationsDataTableWriter();
            this.authorizationsReader = new AuthorizationsDataTableReader();
            this.authorizationUsersWriter = new AuthorizationsUsersDataTableWriter();
            this.siseAuthorizationsDataProvider = new SISEAuthorizationDataProvider();
            this.informacionNegocioReaderService = new DatosInformacionNegocioTableReader();
            this.cotizacionDataProvider = new CotizacionDataProvider();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.amparosReader = new DatosAmparoGrupoAseguradoTableReader();
            this.tasaOpcionesReader = new DatosTasaOpcionTableReader();
            this.opcionValoresReader = new DatosOpcionValorAseguradoTableReader();
            this.fichaTecnica = new FichaTecnicaDataProvider();
        }

        public async Task<CotizacionValidationResponse> ValidateAsync(string userName, int codigoCotizacion, int version, int flag)
        {
            try
            {
                // fetch infoNegocio
                var informacionNegocio = await this.informacionNegocioReaderService.LeerInformacionNegocioAsync(codigoCotizacion);
                var NumCot = int.Parse(informacionNegocio.NumeroCotizacion);


                // insert SISE authorization data
                await this.siseAuthorizationsDataProvider.InsertAuthorizationData(userName, codigoCotizacion, version);
                var authorizationsExist = await this.GetAuthorizationControls(codigoCotizacion);

                
                
               
                if (userName == informacionNegocio.LastAuthorName)
                {
                    if ((flag == 1 && authorizationsExist.Count() > 0) || flag == 2)
                    {
                        // get authorization from SISE process
                        var siseValidation = await this.GetValidationsAsync(NumCot, version);
                        // save sise authorization values
                        await this.SaveAuthorizationsAsync(codigoCotizacion, siseValidation.Authorizations);
                        // save web authorizations
                        await authorizationsWriter.SaveValidationsWEBAsync(codigoCotizacion, NumCot, version);
                        var authorizationsOld = await this.GetAuthorizationControls(codigoCotizacion);

                        // Validate Modalidad Auth
                        var listaAutorizaciones = new List<CotizacionAuthorization>();
                        var grupos = this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion).Result;
                        var flagMod = false;

                    
                            foreach (var gru in grupos)
                            {
                                var amparos = amparosReader.LeerAmparoGrupoAseguradoAsync(gru.CodigoGrupoAsegurado).Result;
                                foreach (var amp in amparos)
                                {
                                    var ampInfo = this.informacionPersonasReader.TraerAmparoxCodigoAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, amp.CodigoAmparo, informacionNegocio.CodigoSector).Result;
                                    amp.AmparoInfo = ampInfo;

                                    var opciones = this.opcionValoresReader.LeerOpcionValorAseguradoAsync(amp.CodigoAmparoGrupoAsegurado).Result;
                                    if (gru.ConDistribucionAsegurados)
                                    {
                                        gru.AseguradosOpcion1 = opciones.Where(x => x.IndiceOpcion == 1).FirstOrDefault().NumeroAsegurados;
                                        gru.AseguradosOpcion2 = opciones.Where(x => x.IndiceOpcion == 2).FirstOrDefault().NumeroAsegurados;
                                        gru.AseguradosOpcion3 = opciones.Where(x => x.IndiceOpcion == 3).FirstOrDefault().NumeroAsegurados;
                                    }

                                    if (amp.AmparoInfo.Modalidad.Codigo == 4)
                                    {
                                        foreach (var op in opciones)
                                        {
                                            if (op.NumeroDias > (decimal)amp.AmparoInfo.Modalidad.Valores[1].Valor)
                                            {
                                                var authorizationDias = new CotizacionAuthorization
                                                {
                                                    CodigoCotizacion = NumCot,
                                                    CampoEntrada = "dias",
                                                    CodigoRamo = informacionNegocio.CodigoRamo,
                                                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                                                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                                                    CodigoAmparo = amp.CodigoAmparo,
                                                    CodigoGrupoAsegurado = gru.CodigoGrupoAsegurado,
                                                    CodigoTipoAutorizacion = 2,
                                                    Version = version,
                                                    CodigoUsuario = userName,
                                                    RequiereAutorizacion = true,
                                                    ValorEntrada = op.NumeroDias,
                                                    MensajeValidacion = $"La variable numero de días: {op.NumeroDias.ToString("0.####")} supera el tope de {amp.AmparoInfo.Modalidad.Valores[1].Valor} ",
                                                    NombreSeccion = $"{amp.AmparoInfo.NombreAmparo}"

                                                };

                                                listaAutorizaciones.Add(authorizationDias);

                                            }

                                            if (op.ValorDiario > (decimal)amp.AmparoInfo.Modalidad.Valores[0].Valor)
                                            {
                                                var authorizacionValorDiario = new CotizacionAuthorization
                                                {
                                                    CodigoCotizacion = NumCot,
                                                    CampoEntrada = "valor_diario",
                                                    CodigoRamo = informacionNegocio.CodigoRamo,
                                                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                                                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                                                    CodigoAmparo = amp.CodigoAmparo,
                                                    CodigoGrupoAsegurado = gru.CodigoGrupoAsegurado,
                                                    CodigoTipoAutorizacion = 2,
                                                    Version = version,
                                                    CodigoUsuario = userName,
                                                    ValorEntrada = op.ValorDiario,
                                                    RequiereAutorizacion = true,
                                                    MensajeValidacion = $"La variable valor diario: {op.ValorDiario.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))} supera el tope de {amp.AmparoInfo.Modalidad.Valores[0].Valor.ToString("C0", System.Globalization.CultureInfo.GetCultureInfo("en-US"))} ",
                                                    NombreSeccion = $"{amp.AmparoInfo.NombreAmparo}"
                                                };

                                                listaAutorizaciones.Add(authorizacionValorDiario);

                                            }

                                        }

                                    }
                                    else if (amp.AmparoInfo.Modalidad.Codigo == 2) 
                                    {
                                        foreach (var op in opciones)
                                        {
                                            if (op.PorcentajeCobertura > (decimal)amp.AmparoInfo.Modalidad.Valores[0].Valor)
                                            {
                                                var authorizationDias = new CotizacionAuthorization
                                                {
                                                    CodigoCotizacion = NumCot,
                                                    CampoEntrada = "porcentaje_amparo",
                                                    CodigoRamo = informacionNegocio.CodigoRamo,
                                                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                                                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                                                    CodigoAmparo = amp.CodigoAmparo,
                                                    CodigoGrupoAsegurado = gru.CodigoGrupoAsegurado,
                                                    CodigoTipoAutorizacion = 2,
                                                    Version = version,
                                                    CodigoUsuario = userName,
                                                    RequiereAutorizacion = true,
                                                    ValorEntrada = op.PorcentajeCobertura,
                                                    MensajeValidacion = $"El porcentaje del {op.PorcentajeCobertura.ToString("0.####")}% excede el valor límite de {amp.AmparoInfo.Modalidad.Valores[0].Valor}% ",
                                                    NombreSeccion = $"{amp.AmparoInfo.NombreAmparo}"

                                                };

                                                listaAutorizaciones.Add(authorizationDias);

                                            }

                                        }
                                    }
                                    else if (amp.AmparoInfo.Modalidad.Codigo == 3 ) 
                                    {
                                        foreach (var op in opciones)
                                        {
                                            if (op.ValorAsegurado > (decimal)amp.AmparoInfo.Modalidad.Valores[0].Valor)
                                            {
                                                var authorizationDias = new CotizacionAuthorization
                                                {
                                                    CodigoCotizacion = NumCot,
                                                    CampoEntrada = "valor_asegurado",
                                                    CodigoRamo = informacionNegocio.CodigoRamo,
                                                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                                                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                                                    CodigoAmparo = amp.CodigoAmparo,
                                                    CodigoGrupoAsegurado = gru.CodigoGrupoAsegurado,
                                                    CodigoTipoAutorizacion = 2,
                                                    Version = version,
                                                    CodigoUsuario = userName,
                                                    RequiereAutorizacion = true,
                                                    ValorEntrada = op.ValorAsegurado,
                                                    MensajeValidacion = $"El Valor Asegurado de {String.Format("{0:C}", op.ValorAsegurado)} excede el valor limite de { String.Format("{0:C}", amp.AmparoInfo.Modalidad.Valores[0].Valor)} ",
                                                    NombreSeccion = $"{amp.AmparoInfo.NombreAmparo}"

                                                };

                                                listaAutorizaciones.Add(authorizationDias);

                                            }

                                        }
                                    }
                                    else if (amp.AmparoInfo.Modalidad.Codigo ==  5 )
                                    {
                                        foreach (var op in opciones)
                                        {
                                            var tiposSumaAsegurada =  await this.informacionPersonasReader.TraerTiposSumaAsegurada(15, informacionNegocio.CodigoSubramo);
                                            var salario = tiposSumaAsegurada.Where(x => x.CodigoTipoSumaAsegurada == 10).FirstOrDefault().ValorSalarioMinimo;

                                            var valorAmparo = op.ValorAsegurado / salario;
                                            if (valorAmparo > (decimal)amp.AmparoInfo.Modalidad.Valores[0].Valor)
                                            {
                                                var authorizationDias = new CotizacionAuthorization
                                                {
                                                    CodigoCotizacion = NumCot,
                                                    CampoEntrada = "valor_amparo",
                                                    CodigoRamo = informacionNegocio.CodigoRamo,
                                                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                                                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                                                    CodigoAmparo = amp.CodigoAmparo,
                                                    CodigoGrupoAsegurado = gru.CodigoGrupoAsegurado,
                                                    CodigoTipoAutorizacion = 2,
                                                    Version = version,
                                                    CodigoUsuario = userName,
                                                    RequiereAutorizacion = true,
                                                    ValorEntrada = op.ValorAsegurado,
                                                    MensajeValidacion = $"El Valor Asegurado de {String.Format("{0:C}", op.ValorAsegurado)} excede el valor limite de { String.Format("{0}", amp.AmparoInfo.Modalidad.Valores[0].Valor)} SMMLV ",
                                                    NombreSeccion = $"{amp.AmparoInfo.NombreAmparo}"

                                                };

                                                listaAutorizaciones.Add(authorizationDias);

                                            }

                                        }
                                    }
                                }
                                
                            }

                        foreach (var auth in listaAutorizaciones)
                        {
                            await this.authorizationsWriter.SaveAuthorizationAsync(auth);
                        }


                        // save authorization users
                        var users = await this.SaveAuthorizationUsersAsync(codigoCotizacion, siseValidation.Users);
                        // fetch authorizations list

                        var authorizations = await this.GetAuthorizationControls(codigoCotizacion);
                        //authorizations = await this.GetAuthorizationControls(codigoCotizacion);

                        // build validacion result
                        var validation = this.BuildValidationResult(authorizations, users, siseValidation.ValidationMessage);
                        // fetch tasas cotizacion
                        var tasasResponse = await this.cotizacionDataProvider.FetchTasasAsync(codigoCotizacion, version);
                        // update cotizacion state          
                        var requireAuthorization = validation.Authorizations.Count() > 0;



                        // si la cotizacion no tiene controles de validacion cambie el estado a aprobada, de lo contrario a validada
                        if (requireAuthorization)
                        {
                            //if (informacionNegocio.CotizacionChanged && (CotizacionState)informacionNegocio.CodigoEstadoCotizacion != CotizacionState.ApprovedAuthorization)
                            //{
                            //    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                            //    informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                            //}
                            //else if (informacionNegocio.CotizacionChanged && (CotizacionState)informacionNegocio.CodigoEstadoCotizacion == CotizacionState.ApprovedAuthorization)
                            //{
                            //    if (!informacionNegocio.SelfAuthorize)
                            //    {
                            //        await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                            //        informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                            //    }
                            //}
                            informacionNegocio = await this.informacionNegocioReaderService.LeerInformacionNegocioAsync(codigoCotizacion);

                            if (informacionNegocio.CotizacionChanged)
                            {
                                if ((CotizacionState)informacionNegocio.CodigoEstadoCotizacion != CotizacionState.ApprovedAuthorization)
                                {
                                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                                    informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                                }
                                else
                                {
                                    if (!informacionNegocio.SelfAuthorize)
                                    {
                                        await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                                        informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                                    }
                                }
                            }
                            /*else
                            {
                                if ((CotizacionState)informacionNegocio.CodigoEstadoCotizacion != CotizacionState.ApprovedAuthorization)
                                {
                                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                                    informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                                }
                                else
                                {
                                    if (!informacionNegocio.SelfAuthorize)
                                    {
                                        await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.Validated);
                                        informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.Validated;
                                    }
                                }
                            }*/
                        }
                        else
                        {
                            if ((CotizacionState)informacionNegocio.CodigoEstadoCotizacion <= CotizacionState.ApprovedAuthorization)
                            {
                                await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.ApprovedAuthorization);
                                informacionNegocio.CodigoEstadoCotizacion = (int)CotizacionState.ApprovedAuthorization;
                            }
                        }

                        // Update modified flag after pass authorization controls
                        await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, false);

                        var isApproved = !informacionNegocio.CotizacionChanged && informacionNegocio.CotizacionState >= CotizacionState.ApprovedAuthorization;

                        return new CotizacionValidationResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = informacionNegocio.Version,
                            CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                            NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                            // TODO check SISE reponse 0 auth isValid=false, message
                            IsValid = siseValidation.ValidationMessage.IsValid || validation.Authorizations.Count() == 0,
                            Validation = validation,
                            Tasas = tasasResponse.Tasas,
                            RequireAuthorization = requireAuthorization
                        };
                    }
                    else
                    {
                        return new CotizacionValidationResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = informacionNegocio.Version,
                            CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                            NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                            // TODO check SISE reponse 0 auth isValid=false, message
                            IsValid = true,
                            Validation = new CotizacionValidation(),
                            Tasas = new List<CotizacionTasa>(),
                            RequireAuthorization = false
                        };
                    }

                }
                else
                {
                    return new CotizacionValidationResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Version = informacionNegocio.Version,
                        CodigoEstadoCotizacion = informacionNegocio.CodigoEstadoCotizacion,
                        NumeroCotizacion = informacionNegocio.NumeroCotizacion,
                        // TODO check SISE reponse 0 auth isValid=false, message
                        IsValid = true,
                        Validation = new CotizacionValidation(),
                        Tasas = new List<CotizacionTasa>(),
                        RequireAuthorization = false
                    };
                }


            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataValidator :: ValidateAsync", ex);
            }
        }

        private async Task<IEnumerable<CotizacionAuthorization>> GetAuthorizationControls(int codigoCotizacion)
        {
            var infoNegocio = this.informacionNegocioReaderService.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            var numCot = int.Parse(infoNegocio.NumeroCotizacion);
            var response = await this.authorizationsReader.GetAuthorizationsByCotizacionAsync(numCot, infoNegocio.Version);
            return response.Authorizations;
        }

        private CotizacionValidation BuildValidationResult(IEnumerable<CotizacionAuthorization> authorizations, IEnumerable<AuthorizationUser> users, AuthorizationValidationMessage validationMessage)
        {
            var result = new CotizacionValidation();

            var ageAuthorizations = authorizations.Where(x => x.CampoEntrada.Contains("edad_"));
            var groupedAgeAuthorizations = ageAuthorizations.GroupBy(x => x.CodigoGrupoAsegurado);
            var notAgeAuthorizations = authorizations.Except(ageAuthorizations);

            //result.Authorizations = notAgeAuthorizations;
            result.Authorizations = authorizations;
            result.Users = users;
            result.ValidationMessage = validationMessage;

            return result;
        }

        private async Task<IEnumerable<AuthorizationUser>> SaveAuthorizationUsersAsync(int codigoCotizacion, IEnumerable<AuthorizationUser> usersSise)
        {
            var infoNegocio = this.informacionNegocioReaderService.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            var numCot = int.Parse(infoNegocio.NumeroCotizacion);
            var auth = this.authorizationsReader.GetAuthorizationsByCotizacionAsync(numCot, infoNegocio.Version).Result;

            var authWeb = auth.Authorizations.Where(x => x.CodigoTipoAutorizacion == 2);
            // remove authorization users
            await this.authorizationUsersWriter.RemoveAuthorizationUsersAsync(numCot, infoNegocio.Version);
            // fetch web authorization users
            var webUsers = await siseAuthorizationsDataProvider.GetWebAuthorizationUsersAsync(numCot, infoNegocio.Version);            

            if (authWeb.Count() > 0)
            {
                usersSise = usersSise.Where(x => !x.Notificado );
            }
            // distinct pasive users    
            var pasiveUsers = usersSise.Where(x => !x.Activo);

            

            //var distinctUsers = usersSise.Count() > 0 ? usersSise.Except(finalWebUser) : finalWebUser;
            //distinctUsers = distinctUsers.Union(webUsers);
            var respClausulas = await this.authorizationUsersWriter.GetClausulasEspAsync(codigoCotizacion);
            if (respClausulas > 0)
            {
                var espUsers = await siseAuthorizationsDataProvider.GetEspAuthorizationUsersAsync(numCot, infoNegocio.Version);
                pasiveUsers = pasiveUsers.Union(espUsers);
            }

            var finalWebUser = webUsers.Union(pasiveUsers.Except(webUsers));

            // define final users and aggrgate pasive users defined by SISE validation and special clauses definition
            var finalUsers = authWeb.Count() > 0 ? pasiveUsers.Union(finalWebUser) : usersSise;

            foreach (var user in finalUsers)
            {
                // save authorization users
                await this.authorizationUsersWriter.SaveAuthorizationUserAsync(user);
            }

            return finalUsers.Except(pasiveUsers);
        }

        private async Task SaveAuthorizationsAsync(int codigoCotizacion, IEnumerable<CotizacionAuthorization> authorizations)
        {
            var infoNegocio = this.informacionNegocioReaderService.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            var numCot = int.Parse(infoNegocio.NumeroCotizacion);
            // delete authorizations to update values
            await this.authorizationsWriter.DeleteAuthorizationAsync(numCot, infoNegocio.Version);
            foreach (var authorization in authorizations)
            {
                await this.authorizationsWriter.SaveAuthorizationAsync(authorization);
            }
        }

        private async Task<CotizacionValidation> GetValidationsAsync(int codigoCotizacion, int version)
        {
            var validationResponse = await this.authorizationsProcessor.ProcessAsync(codigoCotizacion, version);
            await this.AggregateGruposAseguradosInfo(codigoCotizacion, version, validationResponse.Authorizations);
            var validation = new CotizacionValidation
            {
                ValidationMessage = validationResponse.ValidationMessage,
                Authorizations = validationResponse.Authorizations,
                Users = validationResponse.AuthorizationUsers
            };

            return validation;
        }

        private async Task<IEnumerable<CotizacionAuthorizationDTO>> AggregateGruposAseguradosInfo(int codigoCotizacion, int version, List<CotizacionAuthorization> authorizations)
        {
            var result = new List<CotizacionAuthorizationDTO>();
            var groups = await this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);
            foreach (var item in authorizations)
            {
                var authorization = new CotizacionAuthorizationDTO
                {
                    CampoEntrada = item.CampoEntrada,
                    CodigoAmparo = item.CodigoAmparo,
                    CodigoAutorizacion = item.CodigoAutorizacion,
                    CodigoCotizacion = item.CodigoCotizacion,
                    CodigoGrupoAsegurado = item.CodigoGrupoAsegurado,
                    CodigoRamo = item.CodigoRamo,
                    CodigoSubramo = item.CodigoSubramo,
                    CodigoSucursal = item.CodigoSucursal,
                    CodigoTipoAutorizacion = item.CodigoTipoAutorizacion,
                    CodigoUsuario = item.CodigoUsuario,
                    MensajeValidacion = item.MensajeValidacion,
                    NombreGrupoAsegurado = "",
                    RequiereAutorizacion = item.RequiereAutorizacion,
                    ValorEntrada = item.ValorEntrada,
                    Version = item.Version
                };

                var group = groups.Where(x => x.CodigoGrupoAsegurado == item.CodigoGrupoAsegurado).FirstOrDefault();
                if (group != null)
                {
                    authorization.NombreGrupoAsegurado = group.NombreGrupoAsegurado;
                }

                result.Add(authorization);
            }

            return result;
        }
    }
}
