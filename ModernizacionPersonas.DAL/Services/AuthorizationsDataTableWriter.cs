using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class AuthorizationsDataTableWriter : IAuthorizationsDataWriter
    {
        const string command = "PMP.USP_TB_Autorizaciones";

        public async Task SaveAuthorizationAsync(CotizacionAuthorization model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = model.Version;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_suc", SqlDbType.Int).Value = model.CodigoSucursal;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@VAR_VC_campo_entrada", SqlDbType.VarChar).Value = model.CampoEntrada;
                cmd.Parameters.Add("@VAR_NM_valor_entrada", SqlDbType.Decimal).Value = model.ValorEntrada;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_autorizacion", SqlDbType.Int).Value = model.CodigoTipoAutorizacion;
                cmd.Parameters.Add("@VAR_IN_requiere_autorizacion", SqlDbType.Int).Value = model.RequiereAutorizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.CodigoUsuario;
                cmd.Parameters.Add("@VAR_VC_txt_respuesta", SqlDbType.VarChar).Value = model.MensajeValidacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAutorizacionesTableWriter :: CrearAutorizacionAsync", ex);
                }
            }
        }

        public async Task SaveValidationsWEBAsync(int codigoCotizacion, int numCot, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion_cons", SqlDbType.Int).Value = numCot;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = version;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAutorizacionesTableWriter :: SaveValidationsWEBAsync", ex);
                }
            }
        }

        public async Task DeleteAuthorizationAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = version;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAutorizacionesTableWriter :: EliminarAutorizacionAsync", ex);
                }
            }
        }

        public async Task DeleteAuthorizationByQueryAsync(int codigoCotizacion, int version)
        {
            var queryString = "DELETE FROM PMP.TB_Autorizaciones WHERE (IN_cod_cotizacion = @VAR_IN_cod_cotizacion AND IN_cod_version = @VAR_IN_cod_version) AND VC_campo_entrada LIKE 'condiciones'";
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = version;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAutorizacionesTableWriter :: EliminarAutorizacionAsync", ex);
                }
            }
        }
    }
}
