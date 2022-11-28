using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class AuthorizationEmailSender
    {
        private readonly IDatosPersonasReader personasReaderService;
        private readonly IDatosEnvioSlipWriter datosEnvioSlipWriter;
        private readonly IDatosTomadorReader datosTomadorReader;
        private readonly IAuthorizationsDataReader authorizationsReader;
        private readonly IEmailSender emailSender;
        private readonly EmailsDataProvider emailsDataProvider;
        private readonly ISoligesproDataUsuariosReader soligesproDataUsuariosReader;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosEnvioSlipReader datosEnvioSlipReader;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private string basePath = "";

        public AuthorizationEmailSender()
        {
            this.datosEnvioSlipReader = new DatosEnvioSlipTableReader();
            this.datosEnvioSlipWriter = new DatosEnvioSlipTableWriter();
            this.personasReaderService = new InformacionPersonasReader();
            this.datosTomadorReader = new DatosTomadorTableReader();
            this.emailSender = new SolidariaExchangeEmailSender();
            this.authorizationsReader = new AuthorizationsDataTableReader();
            this.emailsDataProvider = new EmailsDataProvider();
            this.soligesproDataUsuariosReader = new SoligesproDataUsuariosReader();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";
        }

        public async Task SendNotificationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string comment, int version)
        {
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "notify_authorization_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(3, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment,
                controls = args.ControlesAutorizacion
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cc = await this.GetCopieTo(codigoCotizacion);
            var cco = new string[] { "ext.jmalagon@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"SOLICITUD AUTORIZACION DE COTIZACION Nro. { Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR – {args.NombreTomador} RAMO – {args.DescripcionRamo}";
            // Slip de cotización, Condicionado general del ramo
            var adjuntosAll = await this.datosEnvioSlipReader.LeerAdjuntoEnvioSlipAsync(codigoCotizacion);
            var attachments = new List<string>();
            List<AdjuntoEnvioSlip> adjuntosFinales = new List<AdjuntoEnvioSlip>();
            foreach (var adjunto in adjuntosAll)
            {
                if (adjunto.FileName.Contains("248699"))
                {
                    var attachmentPath = Path.Combine(basePath, codigoCotizacion.ToString(), adjunto.FileName);
                    attachments.Add(attachmentPath);
                    adjuntosFinales.Add(adjunto);
                }

            }

            await this.SaveTemporalAttachment(codigoCotizacion, adjuntosFinales);


            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = cc,
                //CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);

            var adjuntos = await this.datosEnvioSlipReader.LeerAdjuntoEnvioSlipAsync(codigoCotizacion);

            foreach (var adjunto in adjuntos)
            {
                if (adjunto.FileName.Contains("248699"))
                {
                    await this.datosEnvioSlipWriter.BorrarAdjuntoEnvioSlipByNamesAsync(codigoCotizacion, adjunto.FileName);
                }

            }


        }
        private async Task SaveTemporalAttachment(int codigoCotizacion, IEnumerable<AdjuntoEnvioSlip> adjuntos)
        {
            var envios = await this.datosEnvioSlipReader.LeerEnvioSlipAsync(codigoCotizacion);
            var last = envios.LastOrDefault();
            //var adjunto = adjuntos.LastOrDefault();

            var directoryPath = $@"{ this.basePath}\{ codigoCotizacion}";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var countFN = 1;

            foreach (var adjunto in adjuntos)
            {
                if (adjunto.FileName == null)
                {
                    adjunto.FileName = "Archivo No." + countFN.ToString();
                    countFN += 1;
                }
                var path = $@"{this.basePath}\{codigoCotizacion}\{adjunto.FileName}";
                using (MemoryStream ms = new MemoryStream(adjunto.Adjunto))
                using (var fileStream = File.Create(path))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    for (int i = 0; i < adjunto.Adjunto.Length; i++)
                    {
                        byte result = reader.ReadByte();
                    }

                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fileStream);
                }
            }
        }
        public async Task SendReviewAuthorizationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string comment, int version)
        {
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "return_review_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(6, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment,
                controls = args.ControlesAutorizacion
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cc = await this.GetCopieTo(codigoCotizacion);
            var cco = new string[] { "ext.jmalagon@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"AJUSTE DE COTIZACION Nro. { Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR – {args.NombreTomador} RAMO – {args.DescripcionRamo}";
            // Slip de cotización, Condicionado general del ramo
            var attachments = new List<string>();
            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = cc,
                //CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
        }

        public async Task SendRefusedAuthorizationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string comment, int version)
        {
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "refused_authorization_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(4, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cc = await this.GetCopieTo(codigoCotizacion);
            var cco = new string[] { "ext.jmalagon@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"NO AUTORIZACIÓN COTIZACIÓN Nro. { Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR – {args.NombreTomador} RAMO – {args.DescripcionRamo}";
            // Slip de cotización, Condicionado general del ramo
            var attachments = new List<string>();
            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = cc,
                //CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
        }

        public async Task SendApprovedAuthorizationEmail(int codigoCotizacion, string numeroCotizacion, int codigoRamo, string[] recipients, string comment, int version)
        {
            var args = await this.GetBodyArgs(codigoCotizacion, codigoRamo);
            var templateName = "approved_authorization_template.html";
            var bodyTemplate = await this.emailsDataProvider.GetTextosEmailByTemplate(1, 2);
            var data = new
            {
                bodyTemplate = bodyTemplate.Texto,
                comments = comment
            };

            //var cc = new string[] { "director-comercial@solidaria.com.co", "gerente-agencia@solidaria.com.co" };
            var cc = await this.GetCopieTo(codigoCotizacion);
            var cco = new string[] { "ext.jmalagon@solidaria.com.co" };
            var body = HtmlTemplateBuilder.BuildHtmlTemplate(data, templateName);
            var subject = $"AUTORIZACION DE COTIZACION Nro. {Convert.ToInt32(numeroCotizacion)} V{version} TOMADOR – {args.NombreTomador} RAMO – {args.DescripcionRamo}";
            // Slip de cotización, Condicionado general del ramo
            var attachments = new List<string>();
            var sendEmailArgs = new SendEmailArgs
            {
                Attachments = attachments,
                Body = body,
                CC = cc,
                //CCO = cco,
                Recipients = recipients,
                Subject = subject
            };

            await this.emailSender.SendEmailUsingTemplateAsync(sendEmailArgs);
        }

        private async Task<SendNotificacionEmailArgs> GetBodyArgs(int codigoCotizacion, int codigoRamo)
        {
            var tomador = await this.datosTomadorReader.GetTomadorAsync(codigoCotizacion);
            var nombreTomador = $"{tomador.Nombres} {tomador.PrimerApellido}";

            var infoNegocio = this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion).Result;
            var NumCot = int.Parse(infoNegocio.NumeroCotizacion);
            var version = infoNegocio.Version;

            var ramo = await this.personasReaderService.TraerRamoAsync(codigoRamo);
            var authorizationControlsResponse = await this.authorizationsReader.GetAuthorizationsByCotizacionAsync(NumCot,version);
            var controls = authorizationControlsResponse.Authorizations.ToList();
            var args = new SendNotificacionEmailArgs
            {
                ControlesAutorizacion = controls,
                DescripcionRamo = ramo.NombreAbreviado,
                NombreTomador = nombreTomador
            };

            return args;
        }

        private async Task<string[]> GetCopieTo(int codigoCotizacion)
        {
            var infoNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            List<string> withCopieTo = new List<string>();

            var dTecnico = await this.soligesproDataUsuariosReader.GetUserDirectorTecnicoAsync(infoNegocio.CodigoSucursal);
            var dComercial = await this.soligesproDataUsuariosReader.GetUserDirectorComercialAsync(infoNegocio.CodigoSucursal);
            var dtZonal = await this.soligesproDataUsuariosReader.GetUserDirectorZonalAsync(infoNegocio.CodigoZona);

            if (dTecnico.EmailUsuario != null)
            {
                withCopieTo.Add(dTecnico.EmailUsuario);
            }

            if (dComercial.Count() > 0)
            {
                withCopieTo.Add(dComercial.FirstOrDefault().EmailUsuario);
            }

            if (dtZonal.LoginUsuario != null)
            {
                withCopieTo.Add(dtZonal.EmailUsuario);
            }

            return withCopieTo.ToArray();
        }
    }
}
