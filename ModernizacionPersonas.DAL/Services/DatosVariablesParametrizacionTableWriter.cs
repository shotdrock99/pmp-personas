using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosVariablesParametrizacionTableWriter : IDatosVariablesParametrizacionWriter
    {
        private const string SP_NAME = "PMP.USP_TB_VariablesParametrizacion";
        public async Task GuardarVariableAsync(VariableSlipParametrizacion model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_VC_nombre_variable", SqlDbType.VarChar).Value = model.NombreVariable;
                cmd.Parameters.Add("@VAR_VC_descripcion_variable", SqlDbType.VarChar).Value = model.DescripcionVariable;
                cmd.Parameters.Add("@VAR_VC_tipo_dato", SqlDbType.VarChar).Value = model.TipoDato;
                cmd.Parameters.Add("@VAR_IN_valor_tope_variable", SqlDbType.Int).Value = model.ValorTope;
                cmd.Parameters.Add("@VAR_IN_valor_variable", SqlDbType.Int).Value = model.ValorVariable;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableWriter :: GuardarVariableAsync", ex);
                }
            }
        }

        public async Task ActualizarVariableAsync(VariableSlipParametrizacion model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = model.CodigoVariable;
                cmd.Parameters.Add("@VAR_VC_nombre_variable", SqlDbType.VarChar).Value = model.NombreVariable;
                cmd.Parameters.Add("@VAR_VC_descripcion_variable", SqlDbType.VarChar).Value = model.DescripcionVariable;
                cmd.Parameters.Add("@VAR_VC_tipo_dato", SqlDbType.VarChar).Value = model.TipoDato;
                cmd.Parameters.Add("@VAR_IN_valor_tope_variable", SqlDbType.Int).Value = model.ValorTope;
                cmd.Parameters.Add("@VAR_IN_valor_variable", SqlDbType.Int).Value = model.ValorVariable;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableWriter :: ActualizarVariableAsync", ex);
                }
            }
        }

        public async Task DesactivarVariableAsync(int codigoVariable, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = codigoVariable;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 51;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableWriter :: DesactivarVariableAsync", ex);
                }
            }
        }

        public async Task EliminarVariableAsync(int codigoVariable, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = codigoVariable;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableWriter :: EliminarVariableAsync", ex);
                }
            }
        }
    }
}
