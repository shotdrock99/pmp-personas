using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Soligespro
{
    public class SoligesproDataUsuariosReader : ISoligesproDataUsuariosReader
    {
        const string SP_NAME = "Usp_UsuariosSoligesproBPM";

        public async Task<UserExternalInfo> GetUserAsync(string usuario)
        {
            using (var conn = ConnectionFactory.SoligesproConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_CodigoZona", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoSucursal", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoDependencia", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoArea", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoCargo", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_VC_Usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@VAR_IN_TipoTran", SqlDbType.Int).Value = 1;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                var user = new UserExternalInfo();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var codigoZona = SqlReaderUtilities.SafeGet<int>(reader, "cod_zona");
                        user.LoginUsuario = SqlReaderUtilities.SafeGet<string>(reader, "login_usu");
                        user.NombreUsuario = SqlReaderUtilities.SafeGet<string>(reader, "nom_usu");
                        user.EmailUsuario = SqlReaderUtilities.SafeGet<string>(reader, "mail_usu").ToString();
                        user.Zona = int.Parse(codigoZona.ToString());
                        user.Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc");
                        user.Sucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc");
                        user.NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc");
                        user.CodigoAreaDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_area_dep");
                        user.NombreDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_depen");
                        user.CodigoArea = SqlReaderUtilities.SafeGet<int>(reader, "cod_area");
                        user.Area = SqlReaderUtilities.SafeGet<string>(reader, "txt_area");
                        user.CodigoCargo = SqlReaderUtilities.SafeGet<int>(reader, "cod_cargo");
                        user.Cargo = SqlReaderUtilities.SafeGet<string>(reader, "txt_cargo");
                        user.CodigoTipoDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_dependencia");
                        user.TipoDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_tipo_dependencia");
                    }

                    return user;

                }
                catch (Exception ex)
                {
                    throw new Exception("SoligesproDataUsuariosReader :: SoligesproUsuarioReadAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<UserExternalInfo>> GetUserDirectorComercialAsync(int codigoSucursal)
        {
            using (var conn = ConnectionFactory.SoligesproConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_CodigoZona", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoSucursal", SqlDbType.Int).Value = codigoSucursal;
                cmd.Parameters.Add("@VAR_IN_CodigoDependencia", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoArea", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoCargo", SqlDbType.Int).Value = 16;
                cmd.Parameters.Add("@VAR_VC_Usuario", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@VAR_IN_TipoTran", SqlDbType.Int).Value = 2;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                var userList = new List<UserExternalInfo>();

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var codigoZona = SqlReaderUtilities.SafeGet<int>(reader, "cod_zona");
                            var user = new UserExternalInfo
                            {
                                LoginUsuario = SqlReaderUtilities.SafeGet<string>(reader, "login_usu"),
                                NombreUsuario = SqlReaderUtilities.SafeGet<string>(reader, "nom_usu"),
                                EmailUsuario = SqlReaderUtilities.SafeGet<string>(reader, "mail_usu").ToString(),
                                Zona = int.Parse(codigoZona.ToString()),
                                Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc"),
                                Sucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc"),
                                NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc"),
                                CodigoAreaDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_area_dep"),
                                NombreDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_depen"),
                                CodigoArea = SqlReaderUtilities.SafeGet<int>(reader, "cod_area"),
                                Area = SqlReaderUtilities.SafeGet<string>(reader, "txt_area"),
                                CodigoCargo = SqlReaderUtilities.SafeGet<int>(reader, "cod_cargo"),
                                Cargo = SqlReaderUtilities.SafeGet<string>(reader, "txt_cargo"),
                                CodigoTipoDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_dependencia"),
                                TipoDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_tipo_dependencia"),
                            };

                            userList.Add(user);
                        }

                        return userList;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SoligesproDataUsuariosReader :: GetUserDirectorComercialAsync", ex);
                }
            }
        }

        public async Task<UserExternalInfo> GetUserDirectorTecnicoAsync(int codigoSucursal)
        {
            using (var conn = ConnectionFactory.SoligesproConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_CodigoZona", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoSucursal", SqlDbType.Int).Value = codigoSucursal;
                cmd.Parameters.Add("@VAR_IN_CodigoDependencia", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoArea", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoCargo", SqlDbType.Int).Value = 43;
                cmd.Parameters.Add("@VAR_VC_Usuario", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@VAR_IN_TipoTran", SqlDbType.Int).Value = 2;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                var user = new UserExternalInfo();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var codigoZona = SqlReaderUtilities.SafeGet<int>(reader, "cod_zona");
                            user.LoginUsuario = SqlReaderUtilities.SafeGet<string>(reader, "login_usu");
                            user.NombreUsuario = SqlReaderUtilities.SafeGet<string>(reader, "nom_usu");
                            user.EmailUsuario = SqlReaderUtilities.SafeGet<string>(reader, "mail_usu").ToString();
                            user.Zona = int.Parse(codigoZona.ToString());
                            user.Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc");
                            user.Sucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc");
                            user.NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc");
                            user.CodigoAreaDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_area_dep");
                            user.NombreDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_depen");
                            user.CodigoArea = SqlReaderUtilities.SafeGet<int>(reader, "cod_area");
                            user.Area = SqlReaderUtilities.SafeGet<string>(reader, "txt_area");
                            user.CodigoCargo = SqlReaderUtilities.SafeGet<int>(reader, "cod_cargo");
                            user.Cargo = SqlReaderUtilities.SafeGet<string>(reader, "txt_cargo");
                            user.CodigoTipoDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_dependencia");
                            user.TipoDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_tipo_dependencia");
                        }

                        return user;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SoligesproDataUsuariosReader :: GetUserDirectorTecnicoAsync", ex);
                }
            }
        }

        public async Task<UserExternalInfo> GetUserDirectorZonalAsync(int codigoZona)
        {
            using (var conn = ConnectionFactory.SoligesproConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_CodigoZona", SqlDbType.Int).Value = codigoZona;
                cmd.Parameters.Add("@VAR_IN_CodigoSucursal", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoDependencia", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoArea", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoCargo", SqlDbType.Int).Value = 727;
                cmd.Parameters.Add("@VAR_VC_Usuario", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@VAR_IN_TipoTran", SqlDbType.Int).Value = 8;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                var user = new UserExternalInfo();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            
                            user.LoginUsuario = SqlReaderUtilities.SafeGet<string>(reader, "login_usu");
                            user.NombreUsuario = SqlReaderUtilities.SafeGet<string>(reader, "nom_usu");
                            user.EmailUsuario = SqlReaderUtilities.SafeGet<string>(reader, "mail_usu").ToString();
                            user.Zona = int.Parse(codigoZona.ToString());
                            user.Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc");
                            user.Sucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc");
                            user.NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc");
                            user.CodigoAreaDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_area_dep");
                            user.NombreDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_depen");
                            user.CodigoArea = SqlReaderUtilities.SafeGet<int>(reader, "cod_area");
                            user.Area = SqlReaderUtilities.SafeGet<string>(reader, "txt_area");
                            user.CodigoCargo = SqlReaderUtilities.SafeGet<int>(reader, "cod_cargo");
                            user.Cargo = SqlReaderUtilities.SafeGet<string>(reader, "txt_cargo");
                            user.CodigoTipoDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_dependencia");
                            user.TipoDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_tipo_dependencia");
                        }

                        return user;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SoligesproDataUsuariosReader :: GetUserDirectorTecnicoAsync", ex);
                }
            }
        }


        public async Task<UserExternalInfo> GetUserGerenteAsync(int codigoSucursal)
        {
            using (var conn = ConnectionFactory.SoligesproConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_CodigoZona", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoSucursal", SqlDbType.Int).Value = codigoSucursal;
                cmd.Parameters.Add("@VAR_IN_CodigoDependencia", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoArea", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@VAR_IN_CodigoCargo", SqlDbType.Int).Value = 9;
                cmd.Parameters.Add("@VAR_VC_Usuario", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@VAR_IN_TipoTran", SqlDbType.Int).Value = 2;
                //cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                var user = new UserExternalInfo();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var codigoZona = SqlReaderUtilities.SafeGet<int>(reader, "cod_zona");
                            user.LoginUsuario = SqlReaderUtilities.SafeGet<string>(reader, "login_usu");
                            user.NombreUsuario = SqlReaderUtilities.SafeGet<string>(reader, "nom_usu");
                            user.EmailUsuario = SqlReaderUtilities.SafeGet<string>(reader, "mail_usu").ToString();
                            user.Zona = int.Parse(codigoZona.ToString());
                            user.Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc");
                            user.Sucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc");
                            user.NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc");
                            user.CodigoAreaDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_area_dep");
                            user.NombreDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_depen");
                            user.CodigoArea = SqlReaderUtilities.SafeGet<int>(reader, "cod_area");
                            user.Area = SqlReaderUtilities.SafeGet<string>(reader, "txt_area");
                            user.CodigoCargo = SqlReaderUtilities.SafeGet<int>(reader, "cod_cargo");
                            user.Cargo = SqlReaderUtilities.SafeGet<string>(reader, "txt_cargo");
                            user.CodigoTipoDependencia = SqlReaderUtilities.SafeGet<int>(reader, "cod_tipo_dependencia");
                            user.TipoDependencia = SqlReaderUtilities.SafeGet<string>(reader, "txt_tipo_dependencia");
                        }

                        return user;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SoligesproDataUsuariosReader :: GetUserGerenteAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal)
        {
            var result = new List<UserExternalInfo>();

            var directorComercial = await GetUserDirectorComercialAsync(codigoSucursal);
            result.AddRange(directorComercial);
            var directorTecnico = await GetUserDirectorTecnicoAsync(codigoSucursal);
            result.Add(directorTecnico);
            var gerente = await GetUserGerenteAsync(codigoSucursal);
            result.Add(gerente);

            return result;
        }

        public async Task<IEnumerable<UserExternalInfo>> GetDirectoresAsync(int codigoSucursal, int codigoZona)
        {
            var result = new List<UserExternalInfo>();

            var directorComercial = await GetUserDirectorComercialAsync(codigoSucursal);
            result.AddRange(directorComercial);
            var directorTecnico = await GetUserDirectorTecnicoAsync(codigoSucursal);
            result.Add(directorTecnico);
            var gerente = await GetUserGerenteAsync(codigoSucursal);
            result.Add(gerente);
            var directorZonal = await GetUserDirectorZonalAsync(codigoZona);
            result.Add(directorZonal);

            return result;
        }

    }
}