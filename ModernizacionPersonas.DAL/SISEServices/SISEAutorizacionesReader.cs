using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.SISEServices
{
    public class SISEAutorizacionesReader
    {
        const string SP_NAME = "usp_pmp_autorizaciones";

        public async Task<IEnumerable<RolesAutorizaciones>> LeerRolesAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", 3);
                cmd.Connection = conn;
                var roles = new List<RolesAutorizaciones>();
                try
                {
                    var rolesReader = await cmd.ExecuteReaderAsync();
                    while (await rolesReader.ReadAsync())
                    {
                        var rol = new RolesAutorizaciones()
                        {
                            CodigoRol = (int)rolesReader["cod_rol"],
                            Rol = (string)rolesReader["txt_desc_rol"]
                        };
                        roles.Add(rol);
                    }

                    return roles;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAutorizacionesReader :: LeerRolesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<AuthorizationUser>> LeerUsersWEBAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.AddWithValue("@VAR_IN_cod_cotizacion", codigoCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_version", version);
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", 4);
                cmd.Connection = conn;
                var users = new List<AuthorizationUser>();
                try
                {
                    var usersReader = await cmd.ExecuteReaderAsync();
                    while (await usersReader.ReadAsync())
                    {
                        var activo = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_activo");
                        var notificado = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_notificado");
                        var delegacion = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_delegacion");
                        var especial = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_autoriza_gsp_esp");

                        var _CodigoCotizacion = (int)usersReader["cod_cotizacion"];
                        var _VersionCotizacion = (int)usersReader["cod_version"];
                        var _Codigo = (string)usersReader["cod_usuario_autoriza"];
                        var _CodigoRol = (int)usersReader["cod_rol"];
                        var _CodigoNivel = (int)usersReader["cod_nivel_rango"];
                        var _CodigoTipoAutorizacion = (int)usersReader["cod_tipo_autorizacion"];

                        var user = new AuthorizationUser()
                        {
                            CodigoCotizacion = (int)usersReader["cod_cotizacion"],
                            VersionCotizacion = (int)usersReader["cod_version"],
                            Codigo = (string)usersReader["cod_usuario_autoriza"],
                            CodigoRol = (int)usersReader["cod_rol"],
                            CodigoNivel = (int)usersReader["cod_nivel_rango"],
                            CodigoTipoAutorizacion = (int)usersReader["cod_tipo_autorizacion"],
                            Activo = activo == 0 ? false : true,
                            Notificado = notificado == 0 ? false : true,
                            Delegacion = delegacion == 0 ? false : true,
                            Especial = especial == 0 ? false : true
                        };

                        users.Add(user);
                    }

                    return users;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAutorizacionesReader :: LeerRolesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<AuthorizationUser>> LeerUsersSpecialAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.AddWithValue("@VAR_IN_cod_cotizacion", codigoCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_version", version);
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", 5);
                cmd.Connection = conn;
                var users = new List<AuthorizationUser>();
                try
                {
                    var usersReader = await cmd.ExecuteReaderAsync();
                    while (await usersReader.ReadAsync())
                    {
                        var activo = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_activo");
                        var notificado = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_notificado");
                        var delegacion = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_delegacion");
                        var especial = SqlReaderUtilities.SafeGet<int>(usersReader, "sn_autoriza_gsp_esp");

                        var _CodigoCotizacion = (int)usersReader["cod_cotizacion"];
                        var _VersionCotizacion = (int)usersReader["cod_version"];
                        var _Codigo = (string)usersReader["cod_usuario_autoriza"];
                        var _CodigoRol = (int)usersReader["cod_rol"];
                        var _CodigoNivel = (int)usersReader["cod_nivel_rango"];
                        var _CodigoTipoAutorizacion = (int)usersReader["cod_tipo_autorizacion"];

                        var user = new AuthorizationUser()
                        {
                            CodigoCotizacion = (int)usersReader["cod_cotizacion"],
                            VersionCotizacion = (int)usersReader["cod_version"],
                            Codigo = (string)usersReader["cod_usuario_autoriza"],
                            CodigoRol = (int)usersReader["cod_rol"],
                            CodigoNivel = (int)usersReader["cod_nivel_rango"],
                            CodigoTipoAutorizacion = (int)usersReader["cod_tipo_autorizacion"],
                            Activo = activo == 0 ? false : true,
                            Notificado = notificado == 0 ? false : true,
                            Delegacion = delegacion == 0 ? false : true,
                            Especial = especial == 0 ? false : true
                        };

                        users.Add(user);
                    }

                    return users;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAutorizacionesReader :: LeerRolesAsync", ex);
                }
            }
        }
    }

    public class RolesAutorizaciones
    {
        public int CodigoRol { get; set; }
        public string Rol { get; set; }
    }
}
