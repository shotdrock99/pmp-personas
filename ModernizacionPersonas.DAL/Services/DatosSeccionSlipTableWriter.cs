using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSeccionSlipTableWriter : IDatosSeccionSlipWriter
    {
        private const string SP_NAME = "PMP.USP_TB_SeccionesSlip";
        public async Task GuardarSeccionAsync(SeccionSlip model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_VC_seccion_slip", SqlDbType.VarChar).Value = model.Seccion;
                cmd.Parameters.Add("@VAR_IN_seccion_grupo", SqlDbType.Int).Value = model.Grupo;
                cmd.Parameters.Add("@VAR_IN_especial", SqlDbType.Int).Value = model.Especial;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSeccionSlipTableWriter :: GuardarSeccionAsync", ex);
                }
            }
        }

        public async Task ActualizarSeccionAsync(SeccionSlip model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_VC_seccion_slip", SqlDbType.VarChar).Value = model.Seccion;
                cmd.Parameters.Add("@VAR_IN_seccion_grupo", SqlDbType.Int).Value = model.Grupo;
                cmd.Parameters.Add("@VAR_IN_especial", SqlDbType.Int).Value = model.Especial;
                cmd.Parameters.Add("@VAR_IN_activo", SqlDbType.Int).Value = model.Activo;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSeccionSlipTableWriter :: ActualizarSeccionAsync", ex);
                }
            }
        }

        public async Task EliminarSeccionAsync(int codigoSeccion, string usuarioLog)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.VarChar).Value = codigoSeccion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuarioLog;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;

                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSeccionSlipTableWriter :: EliminarSeccionAsync", ex);
                }
            }
        }
        
    }
}
