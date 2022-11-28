using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosGrupoAseguradoTableWriter : IDatosGrupoAseguradoWriter
    {
        private const string SP_NAME = "PMP.USP_TB_GrupoAsegurado";

        public async Task<int> CrearGrupoAseguradoAsync(GrupoAsegurado model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_VC_nombre_grupo_asegurado", SqlDbType.VarChar).Value = model.NombreGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_suma_asegurada", SqlDbType.Int).Value = model.CodigoTipoSuma;
                cmd.Parameters.Add("@VAR_MO_valor_min_asegurado", SqlDbType.Money).Value = model.ValorMinAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_max_asegurado", SqlDbType.Money).Value = model.ValorMaxAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                //TODO actualizar SP para recibir valor de ConListaAsegurados
                //cmd.Parameters.Add("", SqlDbType.Int).Value = model.ConListaAsegurados;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: CrearGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task ActualizarGrupoAseguradoAsync(int codigoGrupoAsegurado, GrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_VC_nombre_grupo_asegurado", SqlDbType.VarChar).Value = model.NombreGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_suma_asegurada", SqlDbType.Int).Value = model.CodigoTipoSuma;
                cmd.Parameters.Add("@VAR_MO_valor_min_asegurado", SqlDbType.Money).Value = model.ValorMinAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_max_asegurado", SqlDbType.Money).Value = model.ValorMaxAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_distribucion", SqlDbType.Int).Value = model.ConDistribucionAsegurados ? 1 : 0;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: ActualizarGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task InsertarNumAseguradosAsync(int codigoGrupoAsegurado, int numeroAsegurados, int edadPromedio, int porcentajeAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_numero_asegurados", SqlDbType.Int).Value = numeroAsegurados;
                cmd.Parameters.Add("@VAR_IN_edad_promedio", SqlDbType.Int).Value = edadPromedio;
                cmd.Parameters.Add("@VAR_IN_porcentaje_asegurado", SqlDbType.Int).Value = porcentajeAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    var res = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: InsertarNumAseguradosAsync", ex);
                }
            }
        }

        public async Task ActualizarGrupoAseguradoAsync(GrupoAsegurado grupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                //cmd.Parameters.Add("@VAR_TN_listado_asegurado", SqlDbType.Int).Value = grupoAsegurado.ConListaAsegurados ? 1 : 0;
                cmd.Parameters.AddWithValue("@VAR_MO_valor_total_asegurado", grupoAsegurado.ValorAsegurado);
                cmd.Parameters.Add("@VAR_IN_numero_asegurados", SqlDbType.Int).Value = grupoAsegurado.NumeroAsegurados;
                cmd.Parameters.Add("@VAR_IN_edad_promedio", SqlDbType.Int).Value = grupoAsegurado.EdadPromedioAsegurados;
                cmd.Parameters.Add("@VAR_MO_valor_min_asegurado", SqlDbType.Money).Value = grupoAsegurado.ValorMinAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_max_asegurado", SqlDbType.Money).Value = grupoAsegurado.ValorMaxAsegurado;
                cmd.Parameters.Add("@VAR_IN_porcentaje_asegurado", SqlDbType.Int).Value = grupoAsegurado.PorcentajeAsegurados;
                cmd.Parameters.Add("@VAR_IN_tipo_estructura", SqlDbType.Int).Value = grupoAsegurado.TipoEstructura;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = grupoAsegurado.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_distribucion", SqlDbType.Int).Value = grupoAsegurado.ConDistribucionAsegurados ? 1 : 0;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    var res = await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: ActualizarGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task ActualizarConListadoAsync(int codigoGrupoAsegurado, bool conListado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_TN_listado_asegurado", SqlDbType.Int).Value = conListado ? 1 : 0;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: ActualizarConListadoAsync", ex);
                }
            }
        }

        public async Task ActualizarConDistribucionAsync(int codigoGrupoAsegurado, bool conDistribucion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_distribucion", SqlDbType.Int).Value = conDistribucion ? 1 : 0;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: ActualizarConListadoAsync", ex);
                }
            }
        }

        public async Task EliminarGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: EliminarGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task LimpiarGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: LimpiarGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task UpdateNombreGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_VC_nombre_grupo_asegurado", SqlDbType.VarChar).Value = model.NombreGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 8;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: UpdateNombreGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task UpdateTipoEstructuraGrupoAseguradoAsync(int codigoCotizacion, GrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_tipo_estructura", SqlDbType.Int).Value = model.TipoEstructura;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 9;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGrupoAseguradoTableWriter :: UpdateNombreGrupoAseguradoAsync", ex);
                }
            }
        }
    }
}
