using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTextoParametrizacionTableWriter : IDatosTextosParametrizacionWriter
    {
        private const string SP_NAME = "PMP.USP_TB_TextosParametrizacion";
        public async Task<int> CreateTextoParametrizacionAsync(TextoSlip model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = model.CodigoSeccion;
                cmd.Parameters.Add("@VAR_TX_texto_parametrizacion", SqlDbType.VarChar).Value = model.Texto;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var response = await cmd.ExecuteScalarAsync();
                    return int.Parse(response.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableWriter :: GuardarTextoParametrizacionAsync", ex);
                }
            }
        }

        public async Task UpdateTextoParametrizacionAsync(TextoSlip model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_texto_parametrizacion", SqlDbType.VarChar).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = model.CodigoSeccion;
                cmd.Parameters.Add("@VAR_TX_texto_parametrizacion", SqlDbType.VarChar).Value = model.Texto;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = model.Usuario;

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableWriter :: ActualizarTextoParametrizacionAsync", ex);
                }
            }
        }

        public async Task DeleteTextoParametrizacionAsync(int codigoTextoParametrizacion, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_texto_parametrizacion", SqlDbType.VarChar).Value = codigoTextoParametrizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableWriter :: EliminarTextoParametrizacionAsync", ex);
                }
            }
        }
    }
}
