using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosRolesTableWriter : IDatosRolesWriter
    {
        private const string SP_NAME = "PMP.USP_TB_Rol";

        public async Task<int> GuardarRolAsync(Rol model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Rol"
                };
                cmd.Parameters.Add("@VAR_VC_nombre_rol", SqlDbType.VarChar).Value = model.Nombre;
                cmd.Parameters.Add("@VAR_VC_descripcion_rol", SqlDbType.VarChar).Value = model.Descripcion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var resp = await cmd.ExecuteScalarAsync();
                    return int.Parse(resp.ToString());

                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRolesTableWriter :: GuardarRolAsync", ex);
                }
            }
        }

        public async Task ActualizarRolAsync(Rol model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Rol"
                };
                
                cmd.Parameters.Add("@VAR_VC_nombre_rol", SqlDbType.VarChar).Value = model.Nombre;
                cmd.Parameters.Add("@VAR_VC_descripcion_rol", SqlDbType.VarChar).Value = model.Descripcion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.Codigo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRolesTableWriter :: ActualizarRolAsync", ex);
                }
            }
        }

        public async Task EliminarRolAsync(int codigoRol, string usuarioLog)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Rol"
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = codigoRol;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuarioLog;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRolesTableWriter :: EliminarRolAsync", ex);
                }
            }
        }

        public async Task DesactivarRolAsync(int codigoRol, string usuarioLog)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Rol"
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = codigoRol;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuarioLog;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 51;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRolesTableWriter :: EliminarRolAsync", ex);
                }
            }
        }
    }
}
