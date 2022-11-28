using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosParametrizacionEmailTableWriter : IDatosParametrizacionEmailWriter
    {
        private const string SP_NAME = "PMP.USP_TB_ParametrizacionEmail";
        public async Task GuardarEmailParametrizacionAsync(EmailParametrizacion model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_tom_comercial", SqlDbType.Int).Value = model.CodigoTomadorComercial;
                cmd.Parameters.Add("@VAR_IN_cod_seccion_email", SqlDbType.Int).Value = model.CodigoSeccion;
                cmd.Parameters.Add("@VAR_IN_cod_template", SqlDbType.Int).Value = model.CodigoTemplate;
                cmd.Parameters.Add("@VAR_VC_texto_email", SqlDbType.VarChar).Value = model.Texto;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionEmailTableWriter :: GuardarEmailParametrizacionAsync", ex);
                }
            }
        }

        public async Task ActualizarEmailParametrizacionAsync(EmailParametrizacion model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_parametrizacion_email", SqlDbType.Int).Value = model.CodigoParametrizacionEmail;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_tom_comercial", SqlDbType.Int).Value = model.CodigoTomadorComercial;
                cmd.Parameters.Add("@VAR_IN_cod_seccion_email", SqlDbType.Int).Value = model.CodigoSeccion;
                cmd.Parameters.Add("@VAR_IN_cod_template", SqlDbType.Int).Value = model.CodigoTemplate;
                cmd.Parameters.Add("@VAR_VC_texto_email", SqlDbType.VarChar).Value = model.Texto;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionEmailTableWriter :: ActualizarEmailParametrizacionAsync", ex);
                }
            }
        }

        public async Task EliminarEmailParametrizacionAsync(int codigoEmailParametrizacion, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_parametrizacion_email", SqlDbType.VarChar).Value = codigoEmailParametrizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionEmailTableWriter :: EliminarEmailParametrizacionAsync", ex);
                }
            }
        }
        
    }
}
