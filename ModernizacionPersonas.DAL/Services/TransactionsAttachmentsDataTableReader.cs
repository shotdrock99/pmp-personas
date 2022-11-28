using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class TransactionsAttachmentsDataTableReader : ITransactionAttachmentsDataReader
    {
        public string command = "PMP.USP_TB_Soportes";

        public async Task<IEnumerable<TransactionAttachment>> GetTransactionsAttachmentsAsync(int transactionId)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_movimiento", SqlDbType.Int).Value = transactionId;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var attachments = new List<TransactionAttachment>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var attachment = new TransactionAttachment
                        {
                            TransactionId = (int)reader["IN_cod_movimiento"],
                            MimeType = "",
                            Name = reader["VC_filename"].ToString(),
                            Uri = ""
                        };

                        attachments.Add(attachment);
                    }

                    return attachments;
                }
                catch (Exception ex)
                {
                    throw new Exception("TransactionsAttachmentsDataTableReader :: GetTransactionsAttachmentsAsync", ex);
                }
            }
        }

    }
}
