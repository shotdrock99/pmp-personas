using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSiniestralidadTableWriter : IDatosSiniestralidadWriter
    {
        public string command = "PMP.USP_TB_Siniestralidad";
        public async Task<int> CrearSiniestralidadAsync(Siniestralidad model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_anho", SqlDbType.Int).Value = model.Anno;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio", SqlDbType.Date).Value = model.FechaInicial;
                cmd.Parameters.Add("@VAR_DA_fecha_fin", SqlDbType.Date).Value = model.FechaFinal;
                cmd.Parameters.Add("@VAR_MO_valor_incurrido", SqlDbType.Money).Value = model.ValorIncurrido;
                cmd.Parameters.Add("@VAR_IN_numero_casos", SqlDbType.VarChar).Value = model.NumeroCasos;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSiniestralidadTableWriter :: CrearSiniestralidadAsync", ex);
                }
            }
        }

        public async Task ActualizarSiniestralidadAsync(Siniestralidad model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = command
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_anho", SqlDbType.Int).Value = model.Anno;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio", SqlDbType.Date).Value = model.FechaInicial;
                cmd.Parameters.Add("@VAR_DA_fecha_fin", SqlDbType.Date).Value = model.FechaFinal;
                cmd.Parameters.Add("@VAR_MO_valor_incurrido", SqlDbType.Money).Value = model.ValorIncurrido;
                cmd.Parameters.Add("@VAR_IN_numero_casos", SqlDbType.VarChar).Value = model.NumeroCasos;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSiniestralidadTableWriter :: ActualizarSiniestralidadAsync", ex);
                }
            }
        }

        public async Task EliminarSiniestralidadAsync(int codigoCotizacion)
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
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSiniestralidadTableWriter :: EliminarSiniestralidadAsync", ex);
                }
            }
        }
    }
}
