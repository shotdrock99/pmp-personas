using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace ModernizacionPersonas.DAL.Services
{
    public class DatosInformacionNegocioTableReader : IDatosInformacionNegocioReader
    {
        const string SP_NAME = "PMP.USP_TB_Cotizacion";

        public async Task<InformacionNegocio> LeerInformacionNegocioAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,

                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var result = new InformacionNegocio();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var codigoTipoTasa1 = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_tasa");
                        var codigoTipoTasa2 = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_tasa2");
                        var esNegocioDirecto = SqlReaderUtilities.SafeGet<bool>(reader, "BT_negocio_directo");
                        var changed = SqlReaderUtilities.SafeGet<int>(reader, "IN_modificado");
                        var blocked = SqlReaderUtilities.SafeGet<int>(reader, "Bloqueado");
                        var selfAuthorize = SqlReaderUtilities.SafeGet<int>(reader, "IN_autoriza_control");

                        result.CodigoCotizacion = codigoCotizacion;
                        result.NumeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                        result.Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version_consecutivo");
                        result.CodigoEstadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        result.CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_agencia");
                        result.CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo");
                        result.CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo");
                        result.CodigoZona = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_zona");
                        result.CodigoPeriodoFacturacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_codigo_periodo");
                        result.CodigoTipoNegocio = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_negocio");
                        result.CodigoTipoContratacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_contratacion");
                        result.CodigoTipoRiesgo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_riesgo");
                        result.CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector");
                        result.FechaInicio = SqlReaderUtilities.SafeGet<DateTime>(reader, "DA_fecha_inicio_vigencia");
                        result.FechaFin = SqlReaderUtilities.SafeGet<DateTime>(reader, "DA_fecha_fin_vigencia");
                        result.CodigoPerfilEdad = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_perfil_edad");
                        result.CodigoPerfilValor = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_perfil_valor");
                        result.CodigoTipoTasa1 = codigoTipoTasa1;
                        result.CodigoTipoTasa2 = codigoTipoTasa2;
                        result.EsNegocioDirecto = esNegocioDirecto;
                        result.ConListaAsegurados = SqlReaderUtilities.SafeGet<bool>(reader, "BT_listado_asegurados");
                        result.PorcentajeRetorno = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_porcentaje_retorno");
                        result.PorcentajeOtrosGastos = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_porcentaje_otros");
                        result.OtrosGastos = SqlReaderUtilities.SafeGet<string>(reader, "VC_otros_gastos");
                        result.PorcentajeComision = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_comision");
                        //result.PorcentajeComision = reader["NM_comision"] == DBNull.Value ? 0 : (decimal)reader["NM_comision"];
                        result.PorcentajeGastosCompania = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_gastos_compania");
                        result.PorcentajeIvaRetorno = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_IVA_porcentaje_retorno");
                        result.PorcentajeIvaComision= SqlReaderUtilities.SafeGet<decimal>(reader, "NM_IVA_comision");
                        result.UtilidadCompania = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_utilidad");
                        result.NombreAseguradora = SqlReaderUtilities.SafeGet<string>(reader, "VC_aseguradora_actual");
                        result.FactorG = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_factorg");
                        result.IBNR = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_IBNR");
                        result.UsuarioNotificado = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario_notificado");
                        result.LastAuthorId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_usuario_ultima_modificacion");
                        result.LastAuthorName = SqlReaderUtilities.SafeGet<string>(reader, "VC_username_ultima_modificacion");
                        result.CotizacionChanged = changed == 1 ? true : false;
                        result.Bloqueado = blocked == 1 ? true : false;
                        result.BloqueadoBy = SqlReaderUtilities.SafeGet<string>(reader, "BloqueadoBy");
                        result.UsuarioDirectorComercial = SqlReaderUtilities.SafeGet<string>(reader, "VC_director_comercial");
                        result.NombreDirectorComercial = SqlReaderUtilities.SafeGet<string>(reader, "VC_director_comercial_nombre");
                        result.EmailDirectorComercial = SqlReaderUtilities.SafeGet<string>(reader, "VC_director_comercial_email");
                        result.anyosSiniestralidad = SqlReaderUtilities.SafeGet<int>(reader, "IN_anos_siniestros");
                        result.VersionCopia = SqlReaderUtilities.SafeGet<int>(reader, "version_copia");
                        result.SelfAuthorize = selfAuthorize == 1 ? true : false;

                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableReader :: LeerInformacionNegocioResumenAsync", ex);
                }
            }
        }
    }
}
