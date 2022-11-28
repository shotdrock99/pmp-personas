using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAsegurabilidadTableWriter : IDatosAsegurabilidadWriter
    {
        public string command = "PMP.USP_TB_Asegurabilidad";
        public async Task<int> CrearAsegurabilidadAsync(Asegurabilidad model, int cdigoCotiacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = cdigoCotiacion;
                cmd.Parameters.Add("@VAR_IN_edad_desde", SqlDbType.Int).Value = model.EdadDesde;
                cmd.Parameters.Add("@VAR_IN_edad_hasta", SqlDbType.Int).Value = model.EdadHasta;
                cmd.Parameters.Add("@VAR_MO_valor_individual_desde", SqlDbType.Money).Value = model.ValorIndividualDesde;
                cmd.Parameters.Add("@VAR_MO_valor_individual_hasta", SqlDbType.Money).Value = model.ValorIndividualHasta;
                cmd.Parameters.Add("@VAR_VC_requisitos", SqlDbType.VarChar).Value = model.Requisitos;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var res = await cmd.ExecuteScalarAsync();
                    var codigoEdades = int.Parse(res.ToString());
                    return codigoEdades;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAsegurabilidadTableWriter :: CrearAsegurabilidadAsync", ex);
                }
            }
        }

        public async Task<bool> ActualizarAsegurabilidadAsync(int codigoCotizacion, Asegurabilidad model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_edad_desde", SqlDbType.Int).Value = model.EdadDesde;
                cmd.Parameters.Add("@VAR_IN_edad_hasta", SqlDbType.Date).Value = model.EdadHasta;
                cmd.Parameters.Add("@VAR_MO_valor_individual_desde", SqlDbType.Money).Value = model.ValorIndividualDesde;
                cmd.Parameters.Add("@VAR_MO_valor_individual_hasta", SqlDbType.Money).Value = model.ValorIndividualHasta;
                cmd.Parameters.Add("@VAR_VC_requisitos", SqlDbType.VarChar).Value = model.Requisitos;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAsegurabilidadTableWriter :: ActualizarAsegurabilidadAsync", ex);
                }
            }
        }

        public async Task<bool> EliminarAsegurabilidadAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAsegurabilidadTableWriter :: EliminarAsegurabilidadAsync", ex);
                }
            }
        }

        public async Task<bool> EliminarAsegurabilidadIdAsync(int codigoCotizacion, int codigoAsegurabilidad)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_asegurabilidad", SqlDbType.Int).Value = codigoAsegurabilidad;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAsegurabilidadTableWriter :: EliminarAsegurabilidadIdAsync", ex);
                }
            }
        }
    }
}
