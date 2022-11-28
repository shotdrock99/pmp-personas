using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class AuthorizationsUsersDataTableWriter : IAuthorizationUsersDataWriter
    {
        public string command = "PMP.USP_TB_AutorizacionesUsuarios";

        public async Task SaveAuthorizationUserAsync(AuthorizationUser model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_autorizacion", SqlDbType.Int).Value = model.CodigoTipoAutorizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = model.VersionCotizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario_autoriza", SqlDbType.VarChar).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.CodigoRol;
                cmd.Parameters.Add("@VAR_IN_cod_nivel", SqlDbType.Int).Value = model.CodigoNivel;
                cmd.Parameters.Add("@VAR_IN_activo", SqlDbType.Int).Value = model.Activo;
                cmd.Parameters.Add("@VAR_IN_delegacion", SqlDbType.Int).Value = model.Delegacion;
                cmd.Parameters.Add("@VAR_IN_notificado", SqlDbType.Int).Value = model.Notificado;
                cmd.Parameters.Add("@VAR_IN_autorizacion_gsp_esp", SqlDbType.Int).Value = model.Especial;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("AuthorizationsUsersDataTableWriter :: SaveAuthorizationUserAsync", ex);
                }
            }
        }

        public async Task RemoveAuthorizationUsersAsync(int codigoCotizacion, int version)
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
                    throw new Exception("AuthorizationsUsersDataTableWriter :: RemoveAuthorizationUsersAsync", ex);
                }
            }
        }

        public async Task<int> GetClausulasEspAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 11;
                cmd.Connection = conn;
                var procesados = 0;
                try
                {
                    var readerConteo = await cmd.ExecuteReaderAsync();
                    while (await readerConteo.ReadAsync())
                    {
                        procesados = int.Parse(readerConteo[0].ToString());                        
                    }
                    return procesados;
                }
                catch (Exception ex)
                {
                    throw new Exception("AuthorizationsUsersDataTableWriter :: RemoveAuthorizationUsersAsync", ex);
                }
            }
        }

        public async Task RemoveAuthorizationUsersByQueryAsync(int codigoCotizacion, int version)
        {
            var queryString = "DELETE FROM PMP.TB_AutorizacionesUsuarios WHERE IN_cod_cotizacion = @VAR_IN_cod_cotizacion AND IN_cod_version = @VAR_IN_cod_version";
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
                    throw new Exception("AuthorizationsUsersDataTableWriter :: RemoveAuthorizationUsersAsync", ex);
                }
            }
        }
    }
}
