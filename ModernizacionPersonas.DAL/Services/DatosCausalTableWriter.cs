using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosCausalTableWriter : IDatosCausalWriter
    {
        public async Task GuardarCausalAsync(Causal causal)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };
                cmd.Parameters.Add("@VAR_VC_causal", SqlDbType.VarChar).Value = causal.CausalTexto;
                cmd.Parameters.Add("@VAR_IN_externo", SqlDbType.Int).Value = causal.Externo;
                cmd.Parameters.Add("@VAR_IN_solidaria", SqlDbType.Int).Value = causal.Solidaria;
                cmd.Parameters.Add("@VAR_IN_tipo_causal", SqlDbType.Int).Value = causal.TipoCausal;                
                cmd.Parameters.Add("@VAR_IN_activo", SqlDbType.Int).Value = causal.Activo;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = causal.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCausalAceptacionTableWriter :: GuardarCausalesAceptacionAsync", ex);
                }
            }
        }

        public async Task ActualizarCausalAsync(Causal causal)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };
                cmd.Parameters.Add("@VAR_IN_cod_causal", SqlDbType.VarChar).Value = causal.CodigoCausal;
                cmd.Parameters.Add("@VAR_VC_causal", SqlDbType.VarChar).Value = causal.CausalTexto;
                cmd.Parameters.Add("@VAR_IN_externo", SqlDbType.Int).Value = causal.Externo;
                cmd.Parameters.Add("@VAR_IN_solidaria", SqlDbType.Int).Value = causal.Solidaria;
                cmd.Parameters.Add("@VAR_IN_tipo_causal", SqlDbType.Int).Value = causal.TipoCausal;
                cmd.Parameters.Add("@VAR_IN_activo", SqlDbType.Int).Value = causal.Activo;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = causal.Usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCausalAceptacionTableWriter :: ActualizarCausalAsync", ex);
                }
            }
        }

        public async Task EliminarCausalAsync(int codigoCausal, string usuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };
                cmd.Parameters.Add("@VAR_IN_cod_causal", SqlDbType.VarChar).Value = codigoCausal;
                cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = usuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCausalAceptacionTableWriter :: EliminarCausalAsync", ex);
                }
            }
        }
    }
}
