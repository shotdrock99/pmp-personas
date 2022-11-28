using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SolidariaParametrizacion.Servicios.V2.Models;
using ModernizacionPersonas.Common;

namespace SolidariaParametrizacion.Servicios.V2
{
    public class CotizacionWriterService : ICotizacionWriterService
    {
        public async Task<InicializarCotizacionResponse> CrearCotizacionAsync(InitializeCotizacionViewModel model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PMP.USP_TB_Cotizacion";
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = model.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = model.CodigoSubramo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;
                try
                {
                    var codigoCotizacion = await cmd.ExecuteScalarAsync();
                    var ticks = DateTime.Now.Ticks;
                    return new InicializarCotizacionResponse
                    {
                        Status = CotizacionResponseStatus.Valid,
                        CodigoCotizacion = codigoCotizacion,
                        NumeroCotizacion = $"0000_{ticks}"
                    };
                }
                catch (Exception ex)
                {
                    return new InicializarCotizacionResponse
                    {
                        Status = CotizacionResponseStatus.Invalid,
                        Message = ex.Message
                    };
                }
            }
        }

        public Task ActualizarCotizacionAsync()
        {
            throw new NotImplementedException();
        }

        public Task EliminarCotizacionAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<CotizacionActionResponseBase> InsertarInformacionNegocioAsync(InformacionNegocioViewModel model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PMP.USP_TB_Cotizacion";
                //cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = model.CodigoEstado;
                //cmd.Parameters.Add("@VAR_IN_cod_tomador", SqlDbType.Int).Value = model.CodigoTomador;
                cmd.Parameters.Add("@VAR_IN_codigo_periodo", SqlDbType.Int).Value = model.CodigoPeriodoFacturacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_riesgo", SqlDbType.Int).Value = model.CodigoTipoRiesgo;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_contratacion", SqlDbType.Int).Value = model.CodigoTipoContratacion;
                cmd.Parameters.Add("@VAR_SN_negocio_directo", SqlDbType.Int).Value = model.EsNegocioDirecto;
                //cmd.Parameters.Add("@VAR_MO_valor_poliza", SqlDbType.Int).Value = model.ValorPoliza;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                    var ticks = DateTime.Now.Ticks;
                    return new CotizacionActionResponseBase
                    {
                        Status = CotizacionResponseStatus.Valid
                    };
                }
                catch (Exception ex)
                {
                    return new InicializarCotizacionResponse
                    {
                        Status = CotizacionResponseStatus.Invalid,
                        Message = ex.Message
                    };
                }
            }
        }

        public async Task<CotizacionActionResponseBase> InsertarDatosTomadorAsync(int codigoCotizacion, int codigoTomador)
        {

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PMP.USP_TB_Cotizacion";
                cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_tomador", SqlDbType.Int).Value = codigoTomador;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                    return new CotizacionActionResponseBase
                    {
                        Status = CotizacionResponseStatus.Valid
                    };
                }
                catch (Exception ex)
                {
                    return new CotizacionActionResponseBase
                    {
                        Status = CotizacionResponseStatus.Invalid,
                        Message = ex.Message
                    };
                }
            }
        }
    }
}
