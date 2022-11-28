using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosPermisosRolTableWriter : IDatosPermisosRolWriter
    {
        public async Task GuardarPermisoRolAsync(PermisoRol model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_PermisosxRol"
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.CodigoRol;
                cmd.Parameters.Add("@VAR_IN_cod_permiso", SqlDbType.Int).Value = model.CodigoPermiso;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableWriter :: GuardarPermisoRolAsync", ex);
                }
            }
        }

        public async Task ActualizarPermisoRolAsync(PermisoRol model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_PermisosxRol"
                };

                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.CodigoRol;
                cmd.Parameters.Add("@VAR_IN_cod_permiso", SqlDbType.Int).Value = model.CodigoPermiso;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableWriter :: ActualizarPermisoRolAsync", ex);
                }
            }
        }

        public async Task EliminarPermisoRolAsync(int codigoPermisoRol)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_PermisosxRol"
                };
                cmd.Parameters.Add("@VAR_IN_cod_permiso_rol", SqlDbType.VarChar).Value = codigoPermisoRol;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 51;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableWriter :: EliminarPermisoRolAsync", ex);
                }
            }
        }

        public async Task EliminarPermisosRolAsync(int codigoRol)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_PermisosxRol"
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = codigoRol;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableWriter :: EliminarPermisoRolAsync", ex);
                }
            }
        }
    }
}
