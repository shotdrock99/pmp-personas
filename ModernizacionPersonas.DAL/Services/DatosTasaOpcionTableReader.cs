using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTasaOpcionTableReader : IDatosTasaOpcionReader
    {
        public string command = "PMP.USP_TB_TasaOpcion";

        public async Task<TasaOpcion> LeerTasaOpcionAsync(int codigoGrupoAsegurado, int indiceOpcion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = indiceOpcion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;

                var result = new TasaOpcion();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.CodigoGrupoAsegurado = (int)reader["IN_cod_grupo_asegurado"];
                        result.IndiceOpcion = (int)reader["IN_indice_opcion"];
                        result.SumatoriaTasa = (decimal)reader["NM_sumatoria_tasa"];
                        result.TasaComercial = (decimal)reader["NM_tasa_comercial"];
                        result.TasaComercialTotal = (decimal)reader["NM_tasa_comercial_aplicada"];
                        result.Descuento = reader["NM_descuento"] != DBNull.Value ? (decimal)reader["NM_descuento"] : 0;
                        result.Recargo = reader["NM_recargo"] != DBNull.Value ? (decimal)reader["NM_recargo"] : 0;
                        result.TasaSiniestralidad = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_tasa_siniestralidad");
                        result.PrimaIndividual = (decimal)reader["MO_valor_prima_individual"];
                        result.PrimaTotal = (decimal)reader["MO_valor_prima"];
                        result.DescuentoSiniestralidad = reader["NM_descuento_siniestralidad"] != DBNull.Value ? (decimal)reader["NM_descuento_siniestralidad"] : 0;
                        result.RecargoSiniestralidad = reader["NM_recargo_siniestralidad"] != DBNull.Value ? (decimal)reader["NM_recargo_siniestralidad"] : 0;
                        result.TasaSiniestralidadTotal = reader["NM_tasa_siniestralidad_aplicada"] != DBNull.Value ? SqlReaderUtilities.SafeGet<decimal>(reader, "NM_tasa_siniestralidad_aplicada") : 0;
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTasaOpcionTableReader :: LeerTasaOpcionAsync", ex);
                }
            }
        }
    }
}
