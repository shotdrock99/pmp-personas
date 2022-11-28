using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class AuthorizationsUsersDataTableReader : IAuthorizationsUsersDataReader
    {
        const string SP_NAME = "PMP.USP_TB_AutorizacionesUsuarios";

        public async Task<IEnumerable<AuthorizationUser>> GetAuthorizationsUsersAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = version;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var result = new List<AuthorizationUser>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo");
                        var notificado = SqlReaderUtilities.SafeGet<int>(reader, "IN_notificado");
                        var especial = SqlReaderUtilities.SafeGet<int>(reader, "IN_autorizacion_gsp_esp");
                        var item = new AuthorizationUser
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            VersionCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            Codigo = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_autoriza"),
                            CodigoRol = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_rol"),
                            CodigoNivel = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_nivel"),
                            CodigoTipoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_autorizacion"),
                            Especial = especial == 1 ? true : false,
                            Activo = activo == 1 ? true : false,
                            Notificado = notificado == 1 ? true : false
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("AuthorizationsDataTableReader :: GetAuthorizationsByCodigoCotizacion", ex);
                }
            }
        }
    }
}
