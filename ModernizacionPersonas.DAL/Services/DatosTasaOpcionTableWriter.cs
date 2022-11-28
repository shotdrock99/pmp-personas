using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTasaOpcionTableWriter : IDatosTasaOpcionWriter
    {
        public string command = "PMP.USP_TB_TasaOpcion";
        public async Task CrearTasaOpcionAsync(TasaOpcion model)
        {
            //throw new NotImplementedException();            

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = model.IndiceOpcion;
                cmd.Parameters.Add("@VAR_NM_sumatoria_tasa", SqlDbType.Decimal).Value = model.SumatoriaTasa;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial", SqlDbType.Decimal).Value = model.TasaComercial;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial_aplicada", SqlDbType.Decimal).Value = model.TasaComercialTotal;
                cmd.Parameters.Add("@VAR_NM_descuento", SqlDbType.Decimal).Value = model.Descuento;
                cmd.Parameters.Add("@VAR_NM_recargo", SqlDbType.Decimal).Value = model.Recargo;
                cmd.Parameters.Add("@VAR_NM_tasa_siniestralidad", SqlDbType.Decimal).Value = model.TasaSiniestralidad;
                cmd.Parameters.Add("@VAR_MO_valor_prima_individual", SqlDbType.Money).Value = model.PrimaIndividual;
                cmd.Parameters.Add("@VAR_MO_valor_prima", SqlDbType.Money).Value = model.PrimaTotal;
                cmd.Parameters.Add("@VAR_NM_descuento_siniestralidad", SqlDbType.Decimal).Value = model.DescuentoSiniestralidad;
                cmd.Parameters.Add("@VAR_NM_recargo_siniestralidad", SqlDbType.Decimal).Value = model.RecargoSiniestralidad;
                cmd.Parameters.Add("@VAR_NM_tasa_siniestralidad_aplicada", SqlDbType.Decimal).Value = model.TasaSiniestralidadTotal;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosTasaOpcionTableWriter :: CrearTasaOpcionAsync", ex);
                }
            }
        }

        public async Task ActualizarTasaOpcionAsync(TasaOpcion model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };

                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_indice_opcion", SqlDbType.Int).Value = model.IndiceOpcion;
                cmd.Parameters.Add("@VAR_NM_sumatoria_tasa", SqlDbType.Decimal).Value = model.SumatoriaTasa;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial", SqlDbType.Decimal).Value = model.TasaComercial;
                cmd.Parameters.Add("@VAR_NM_tasa_comercial_aplicada", SqlDbType.Decimal).Value = model.TasaComercialTotal;
                cmd.Parameters.Add("@VAR_NM_descuento", SqlDbType.Decimal).Value = model.Descuento;
                cmd.Parameters.Add("@VAR_NM_recargo", SqlDbType.Decimal).Value = model.Recargo;
                cmd.Parameters.Add("@VAR_NM_tasa_siniestralidad", SqlDbType.Decimal).Value = model.TasaSiniestralidad;
                cmd.Parameters.Add("@VAR_MO_valor_prima_individual", SqlDbType.Money).Value = model.PrimaIndividual;
                cmd.Parameters.Add("@VAR_MO_valor_prima", SqlDbType.Money).Value = model.PrimaTotal;
                cmd.Parameters.Add("@VAR_NM_descuento_siniestralidad", SqlDbType.Decimal).Value = model.DescuentoSiniestralidad;
                cmd.Parameters.Add("@VAR_NM_recargo_siniestralidad", SqlDbType.Decimal).Value = model.RecargoSiniestralidad;
                cmd.Parameters.Add("@VAR_NM_tasa_siniestralidad_aplicada", SqlDbType.Decimal).Value = model.TasaSiniestralidadTotal;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosTasaOpcionTableWriter :: ActualizarTasaOpcionAsync", ex);
                }
            }
        }

        public async Task EliminarTasaOpcionAsync(int codigoGrupoAsegurado, int indiceOpcion)
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
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosTasaOpcionTableWriter :: EliminarTasaOpcionAsync", ex);
                }
            }
        }
    }
}
