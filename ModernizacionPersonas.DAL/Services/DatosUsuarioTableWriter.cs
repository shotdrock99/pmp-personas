using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosUsuarioTableWriter : IDatosUsuariosWriter
    {
        const string SP_NAME = "PMP.USP_TB_Usuarios";

        public async Task<int> CrearUsuarioAsync(ApplicationUser model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_VC_nombre_usuario", SqlDbType.VarChar).Value = model.UserName;
                cmd.Parameters.Add("@VAR_VC_numero_identificacion", SqlDbType.VarChar).Value = model.DocumentId != null ? model.DocumentId : " " ;
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.Rol.RoleId;
                cmd.Parameters.Add("@VAR_VC_nombre_completo_usuario", SqlDbType.VarChar).Value = model.Name;
                cmd.Parameters.Add("@VAR_VC_correo_usuario", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var resp = await cmd.ExecuteScalarAsync();
                    var codigoUsuario = Int32.Parse(resp.ToString());
                    return codigoUsuario;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosUsuarioTableWriter :: CrearUsuarioAsync", ex);
                }
            }
        }

        public async Task ActualizarUsuarioAsyn(ApplicationUser model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_VC_nombre_usuario", SqlDbType.VarChar).Value = model.UserName;
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = model.UserId;
                cmd.Parameters.Add("@VAR_VC_numero_identificacion", SqlDbType.VarChar).Value = model.DocumentId != null ? model.DocumentId : " ";
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = model.Rol.RoleId;
                cmd.Parameters.Add("@VAR_VC_nombre_completo_usuario", SqlDbType.VarChar).Value = model.Name;
                cmd.Parameters.Add("@VAR_VC_correo_usuario", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();                    
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosUsuarioTableWriter :: ActualizarUsuarioAsyn", ex);
                }
            }
        }

        public async Task ActivarDesactivarUsuarioAsync(int codigoUsuario, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = codigoUsuario;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTomadorTableWriter :: EliminarTomadorAsync", ex);
                }
            }
        }
    }
}
