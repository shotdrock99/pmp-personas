using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosBloqueoTableWriter : IDatosBloqueoWriter
    {

        public async Task BloquearAsync(int codigoCotizacion, int codigoUsuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Bloqueo"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.VarChar).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.Int).Value = codigoUsuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await DesbloquearAsync(codigoCotizacion);
                    var resp = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosBloqueoTableWriter :: BloquearAsync", ex);
                }
            }
        }

        public async Task DesbloquearAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Bloqueo"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.VarChar).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    var resp = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosBloqueoTableWriter :: DesbloquearAsync", ex);
                }
            }
        }
    }
}
