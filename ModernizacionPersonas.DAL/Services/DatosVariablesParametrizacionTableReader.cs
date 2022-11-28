using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosVariablesParametrizacionTableReader : IDatosVariablesParametrizacionReader
    {
        private const string SP_NAME = "PMP.USP_TB_VariablesParametrizacion";

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var result = new List<VariableSlipParametrizacion>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var permiso = new VariableSlipParametrizacion
                            {
                                CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_variable"),
                                NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable"),
                                DescripcionVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_variable"),
                                TipoDato = SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_dato"),
                                ValorTope = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_tope_variable"),
                                ValorVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_variable"),
                                Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                                Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                                Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                                FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                            };

                            result.Add(permiso);
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableReader :: GetVariablesAsync", ex);
                }
            }
        }

        public async Task<VariableSlipParametrizacion> GetVariableAsync(int codigoVariable)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = codigoVariable;

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var result = new VariableSlipParametrizacion();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_variable");
                        result.NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable");
                        result.DescripcionVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_variable");
                        result.TipoDato = SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_dato");
                        result.ValorTope = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_tope_variable");
                        result.ValorVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_variable");
                        result.Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo");
                        result.Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario");
                        result.Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc");
                        result.FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento");
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableReader :: GetVariableAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetVariablesTextoAsync(int codigoTexto)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_texto_parametrizacion", SqlDbType.Int).Value = codigoTexto;

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 11;
                cmd.Connection = conn;
                var result = new List<VariableSlipParametrizacion>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new VariableSlipParametrizacion
                        {
                            CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_permiso"),
                            NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable"),
                            DescripcionVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_variable"),
                            TipoDato = SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_dato"),
                            ValorTope = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_tope_variable"),
                            ValorVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_variable"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableReader :: GetVariablesTextoAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<VariableSlipParametrizacion>> GetUnusedVariablesAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;
                var result = new List<VariableSlipParametrizacion>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new VariableSlipParametrizacion
                        {
                            CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_variable"),
                            NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable"),
                            DescripcionVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_variable"),
                            TipoDato = SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_dato"),
                            ValorTope = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_tope_variable"),
                            ValorVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_valor_variable"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosVariablesParametrizacionTableReader :: GetUnusedVariablesAsync", ex);
                }
            }
        }

    }
}
