using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosRangoGrupoAseguradoTableWriter : IDatosRangoGrupoAseguradoWriter
    {
        private const string SP_NAME = "PMP.USP_TB_RangoGrupoAsegurado";
        public async Task<int> CrearRangoGrupoAseguradoAsync(Rango model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_edad_minima", SqlDbType.Int).Value = model.EdadMinAsegurado;
                cmd.Parameters.Add("@VAR_IN_edad_maxima", SqlDbType.Int).Value = model.EdadMaxAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_asegurado", SqlDbType.Money).Value = model.ValorAsegurado;
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
                    throw new Exception("DatosRangoGrupoAseguradoTableWriter :: CrearRangoGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task ActualizarRangoGrupoAseguradoAsync(int codigoRangoGrupoAsegurado, Rango model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_rango_grupo_asegurado", SqlDbType.Int).Value = codigoRangoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_NM_tasa_riesgo", SqlDbType.Decimal).Value = model.TasaRiesgo;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial", SqlDbType.Decimal).Value = model.TasaComercial;
                cmd.Parameters.Add("@VAR_MO_valor_prima_basico", SqlDbType.Money).Value = model.ValorPrimaBasico;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRangoGrupoAseguradoTableWriter :: ActualizarRangoGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task EliminarRangoGrupoAseguradoAsync(int CodigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = CodigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRangoGrupoAseguradoTableWriter :: EliminarRangoGrupoAseguradoAsync", ex);
                }
            }
        }
    }
}
