using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosOpcionValorAseguradoTableWriter : IDatosOpcionValorAseguradoWriter
    {
        private const string SP_NAME = "PMP.USP_TB_OpcionValorAsegurado";

        public async Task<int> CrearOpcionValorAseguradoAsync(OpcionValorAsegurado model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = model.IndiceOpcion;
                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = model.CodigoAmparoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_NM_porcentaje_cobertura", SqlDbType.Decimal).Value = model.PorcentajeCobertura;
                cmd.Parameters.Add("@VAR_NM_numero_salarios", SqlDbType.Decimal).Value = model.NumeroSalarios;
                cmd.Parameters.Add("@VAR_MO_valor_asegurado", SqlDbType.Money).Value = model.ValorAsegurado;
                cmd.Parameters.Add("@VAR_MO_prima", SqlDbType.Money).Value = model.Prima;
                cmd.Parameters.Add("@VAR_NM_numero_dias", SqlDbType.Decimal).Value = model.NumeroDias;
                cmd.Parameters.Add("@VAR_MO_valor_diario", SqlDbType.Money).Value = model.ValorDiario;
                cmd.Parameters.Add("@VAR_IN_numero_asegurados", SqlDbType.Int).Value = model.NumeroAsegurados;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: CrearOpcionValorAseguradoAsync", ex);
                }
            }
        }

        public async Task UpdateOpcionValorAseguradoAsync(int codigoOpcionValorAsegurado, OpcionValorAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = model.IndiceOpcion;
                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = model.CodigoAmparoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_NM_porcentaje_cobertura", SqlDbType.Decimal).Value = model.PorcentajeCobertura;
                cmd.Parameters.Add("@VAR_NM_numero_salarios", SqlDbType.Decimal).Value = model.NumeroSalarios;
                cmd.Parameters.Add("@VAR_MO_valor_asegurado", SqlDbType.Money).Value = model.ValorAsegurado;
                cmd.Parameters.Add("@VAR_MO_prima", SqlDbType.Money).Value = model.Prima;
                cmd.Parameters.Add("@VAR_IN_cod_opcion_valor_asegurado", SqlDbType.Int).Value = codigoOpcionValorAsegurado;
                cmd.Parameters.Add("@VAR_NM_numero_dias", SqlDbType.Decimal).Value = model.NumeroDias;
                cmd.Parameters.Add("@VAR_MO_valor_diario", SqlDbType.Money).Value = model.ValorDiario;
                cmd.Parameters.Add("@VAR_IN_numero_asegurados", SqlDbType.Int).Value = model.NumeroAsegurados;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: UpdateOpcionValorAseguradoAsync", ex);
                }
            }
        }

        public async Task UpdateTasaRiesgoAmparoAsync(int codigoOpcionValorAsegurado, decimal tasaRiesgo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_NM_tasa_riesgo", SqlDbType.Decimal).Value = tasaRiesgo;
                cmd.Parameters.Add("@VAR_IN_cod_opcion_valor_asegurado", SqlDbType.Int).Value = codigoOpcionValorAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: UpdateTasaRiesgoAmparoAsync", ex);
                }
            }
        }

        public async Task UpdateTasasPrimasAmparoAsync(int codigoOpcionValorAsegurado, decimal tasaRiesgo, decimal tasaComercial, decimal prima)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_NM_tasa_riesgo", SqlDbType.Float).Value = tasaRiesgo;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial", SqlDbType.Float).Value = tasaComercial;
                cmd.Parameters.Add("@VAR_MO_prima", SqlDbType.Money).Value = prima;
                cmd.Parameters.Add("@VAR_IN_cod_opcion_valor_asegurado", SqlDbType.Int).Value = codigoOpcionValorAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: UpdateTasasPrimasAmparoAsync", ex);
                }
            }
        }

        public async Task DeleteOpcionValorAseguradoAsync(int codigoOpcionValorAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_opcion_valor_asegurado", SqlDbType.Int).Value = codigoOpcionValorAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: DeleteOpcionValorAseguradoAsync", ex);
                }
            }
        }

        public async Task UpdatePonderacionAmparoAsync(int codigoOpcionValorAsegurado, int indiceOpcion, decimal ponderacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_NM_ponderacion", SqlDbType.Decimal).Value = ponderacion;
                cmd.Parameters.Add("@VAR_IN_cod_opcion_valor_asegurado", SqlDbType.Int).Value = codigoOpcionValorAsegurado;
                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = indiceOpcion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 8;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosOpcionValorAseguradoTableWriter :: UpdatePonderacionAmparoAsync", ex);
                }
            }
        }
    }
}
