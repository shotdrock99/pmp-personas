using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosOpcionValorAseguradoTableReader : IDatosOpcionValorAseguradoReader
    {

        public async Task<IEnumerable<OpcionValorAsegurado>> LeerOpcionValorAseguradoAsync(int codigoAmparoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_OpcionValorAsegurado"
                };

                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = codigoAmparoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var opciones = new List<OpcionValorAsegurado>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var opcion = new OpcionValorAsegurado
                        {
                            CodigoAmparoGrupoAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo_grupo_asegurado"),
                            CodigoOpcionValorAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_opcion_valor_asegurado"),
                            IndiceOpcion = SqlReaderUtilities.SafeGet<int>(reader, "IN_indice_opcion"),
                            NumeroSalarios = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_numero_salarios"),
                            PorcentajeCobertura = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_porcentaje_cobertura"),
                            TasaRiesgo = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_tasa_riesgo"),
                            TasaComercial = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_tasa_comercial"),
                            Prima = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_prima"),
                            ValorDiario = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_diario"),
                            NumeroDias = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_numero_dias"),
                            ValorAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_asegurado"),
                            ValorAseguradoDias = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_asegurado"),
                            NumeroAsegurados = SqlReaderUtilities.SafeGet<int>(reader, "IN_numero_asegurados"),

                        };

                        opciones.Add(opcion);
                    }

                    return opciones;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosOpcionValorAseguradoTableReader :: LeerOpcionValorAseguradoAsync", ex);
                }
            }
        }

        public async Task<decimal> TraerSumatoriaOpcionValorAseguradoAsync(int codigoGrupoAsegurado, int opcionIndice)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_OpcionValorAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = opcionIndice;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;

                try
                {
                    var sumatoria = await cmd.ExecuteScalarAsync();
                    return (decimal)sumatoria;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosOpcionValorAseguradoTableWriter :: TraerSumatoriaOpcionValorAseguradoAsync", ex);
                }
            }
        }
    }
}
