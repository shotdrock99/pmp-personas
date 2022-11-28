using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosParametrizacionAppTableWriter : IDatosParametrizacionAppWriter
    {

        public async Task EditarValorAppAsync(int codigoVariable, string valorApp)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionApp"
                };
                cmd.Parameters.Add("@VAR_VC_valor_variable", SqlDbType.VarChar).Value = valorApp;
                cmd.Parameters.Add("@VAR_IN_cod_variable_app", SqlDbType.Int).Value = codigoVariable;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    var resp = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionAppTableWriter :: EditarValorAppAsync", ex);
                }
            }
        }

       
    }
}
