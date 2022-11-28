using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EASendMail;
using UtilidadesSolidariaServiceReference;
using System.Net.Mail;

namespace ModernizacionPersonas.BLL.Services
{
    public class SolidariaExchangeEmailSender : IEmailSender
    {
        private const string FROM = "CotizadorSegurosdePersonas@solidaria.com.co";
        private const string EMAIL_USER = "CotizadorSegurosdePersonas";
        private const string EMAIL_PASSWORD = "Csp07122020*";

        private static AppConfiguration appConfiguration = new AppConfiguration();
        private readonly UtilidadesSolidariaService solidariaUtilities;

        public SolidariaExchangeEmailSender()
        {
            this.solidariaUtilities = new UtilidadesSolidariaService();
        }

        public async Task<SendEmailResponse> OldSendEmailUsingTemplateAsync(SendEmailArgs args)
        {
            var email = new Correo
            {
                De = FROM,
                Usuario = EMAIL_USER,
                Pwd = EMAIL_PASSWORD,
                Adjunto = args.Attachments.ToArray(),
                Asunto = args.Subject,
                CC = args.CC,
                CCO = args.CCO,
                Dominio = "",
                EnableSSL = false,
                Host = "",
                IsBodyHTML = BodyType.HTML,
                Para = args.Recipients,
                Puerto = 0,
                SnGrabaCopiaEnviado = false,
                Texto = args.Body
            };

            var response = await this.solidariaUtilities.SendEmailAsync(email);
            return new SendEmailResponse
            {
                CodigoEstado = response.CodigoEstado,
                DescripcionEstado = response.DescripcionEstado,
                RutaPDF = response.RutaPDF,
                HasError = response.SnError
            };
        }
        /*
        public async Task<bool> SendEmailUsingTemplateAsync(SendEmailArgs args)
        {
            
            try
            {
                
                SmtpMail oMail = new SmtpMail("TryIt");

                // Your Offic 365 email address
                oMail.From = FROM;
                // Set recipient email address
                foreach (string i in args.Recipients)
                {
                    oMail.To.Add(new MailAddress(i));
                }
                //oMail.To.Add(new MailAddress("davosanchez08541236@gmail.com"));
                
                if (args.Attachments.Count > 0) {
                    var adjuntos = args.Attachments.ToArray();
                    foreach (string i in adjuntos)
                    {
                        oMail.AddAttachment(i);
                    }
                }

                if (args.CC != null) {
                    foreach (string i in args.CC)
                    {
                        oMail.Cc.Add(new MailAddress(i));
                    } 
                }
                if (args.CCO != null) {
                    foreach (string i in args.CCO)
                    {
                        oMail.Bcc.Add(new MailAddress(i));
                    }
                }
                // Set email subject
                oMail.Subject = args.Subject;
                // Set email body
                oMail.HtmlBody = args.Body;
                
                // Your Office 365 SMTP server address,
                // You should get it from outlook web access.
                SmtpServer oServer = new SmtpServer("smtp.office365.com");

                // user authentication should use your
                // email address as the user name.
                oServer.User = FROM;

                // If you got authentication error, try to create an app password instead of your user password.
                // https://support.microsoft.com/en-us/account-billing/using-app-passwords-with-apps-that-don-t-support-two-step-verification-5896ed9b-4263-e681-128a-a6f2979a7944
                oServer.Password = EMAIL_PASSWORD;

                // Set 587 port
                oServer.Port = 587;

                // detect SSL/TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;


                SmtpClient oSmtp = new SmtpClient();
                 await oSmtp.SendMailAsync(oServer, oMail);

                return true;
            }
            catch (Exception ep)
            {
                throw new Exception("SolidariaExchangeEmailSender :: SendEmailUsingTemplateAsync", ep);
            }
        }*/
        public async Task<bool> SendEmailUsingTemplateAsync(SendEmailArgs args)
        {
            using (MailMessage msg = new MailMessage())
            {

                foreach (string i in args.Recipients)
                {
                    if (i != "")
                    {
                        msg.To.Add(new System.Net.Mail.MailAddress(i));
                    }
                }
                if (args.Attachments.Count > 0)
                {
                    var adjuntos = args.Attachments.ToArray();
                    foreach (string i in adjuntos)
                    {
                        if (i != "")
                        {
                            msg.Attachments.Add(new System.Net.Mail.Attachment(i));
                        }
                    }
                }

                if (args.CC != null)
                {
                    foreach (string i in args.CC)
                    {
                        if (i != "")
                        {
                            msg.CC.Add(new System.Net.Mail.MailAddress(i));
                        }
                    }
                }
                if (args.CCO != null)
                {
                    foreach (string i in args.CCO)
                    {
                        if (i != "")
                        {
                            msg.Bcc.Add(new System.Net.Mail.MailAddress(i));
                        }
                    }
                }
                msg.From = new System.Net.Mail.MailAddress(FROM, EMAIL_USER);
                msg.Subject = args.Subject;
                msg.Body = args.Body;
                msg.IsBodyHtml = true;
                msg.SubjectEncoding = System.Text.Encoding.UTF8;
                msg.BodyEncoding = System.Text.Encoding.UTF8;
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(FROM, EMAIL_PASSWORD);
                client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
                client.Host = "smtp.office365.com";
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                System.Threading.CancellationToken token = new System.Threading.CancellationToken();
                try
                {
                    await client.SendMailAsync(msg);
                    return true;

                }
                catch (Exception ex)
                {
                    throw new Exception("SolidariaExchangeEmailSender :: SendEmailUsingTemplateAsync", ex);
                }
            }
        }
    }
}
