using ModernizacionPersonas.DAL.Entities.SISEEntities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.SISEServices
{
    public class SISEAuthorizationsProcessor
    {
        const string SP_NAME = "sp_valida_autorizacion_cotiz_web_VC";

        public async Task<SISEAutorizacionesProcessResponse> ProcessAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@id_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@id_version", SqlDbType.Int).Value = version;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;

                var authorizations = new List<CotizacionAuthorization>();
                var users = new List<AuthorizationUser>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var message = await this.GetMessageAsync(reader);

                        reader.NextResult();
                        authorizations = await this.GetDataSetAsAuthorizationsAsync(reader);

                        reader.NextResult();
                        users = await this.GetDataSetAsAuthorizationUsersAsync(reader, version);

                        return new SISEAutorizacionesProcessResponse
                        {
                            ValidationMessage = message,
                            Authorizations = authorizations,
                            AuthorizationUsers = users
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosSummaryProcessor :: ProcessAsync", ex);
                }
            }
        }

        private async Task<List<AuthorizationUser>> GetDataSetAsAuthorizationUsersAsync(SqlDataReader reader, int version)
        {
            var result = new List<AuthorizationUser>();
            while (await reader.ReadAsync())
            {
                var activo = SqlReaderUtilities.SafeGet<int>(reader, "sn_usu_niv_activo") == 0 ? false : true;
                var notificado = SqlReaderUtilities.SafeGet<int>(reader, "sn_notificado") == 0 ? false : true;
                var delegacion = SqlReaderUtilities.SafeGet<int>(reader, "sn_delegacion") == 0 ? false : true;
                var user = new AuthorizationUser
                {
                    CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "id_cotizacion"),
                    VersionCotizacion = version,
                    Codigo = SqlReaderUtilities.SafeGet<string>(reader, "cod_usuario_autoriza").ToString(),
                    CodigoRol = SqlReaderUtilities.SafeGet<int>(reader, "cod_rol"),
                    CodigoNivel = SqlReaderUtilities.SafeGet<int>(reader, "cod_nivel"),
                    CodigoTipoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_aut"),
                    Activo = activo,
                    Notificado = notificado,
                    Delegacion = delegacion
                };

                result.Add(user);
            }

            return result;
        }

        private async Task<List<CotizacionAuthorization>> GetDataSetAsAuthorizationsAsync(SqlDataReader reader)
        {
            var result = new List<CotizacionAuthorization>();
            while (await reader.ReadAsync())
            {
                var requiereAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "sn_requiere_aut") == 0 ? false : true;

                var authorization = new CotizacionAuthorization
                {
                    CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "id_cotizacion"),
                    Version = SqlReaderUtilities.SafeGet<int>(reader, "id_version"),
                    CodigoGrupoAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "cod_grupo_aseg"),
                    CodigoSucursal = SqlReaderUtilities.SafeGet<decimal>(reader, "cod_suc"),
                    CodigoRamo = SqlReaderUtilities.SafeGet<decimal>(reader, "cod_ramo"),
                    CodigoSubramo = SqlReaderUtilities.SafeGet<decimal>(reader, "cod_subramo"),
                    CodigoAmparo = SqlReaderUtilities.SafeGet<decimal>(reader, "cod_amparo"),
                    CampoEntrada = SqlReaderUtilities.SafeGet<string>(reader, "campo_entrada"),
                    ValorEntrada = SqlReaderUtilities.SafeGet<decimal>(reader, "valor_entrada"),
                    CodigoTipoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_aut"),
                    RequiereAutorizacion = requiereAutorizacion,
                    CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "cod_usuario"),
                    MensajeValidacion = SqlReaderUtilities.SafeGet<string>(reader, "txt_respuesta")
                };

                if (requiereAutorizacion)
                {
                    result.Add(authorization);
                }
            }

            return result;
        }

        private async Task<AuthorizationValidationMessage> GetMessageAsync(SqlDataReader reader)
        {
            var result = new AuthorizationValidationMessage();
            while (await reader.ReadAsync())
            {
                result.IsValid = SqlReaderUtilities.SafeGet<int>(reader, "sn_ok") == 0 ? false : true;
                //result.Message = SqlReaderUtilities.SafeGet<string>(reader, "");
                result.Message = reader[1].ToString();
            }

            return result;
        }
    }

    public class SISEAutorizacionesProcessResponse
    {
        public AuthorizationValidationMessage ValidationMessage { get; set; }
        public List<CotizacionAuthorization> Authorizations { get; set; }
        public List<AuthorizationUser> AuthorizationUsers { get; set; }
    }
}
