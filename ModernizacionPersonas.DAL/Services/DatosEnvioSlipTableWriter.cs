using ModernizacionPersonas.DAL.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosEnvioSlipTableWriter : IDatosEnvioSlipWriter
    {
        public async Task<int> CrearEnvioSlipAsync(int codigoCotizacion, string emailTo, string emailCC, string emailComments)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EnvioSlip"
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_para", SqlDbType.VarChar).Value = emailTo;
                cmd.Parameters.Add("@VAR_VC_para_oculto", SqlDbType.VarChar).Value = emailCC;
                cmd.Parameters.Add("@VAR_VC_texto_envio", SqlDbType.VarChar).Value = emailComments;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosEnvioSlipTableWriter :: CrearEnvioSlipAsync", ex);
                }
            }
        }
        public async Task<int> BorrarAdjuntoEnvioSlipByNamesAsync(int codigoCotizacion, string fileName)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AdjuntoEnvioSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_filename", SqlDbType.VarChar).Value = fileName;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 51;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                    return 1;
                }
                catch (Exception ex)

                {
                    throw new Exception("DatosAdjuntoEnvioSlipTableWriter :: BorrarAdjuntoEnvioSlipNombreAsync", ex);
                }
            }
        }
        public async Task<int> CrearAdjuntoEnvioSlipAsync(int codigoCotizacion, byte[] adjunto, string fileName)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AdjuntoEnvioSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VB_file", SqlDbType.VarBinary).Value = adjunto;
                cmd.Parameters.Add("@VAR_VC_filename", SqlDbType.VarChar).Value = fileName;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAdjuntoEnvioSlipTableWriter :: CrearAdjuntoEnvioSlipAsync", ex);
                }
            }
        }


        public async Task<int> BorrarAdjuntoEnvioSlipAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AdjuntoEnvioSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 52;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAdjuntoEnvioSlipTableWriter :: BorrarAdjuntoEnvioSlipAsync", ex);
                }
            }
        }

        public async Task<int> BorrarAdjuntoEnvioSlipNombreAsync(int codigoCotizacion, string fileName)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AdjuntoEnvioSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_filename", SqlDbType.VarChar).Value = fileName;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 51;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAdjuntoEnvioSlipTableWriter :: BorrarAdjuntoEnvioSlipNombreAsync", ex);
                }
            }
        }

    }
}
