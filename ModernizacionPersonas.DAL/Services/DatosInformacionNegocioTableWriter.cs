using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosInformacionNegocioTableWriter : IDatosInformacionNegocioWriter
    {
        private const string SP_NAME = "PMP.USP_TB_Cotizacion";

        public async Task InsertarInformacionNegocioAsync(int codigoCotizacion, InformacionNegocio model, decimal factorG)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_aseguradora_actual", SqlDbType.VarChar).Value = model.NombreAseguradora;
                cmd.Parameters.Add("@VAR_IN_codigo_periodo", SqlDbType.Int).Value = model.CodigoPeriodoFacturacion;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = model.CodigoSector;
                cmd.Parameters.Add("@VAR_IN_cod_perfil_edad", SqlDbType.Int).Value = model.CodigoPerfilEdad;
                cmd.Parameters.Add("@VAR_IN_cod_perfil_valor", SqlDbType.Int).Value = model.CodigoPerfilValor;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_riesgo", SqlDbType.Int).Value = model.CodigoTipoRiesgo;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_negocio", SqlDbType.Int).Value = model.CodigoTipoNegocio;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_contratacion", SqlDbType.Int).Value = model.CodigoTipoContratacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_tasa", SqlDbType.Int).Value = model.CodigoTipoTasa1;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_tasa2", SqlDbType.Int).Value = model.CodigoTipoTasa2;
                cmd.Parameters.Add("@VAR_BT_negocio_directo", SqlDbType.Bit).Value = model.EsNegocioDirecto;
                cmd.Parameters.Add("@VAR_BT_listado_asegurados", SqlDbType.Bit).Value = model.ConListaAsegurados;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio_vigencia", SqlDbType.Date).Value = model.FechaInicio;
                cmd.Parameters.Add("@VAR_DA_fecha_fin_vigencia", SqlDbType.Date).Value = model.FechaFin;
                cmd.Parameters.Add("@VAR_NM_utilidad", SqlDbType.Float).Value = (double)model.UtilidadCompania;
                cmd.Parameters.Add("@VAR_NM_porcentaje_retorno", SqlDbType.Float).Value = (double)model.PorcentajeRetorno;
                cmd.Parameters.Add("@VAR_NM_porcentaje_otros", SqlDbType.Float).Value = (double)model.PorcentajeOtrosGastos;
                cmd.Parameters.Add("@VAR_NM_comision", SqlDbType.Float).Value = (double)model.PorcentajeComision;
                // TODO definir si deben pertenecer al modelo de Cotizacion
                cmd.Parameters.Add("@VAR_NM_gastos_compania", SqlDbType.Float).Value = (double)model.PorcentajeGastosCompania;
                cmd.Parameters.Add("@VAR_NM_IVA_porcentaje_retorno", SqlDbType.Float).Value = (double)model.PorcentajeIvaRetorno;
                cmd.Parameters.Add("@VAR_NM_IVA_comision", SqlDbType.Float).Value = (double)model.PorcentajeIvaComision;
                cmd.Parameters.Add("@VAR_VC_otros_gastos", SqlDbType.VarChar).Value = model.OtrosGastos;
                cmd.Parameters.AddWithValue("@VAR_NM_IBNR", model.IBNR);
                cmd.Parameters.Add("@VAR_IN_anos_siniestros", SqlDbType.Int).Value = model.anyosSiniestralidad;
                //cmd.Parameters.Add("@VAR_MO_valor_poliza", SqlDbType.Money).Value = model.ValorPoliza;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    var result = await cmd.ExecuteScalarAsync();
                    var factorgDb = (decimal)result;
                    if (factorgDb != factorG)
                    {
                        throw new Exception("Se presento un error calculando el FactorG.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: InsertarInformacionNegocioAsync", ex);
                }
            }
        }

        public async Task ActualizarInformacionNegocioAsync(int codigoCotizacion, InformacionNegocio model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                var fechaIni = model.FechaInicio.ToString();
                if (model.FechaInicio.ToString().Contains("01/01/0001")) {
                    model.FechaFin = null;
                    model.FechaInicio = null;
                }
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_NM_utilidad", SqlDbType.Float).Value = (double)model.UtilidadCompania;
                cmd.Parameters.Add("@VAR_NM_gastos_compania", SqlDbType.Float).Value = (double)model.PorcentajeGastosCompania;
                cmd.Parameters.Add("@VAR_NM_porcentaje_retorno", SqlDbType.Float).Value = (double)model.PorcentajeRetorno;
                cmd.Parameters.Add("@VAR_NM_porcentaje_otros", SqlDbType.Float).Value = (double)model.PorcentajeOtrosGastos;
                cmd.Parameters.Add("@VAR_NM_comision", SqlDbType.Float).Value = (double)model.PorcentajeComision;
                cmd.Parameters.Add("@VAR_NM_factorg", SqlDbType.VarChar).Value = (double)model.FactorG;
                cmd.Parameters.Add("@VAR_VC_aseguradora_actual", SqlDbType.VarChar).Value = model.NombreAseguradora;
                cmd.Parameters.Add("@VAR_IN_codigo_periodo", SqlDbType.Int).Value = model.CodigoPeriodoFacturacion;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = model.CodigoSector;
                cmd.Parameters.Add("@VAR_IN_cod_perfil_edad", SqlDbType.Int).Value = model.CodigoPerfilEdad;
                cmd.Parameters.Add("@VAR_IN_cod_perfil_valor", SqlDbType.Int).Value = model.CodigoPerfilValor;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_riesgo", SqlDbType.Int).Value = model.CodigoTipoRiesgo;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_negocio", SqlDbType.Int).Value = model.CodigoTipoNegocio;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_contratacion", SqlDbType.Int).Value = model.CodigoTipoContratacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_tasa", SqlDbType.Int).Value = model.CodigoTipoTasa1;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_tasa2", SqlDbType.Int).Value = model.CodigoTipoTasa2;
                cmd.Parameters.Add("@VAR_BT_negocio_directo", SqlDbType.Bit).Value = model.EsNegocioDirecto;
                cmd.Parameters.Add("@VAR_BT_listado_asegurados", SqlDbType.Bit).Value = model.ConListaAsegurados;
                cmd.Parameters.Add("@VAR_VC_otros_gastos", SqlDbType.VarChar).Value = model.OtrosGastos;
                cmd.Parameters.Add("@VAR_DA_fecha_inicio_vigencia", SqlDbType.DateTime).Value = model.FechaInicio;
                cmd.Parameters.Add("@VAR_DA_fecha_fin_vigencia", SqlDbType.DateTime).Value = model.FechaFin;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: ActualizarInformacionNegocioAsync", ex);
                }
            }
        }

        public async Task ActualizarDirectorComercialAsync(int codigoCotizacion, string codigoComercial, string nombreComercial, string emailComercial)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_director_comercial", SqlDbType.VarChar).Value = codigoComercial;
                cmd.Parameters.Add("@VAR_VC_director_comercial_nombre", SqlDbType.VarChar).Value = nombreComercial;
                cmd.Parameters.Add("@VAR_VC_director_comercial_email", SqlDbType.VarChar).Value = emailComercial;
                //cmd.Parameters.Add("@VAR_NM_factorg", SqlDbType.VarChar).Value = (double)factorg;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 20;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: ActualizarDirectorComercialAsync", ex);
                }
            }
        }

        public async Task UpdateCotizacionSelectedTasaAsync(int codigoCotizacion, decimal TasaSeleccionada)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_NM_tasa_seleccionada", SqlDbType.Int).Value = TasaSeleccionada;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 16;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: UpdateCotizacionSelectedTasaAsync", ex);
                }
            }
        }

        public async Task UpdateSelfAuthorizeFlagASync(int codigoCotizacion, bool selfAuthorize)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_autoriza_control", SqlDbType.Int).Value = selfAuthorize ? 1: 0;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 22;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: UpdateSelfAuthorizeFlagASync", ex);
                }
            }
        }

        public async Task UpdateAnosSiniestrosASync(int codigoCotizacion, int anosSiniestros)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_anos_siniestros", SqlDbType.Int).Value = anosSiniestros;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 23;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: UpdateSelfAuthorizeFlagASync", ex);
                }
            }
        }
    }
}
