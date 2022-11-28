using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosCausalTableReader : IDatosCausalReader
    {

        public async Task<IEnumerable<Causal>> GetCausales()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };


                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;
                var causales = new List<Causal>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var causal = new Causal
                        {
                            CodigoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_causal"),
                            CausalTexto = SqlReaderUtilities.SafeGet<string>(reader, "VC_causal"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Externo = SqlReaderUtilities.SafeGet<int>(reader, "IN_externo"),
                            Solidaria = SqlReaderUtilities.SafeGet<int>(reader, "IN_solidaria"),
                            TipoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_tipo_causal"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        causales.Add(causal);
                    }
                    return causales;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCausal :: LeerCausalessAsync", ex);
                }
            }
        }

        public async Task<Causal> GetCausalId(int codigoCausal)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };

                cmd.Parameters.Add("@VAR_IN_cod_causal", SqlDbType.Int).Value = codigoCausal;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 11;
                cmd.Connection = conn;
                var causal = new Causal();


                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {


                        causal.CodigoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_causal");
                            causal.CausalTexto = SqlReaderUtilities.SafeGet<string>(reader, "VC_causal");
                            causal.Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo");
                            causal.Externo = SqlReaderUtilities.SafeGet<int>(reader, "IN_externo");
                            causal.Solidaria = SqlReaderUtilities.SafeGet<int>(reader, "IN_solidaria");
                            causal.TipoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_tipo_causal");
                            causal.Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario");
                            causal.Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc");
                            causal.FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento");



                    }
                    return causal;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCausal :: GetCausalIdAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<Causal>> GetCausalesAceptacionAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var causales = new List<Causal>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var causal = new Causal
                        {
                            CodigoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_causal"),
                            CausalTexto = SqlReaderUtilities.SafeGet<string>(reader, "VC_causal"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Externo = SqlReaderUtilities.SafeGet<int>(reader, "IN_externo"),
                            Solidaria = SqlReaderUtilities.SafeGet<int>(reader, "IN_solidaria"),
                            TipoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_tipo_causal"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        causales.Add(causal);
                    }
                    return causales;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCausalRechazo :: LeerCausalessAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<Causal>> LeerCausalesRechazoAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Causal"
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var causales = new List<Causal>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var causal = new Causal
                        {
                            CodigoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_causal"),
                            CausalTexto = SqlReaderUtilities.SafeGet<string>(reader, "VC_causal"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Externo = SqlReaderUtilities.SafeGet<int>(reader, "IN_externo"),
                            Solidaria = SqlReaderUtilities.SafeGet<int>(reader, "IN_solidaria"),
                            TipoCausal = SqlReaderUtilities.SafeGet<int>(reader, "IN_tipo_causal"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        causales.Add(causal);
                    }
                    return causales;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCausalRechazo :: LeerCausalessAsync", ex);
                }
            }
        }
    }
}
