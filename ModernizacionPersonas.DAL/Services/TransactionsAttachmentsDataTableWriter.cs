using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class TransactionsAttachmentsDataTableWriter : ITransactionAttachmentsDataWriter
    {
        private const string SP_NAME = "PMP.USP_TB_Soportes";

        public async Task CreateTransactionAttachmentAsync(TransactionAttachment model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_VC_filename", SqlDbType.VarChar).Value = model.Name;
                cmd.Parameters.Add("@VAR_IN_cod_movimiento", SqlDbType.Int).Value = model.TransactionId;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsAttachmentsDataTableWriter :: CreateTransactionAttachmentAsync", ex);
                }
            }
        }

        public async Task DeleteTransactionAttachmentAsync(int attachmentId)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_soporte", SqlDbType.Int).Value = attachmentId;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsAttachmentsDataTableWriter :: DeleteTransactionAttachmentAsync", ex);
                }
            }
        }
    }
}
