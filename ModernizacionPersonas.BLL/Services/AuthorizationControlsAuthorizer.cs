using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Contracts;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class AuthorizationControlsAuthorizer
    {
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly CotizacionTransactionsProvider cotizacionTransactionsProvider;
        private readonly CotizacionStateWriter cotizacionStateWriter;
        private readonly InformacionNegocioDataProvider informacionNegocioProvider;
        private readonly AuthorizationEmailSender authorizationEmailSender;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly IDatosEnvioSlipReader datosEnvioSlipReader;
        private readonly IDatosEnvioSlipWriter datosEnvioSlipWriter;


        public AuthorizationControlsAuthorizer()
        {
            this.cotizacionTransactionsProvider = new CotizacionTransactionsProvider();
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.informacionNegocioProvider = new InformacionNegocioDataProvider();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.authorizationEmailSender = new AuthorizationEmailSender();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
            this.datosEnvioSlipReader = new DatosEnvioSlipTableReader();
            this.datosEnvioSlipWriter = new DatosEnvioSlipTableWriter();
        }

        public async Task<ActionResponseBase> NotifyAuthorizationControlsAsync(int codigoCotizacion, NotifyCotizacionArgs args)
        {
            try
            {
                var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
                if (informacionNegocio.CotizacionChanged)
                {
                    return ActionResponseBase.CreateInvalidResponse("La cotización fue modificada y es posible que algunos controles deban ser validados. Por favor verifique e intente nuevamente.");
                }

                var userInfo = await this.soligesproUsersDataReader.GetUserAsync(args.AuthorizationUser.Codigo);
                var authorizationsCount = args.AuthorizationControls.Count();
                var transactionArgs = new CotizacionTransactionArgs
                {
                    CodigoCotizacion = codigoCotizacion,
                    UserName = args.UserName,
                    AuthorizationsCount = authorizationsCount,
                    Description = "Solicitud de autorización",
                    Comments = args.Comments,
                    UNotificado = userInfo.LoginUsuario
                };

                // save transaction                
                if (args.TransactionId > 0)
                {
                    transactionArgs.TransactionId = args.TransactionId;
                    await this.cotizacionTransactionsProvider.UpdateAuthorizationTransaction(transactionArgs);
                }
                else
                {
                    await this.cotizacionTransactionsProvider.CreateAuthorizationTransaction(transactionArgs);
                }

                // Actualizar el estado de la cotizacion        
                var response = await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.PendingAuthorization);
                // Registrar usuario notificado
                await this.datosCotizacionWriter.InsertarUsuarioNotificadoAsync(codigoCotizacion, response.Version, args.AuthorizationUser.Codigo);

                var comment = args.Comments.FirstOrDefault().Message;
                var recipients = new string[] { userInfo.EmailUsuario };
                await this.authorizationEmailSender.SendNotificationEmail(codigoCotizacion, informacionNegocio.NumeroCotizacion, informacionNegocio.CodigoRamo, recipients, comment, response.Version);

                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionAuthorizer :: NotifyCotizacionAsync", ex);
            }
        }

        public async Task<ActionResponseBase> ConfirmAuthorizationControlsAsync(int codigoCotizacion, int version, AuthorizationArgs args)
        {
            try
            {
                var cotizacionState = this.GetCotizacionStateByAuthorizationAction(args.AutorizacionAction);
                var nameUsuarioCreador = this.informacionNegocioProvider.GetUsuarioCreadorAsync(codigoCotizacion, version).Result;
                // save transaction                           
                var transactionArgs = new CotizacionTransactionArgs
                {
                    CodigoCotizacion = codigoCotizacion,
                    UserName = args.UserName,
                    // AuthorizationsCount = authorizationsCount,
                    Description = args.AutorizacionAction == AuthorizationAction.Accept ? "Cotización Autorizada"
                    : args.AutorizacionAction == AuthorizationAction.Modify ? "Cotización devuelta para revisión"
                    : "Cotización No Autorizada",
                    Comments = args.AuthorizationResult.Comments
                };
                
                if (args.TransactionId > 0)
                {
                    transactionArgs.TransactionId = args.TransactionId;
                    await this.cotizacionTransactionsProvider.UpdateAuthorizationTransaction(transactionArgs);
                }
                else
                {
                    await this.cotizacionTransactionsProvider.CreateAuthorizationTransaction(transactionArgs);
                }

                // update cotizacion state
                var response = await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, cotizacionState);
                // update selfAuthorizeFlag infoNegocio
                await this.informacionNegocioWriter.UpdateSelfAuthorizeFlagASync(codigoCotizacion, true);
                // update cotizacion modified flag to false if ApprovedAuthorization
                if (args.AutorizacionAction == AuthorizationAction.Accept)
                {
                    await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, false);
                }

                // update company values            
                await this.informacionNegocioProvider.UpdateCompanyData(codigoCotizacion, version, args.AuthorizationResult.GastosCompania, args.AuthorizationResult.UtilidadesCompania);
                var comment = args.AuthorizationResult.Comments.FirstOrDefault().Message;

                var userInfo = await this.soligesproUsersDataReader.GetUserAsync(args.UserName);
                var userCreadorCot = await this.soligesproUsersDataReader.GetUserAsync(nameUsuarioCreador);
                
                // var recipients = new string[] { userInfo.EmailUsuario };
                var recipients = new string[] { userCreadorCot.EmailUsuario};
                await this.SendEmailAsync(codigoCotizacion, comment, recipients, args.AutorizacionAction);

                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionAuthorizer :: ConfirmCotizacionAsync", ex);
            }
        }

        private async Task SendEmailAsync(int codigoCotizacion, string comment, string[] recipients, AuthorizationAction autorizacionAction)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            if (autorizacionAction == AuthorizationAction.Accept)
            {
                await this.authorizationEmailSender.SendApprovedAuthorizationEmail(codigoCotizacion, informacionNegocio.NumeroCotizacion, informacionNegocio.CodigoRamo, recipients, comment, informacionNegocio.Version);
            }
            else if (autorizacionAction == AuthorizationAction.Modify)
            {
                await this.authorizationEmailSender.SendReviewAuthorizationEmail(codigoCotizacion, informacionNegocio.NumeroCotizacion, informacionNegocio.CodigoRamo, recipients, comment, informacionNegocio.Version);
            }
            else if (autorizacionAction == AuthorizationAction.Reject)
            {
                await this.authorizationEmailSender.SendRefusedAuthorizationEmail(codigoCotizacion, informacionNegocio.NumeroCotizacion, informacionNegocio.CodigoRamo, recipients, comment, informacionNegocio.Version);
            }
        }

        private CotizacionState GetCotizacionStateByAuthorizationAction(AuthorizationAction autorizacionAction)
        {
            switch (autorizacionAction)
            {
                case AuthorizationAction.Accept:
                    return CotizacionState.ApprovedAuthorization;
                case AuthorizationAction.Reject:
                    return CotizacionState.RefusedAuthorization;
                case AuthorizationAction.Modify:
                    return CotizacionState.Lookover;
                default:
                    return CotizacionState.RefusedAuthorization;
            }
        }
    }

    public class SendNotificacionEmailArgs
    {
        public string DescripcionRamo { get; set; }
        public string NombreTomador { get; set; }
        public List<CotizacionAuthorization> ControlesAutorizacion { get; set; }
    }
}
