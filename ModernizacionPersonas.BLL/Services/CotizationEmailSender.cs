using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ParametrizacionServiceReference;
using PersonasServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    class CotizationEmailSender
    {
        private readonly IDatosPersonasReader datosPersonasReader;
        private readonly IDatosTomadorReader datosTomadorReader;
        private readonly IEmailSender emailSender;
        private readonly EmailsDataProvider emailsDataProvider;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private string basePath = "";

        public CotizationEmailSender()
        {
            this.datosPersonasReader = new InformacionPersonasReader();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.emailSender = new SolidariaExchangeEmailSender();
            this.emailsDataProvider = new EmailsDataProvider();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";
        }

        public async Task SendRefuzedCotizationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string[] withCopy, string comment, string causal, ConfirmCotizacionAction cotizacionAction, int version)
        {
            var agenciaNombre = await this.GetAgencia(codigoCotizacion);
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "refused_cotization_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(5, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment,
                causalTexto = causal,
                tipoRechazo = cotizacionAction.ToString() == "RejectedByClient" ? "Rechazo: Por Cliente" : "Rechazo: Por Compañía",
                agencia = agenciaNombre.NombreSucursal
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cco = new string[] { "ext.sruiz@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"NO ACEPTACION CONDICIONES DE COTIZACIÓN Nro. { Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR - {args.NombreTomador} - RAMO {args.DescripcionRamo}";
            var directoryPath = Path.Combine(this.basePath, $"attachments\\{codigoCotizacion}");
            var attachments = new List<string>();
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    attachments.Add(file);
                }
            }
            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = withCopy,
                //CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
        }

        public async Task SendApprovedCotizationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string[] withCopy, string comment, string causal, int version)
        {
            var agenciaNombre = await this.GetAgencia(codigoCotizacion);
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "approved_cotization_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(2, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment,
                causalTexto = causal,
                agencia = agenciaNombre.NombreSucursal
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co", "ext.sruiz@solidaria.com.co" };
            var cco = new string[] { "ext.sruiz@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"ACEPTACIÓN DE COTIZACIÓN Nro. {Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR - {args.NombreTomador} - RAMO {args.DescripcionRamo}";
            var directoryPath = Path.Combine(this.basePath, $"attachments\\{codigoCotizacion}");
            var attachments = new List<string>();
            if (Directory.Exists(directoryPath))
            {
                var files = Directory.GetFiles(directoryPath);
                foreach (var file in files)
                {
                    attachments.Add(file);
                }
            }
            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = withCopy,
                CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
        }

        public async Task<SendNotificacionEmailArgs> GetBodyArgs(int codigoCotizacion, int codigoRamo)
        {
            var tomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
            var nombreTomador = $"{tomador.Nombres} {tomador.PrimerApellido}";
            var ramo = await this.datosPersonasReader.TraerRamoAsync(codigoRamo);
            var args = new SendNotificacionEmailArgs
            {
                DescripcionRamo = ramo.NombreAbreviado,
                NombreTomador = nombreTomador
            };

            return args;
        }

        public async Task<PersonasServiceReference.Sucursal> GetAgencia(int codigoCotizacion)
        {
            var infoNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var sucursal = await this.datosPersonasReader.TraerSucursalAsync(infoNegocio.CodigoSucursal);
            return sucursal;
        }
    }
}
