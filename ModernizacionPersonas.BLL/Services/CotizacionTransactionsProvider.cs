using Microsoft.AspNetCore.Http;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionTransactionsProvider
    {
        private readonly ITransactionsDataWriter transactionsWriter;
        private readonly ITransactionsDataReader transactionsReader;
        private readonly ITransactionAttachmentsDataReader transactionAttachmentsReader;
        private readonly ITransactionAttachmentsDataWriter transactionAttachmentsWriter;
        private readonly TransactionsCommentsDataTableWriter transactionCommentsWriter;
        private readonly TransactionsCommentsDataTableReader transactionCommentsReader;

        public CotizacionTransactionsProvider()
        {
            this.transactionsWriter = new TransactionsDataTableWriter();
            this.transactionsReader = new TransactionsDataTableReader();
            this.transactionAttachmentsReader = new TransactionsAttachmentsDataTableReader();
            this.transactionAttachmentsWriter = new TransactionsAttachmentsDataTableWriter();
            this.transactionCommentsWriter = new TransactionsCommentsDataTableWriter();
            this.transactionCommentsReader = new TransactionsCommentsDataTableReader();
        }

        private async Task<ActionResponseBase> SaveTransactionCommentsAsync(string userName, int transactionId, IEnumerable<TransactionComment> comments)
        {
            foreach (var comment in comments)
            {
                comment.CodigoUsuario = userName;
                comment.TransactionId = transactionId;
                comment.CodigoRolAutorizacion = "";
                comment.CodigoTipoAutorizacion = 0;
                await this.transactionCommentsWriter.CreateTransactionCommentAsync(comment);
            }

            return new ActionResponseBase();
        }

        private async Task SaveTransactionCommentAsync(string userName, int transactionId, string commentText)
        {
            var comment = new TransactionComment
            {
                CodigoUsuario = userName,
                Message = commentText,
                TransactionId = transactionId
            };

            await this.transactionCommentsWriter.CreateTransactionCommentAsync(comment);
        }

        private async Task AggregateTransactionAttachmentsAsync(IEnumerable<CotizacionTransaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var attachments = await this.transactionAttachmentsReader.GetTransactionsAttachmentsAsync(transaction.CodigoTransaccion);
                transaction.Attachments = attachments;
            }
        }

        private async Task AggregateTransactionCommentsAsync(IEnumerable<CotizacionTransaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var comments = await this.transactionCommentsReader.GetTransactionsCommentsAsync(transaction.CodigoTransaccion);
                transaction.Comments = comments;
            }
        }

        public async Task<GetTransactionsResponse> GetTransactionsAsync(int codigoCotizacion, int version)
        {
            var response = await this.transactionsReader.GetTransactionsAsync(codigoCotizacion, version);
            await this.AggregateTransactionCommentsAsync(response.Transactions);
            await this.AggregateTransactionAttachmentsAsync(response.Transactions);
            return new GetTransactionsResponse
            {
                CodigoCotizacion = response.CodigoCotizacion,
                CodigoEstadoCotizacion = response.CodigoEstadoCotizacion,
                NumeroCotizacion = response.NumeroCotizacion,
                Transactions = response.Transactions                
            };
        }

        public async Task<IEnumerable<CotizacionTransaction>> GetAuthorizationTransactionsAsync(int codigoCotizacion, int version)
        {
            var transactions = await this.transactionsReader.GetAuthorizationTransactionsAsync(codigoCotizacion, version);
            await this.AggregateTransactionCommentsAsync(transactions);
            await this.AggregateTransactionAttachmentsAsync(transactions);
            return transactions;
        }

        public async Task<int> CreateTransactionAsync(int codigoCotizacion, int version, string userName, string message)
        {
            var transaction = new CotizacionTransaction
            {
                CodigoCotizacion = codigoCotizacion,
                Version = version,
                CodigoUsuario = userName,
                Description = message,
                CreationDate = DateTime.Now
            };

            return await this.transactionsWriter.CreateTransactionAsync(transaction);
        }
        
        public async Task<int> CreateTransactionAsync(int codigoCotizacion, int version, string userName, string message, string commentText)
        {
            var transactionId = await this.CreateTransactionAsync(codigoCotizacion, version, userName, message);
            await this.SaveTransactionCommentAsync(userName, transactionId, commentText);
            return transactionId;
        }

        public async Task CreateTransactionAttachment(string userName, int transactionId, string fileName)
        {
            var model = new TransactionAttachment
            {
                TransactionId = transactionId,
                Name = fileName
            };

            await this.transactionAttachmentsWriter.CreateTransactionAttachmentAsync(model);
        }

        public async Task<int> CreateAuthorizationTransaction(CotizacionTransactionArgs args)
        {
            var transaction = new CotizacionTransaction
            {
                CodigoCotizacion = args.CodigoCotizacion,
                Version = args.Version,
                CodigoUsuario = args.UserName,
                ConteoAutorizaciones = args.AuthorizationsCount,
                Description = args.Description,
                UNotificado = args.UNotificado
            };

            var transactionId = await this.transactionsWriter.CreateTransactionAsync(transaction);
            await this.SaveTransactionCommentsAsync(args.UserName, transactionId, args.Comments);

            return transactionId;
        }

        public async Task UpdateAuthorizationTransaction(CotizacionTransactionArgs args)
        {
            var transaction = new CotizacionTransaction
            {
                CodigoCotizacion = args.CodigoCotizacion,
                CodigoTransaccion = args.TransactionId,
                Version = args.Version,
                CodigoUsuario = args.UserName,
                ConteoAutorizaciones = args.AuthorizationsCount,
                Description = args.Description,
                UNotificado = args.UNotificado
            };

            await this.SaveTransactionCommentsAsync(args.UserName, args.TransactionId, args.Comments);
            await this.transactionsWriter.UpdateTransactionAsync(transaction);
        }
    }
}
