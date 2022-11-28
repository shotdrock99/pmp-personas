using ModernizacionPersonas.Api.Providers;
using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class ExpedicionWebProvider
    {
        private ExpedicionWebBuilderService expedicionWebBuilderService;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly ResumenCotizacionProvider resumenCotizacionProvider;
        private readonly FichaTecnicaDataProvider fichaTecnicaDataProvider;
        private readonly SlipDataProvider slipDataProvider;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly IEmailSender emailSender;
        private readonly CotizacionStateWriter cotizacionStateUpdater;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly EmailsDataProvider emailsDataProvider;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private string basePath = "";



        public ExpedicionWebProvider()
        {
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.resumenCotizacionProvider = new ResumenCotizacionProvider();
            this.fichaTecnicaDataProvider = new FichaTecnicaDataProvider();
            this.slipDataProvider = new SlipDataProvider();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
            this.tomadorReader = new DatosTomadorTableReader();
            this.emailSender = new SolidariaExchangeEmailSender();
            this.cotizacionStateUpdater = new CotizacionStateWriter();
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.emailsDataProvider = new EmailsDataProvider();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";
        }

        public async Task<ActionResponseBase> GenerateExpedicionAsync(int codigoCotizacion, ExpedicionArgs expedicionArgs, string userName)
        {
            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                var resumen = await this.resumenCotizacionProvider.GenerateAsync(codigoCotizacion, informacionNegocio.Version);
                var fichaTecnica = await this.fichaTecnicaDataProvider.GenerateAsync(codigoCotizacion, informacionNegocio.LastAuthorName);
                var slip = await this.slipDataProvider.GenerateSlipAsync(codigoCotizacion, informacionNegocio.Version, informacionNegocio.LastAuthorName);

                var expedicionWeb = new ExpedicionWeb
                {
                    InformacionNegocio = informacionNegocio,
                    Resumen = resumen.Data,
                    FichaTecnica = fichaTecnica.Data,
                    Slip = slip.Data
                };

                this.expedicionWebBuilderService = new ExpedicionWebBuilderService(expedicionWeb);
                await this.expedicionWebBuilderService.GenerateExpedicion();

                await this.SendExpedicionAsync(expedicionArgs, informacionNegocio);

                await this.cotizacionStateUpdater.UpdateCotizacionStateAsync(informacionNegocio.CodigoCotizacion, CotizacionState.ExpeditionRequest);

                await this.cotizacionTransactionsProvider.CreateTransactionAsync(codigoCotizacion, informacionNegocio.Version, userName, "Solicitud Expedicón");

                return new ActionResponseBase() { 
                    
                    CodigoCotizacion = informacionNegocio.CodigoCotizacion,
                    Version = informacionNegocio.Version
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"ExpedicionWebProvider :: GenerateExpedicionAsync {ex.Message}");
            }
        }

        public async Task SendExpedicionAsync(ExpedicionArgs expedicionArgs, InformacionNegocio informacionNegocio)
        {
            try
            {
                var codigoCotizacion = informacionNegocio.CodigoCotizacion;
                int numeroCotizacion = int.Parse(informacionNegocio.NumeroCotizacion.TrimStart(new char[] { '0' }));
                var version = informacionNegocio.Version;

                var tomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
                var nombreTomador = $"{tomador.Nombres} {tomador.PrimerApellido}";

                var ramo = await this.informacionPersonasReader.TraerRamoAsync(informacionNegocio.CodigoRamo);

                var sucursal = await this.informacionPersonasReader.TraerSucursalAsync(informacionNegocio.CodigoSucursal);
                var directorTecnico = await this.soligesproUsersDataReader.GetUserDirectorTecnicoAsync(sucursal.CodigoSucursal);
                string[] cc = { directorTecnico.EmailUsuario };

                var templateName = "send_expedicion_template.html";
                var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(9, 2);

                var data = new
                {
                    bodyTemplate = bodyTemplate.Texto,
                    comments = expedicionArgs.Observaciones
                };

                var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
                var subject = $"SOLICITUD EXPEDICIÓN COTIZACIÓN Nro. { Convert.ToInt32(numeroCotizacion)} Versión {version} - TOMADOR: {nombreTomador} - RAMO: {ramo.NombreRamo}";

                var attachments = new List<string>();
                var expedicionPath = $@"{this.basePath}\{codigoCotizacion}\Expedición\{numeroCotizacion}_ExpedicionSISE.xlsx";
                var slipPath = $@"{this.basePath}\{codigoCotizacion}\Slip_Cotización_#{numeroCotizacion + "VR"+ version}.pdf";
                attachments.Add(slipPath);
                attachments.Add(expedicionPath);

                var sendEmailArgs = new SendEmailArgs
                {
                    Subject = subject,
                    Body = body,
                    Recipients = expedicionArgs.To,
                    CC = cc,
                    Attachments = attachments
                };

                await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
            }
            catch (Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
