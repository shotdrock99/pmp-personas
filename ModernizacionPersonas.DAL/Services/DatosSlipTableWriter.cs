using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSlipTableWriter : IDatosSlipWriter
    {
        public async Task GuardarConfiguracionSlip(int codigoCotizacion, SlipVariable variable)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = variable.CodigoVariable;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_valor_variable", SqlDbType.VarChar).Value = variable.Valor;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                // var valoresSlipLista = new List<ValoresSlipViewModel>();

                try
                {
                    await this.ClearValorVariableAsync(codigoCotizacion, variable);
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSlipTableWriter :: GuardarConfiguracionSlip", ex);
                }
            }
        }

        public async Task SeleccionarClausulasSlipAsync(int codigoCotizacion, int seccion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = seccion;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                cmd.Connection = conn;

                try
                {
                    await this.ClearSeleccionClausulaAsync(codigoCotizacion, seccion);
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSlipTableWriter :: SeleccionarClausulasSlip", ex);
                }
            }
        }

        private async Task ClearValorVariableAsync(int codigoCotizacion, SlipVariable variable)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = variable.CodigoVariable;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosSlipTableWriter :: LimpiarValorVariableAsync", ex);
                }
            }
        }

        private async Task ClearSeleccionClausulaAsync(int codigoCotizacion, int seccion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_variable", SqlDbType.Int).Value = seccion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 8;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosSlipTableWriter :: LimpiarSeleccionClausulaAsync", ex);
                }
            }
        }

        public async Task ClearSeleccionClausulaAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PMP.USP_ValoresRegistroSlip";
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 9;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error DatosSlipTableWriter :: LimpiarSeleccionClausulaAsync cotización", ex);
                }
            }
        }
    }
}
