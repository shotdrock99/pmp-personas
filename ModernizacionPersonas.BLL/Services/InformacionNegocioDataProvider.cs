using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class InformacionNegocioDataProvider
    {
        private readonly IDatosCotizacionWriter datosCotizacionWriter;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosInformacionNegocioWriter informacionNegocioWriter;
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosIntermediarioWriter intermediarioWriter;
        private readonly IDatosTomadorWriter tomadorWriter;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly CotizacionStateWriter cotizacionStateWriter;

        public InformacionNegocioDataProvider()
        {
            this.datosCotizacionWriter = new DatosCotizacionTableWriter();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.informacionNegocioWriter = new DatosInformacionNegocioTableWriter();
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.intermediarioWriter = new DatosIntermediarioTableWriter();
            this.cotizacionStateWriter = new CotizacionStateWriter();
            this.tomadorWriter = new DatosTomadorTableWriter();
            this.tomadorReader = new DatosTomadorTableReader();
        }

        public async Task<ActionResponseBase> InsertarInformacionNegocioAsync(int codigoCotizacion, int version, InformacionNegocio model)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            if (!model.Equals(informacionNegocio))
            {
                var codigoRamo = informacionNegocio.CodigoRamo;
                var utilidadCompania = await UtilidadesCompaniaReaderService.ReadAsync(codigoRamo);
                var gastosCompania = await GastosCompaniaReader.ReadAsync(codigoRamo);
                var IBNR = await this.informacionPersonasReader.TraerTasaSiniestralidadAsync(codigoRamo);
                // si es Tasa por edad de cada asegurado siempre será con listado de asegurados
                if (model.CodigoTipoTasa1 == 2 || model.CodigoTipoTasa2 == 2)
                {
                    model.ConListaAsegurados = true;
                }

                // actualizar modelo con informacion de utilidad y gastos de compania                        
                model.UtilidadCompania = utilidadCompania.Base;
                model.PorcentajeGastosCompania = gastosCompania.PorcentajeGastos;
                model.PorcentajeIvaComision = gastosCompania.PorcentajeIvaComision;
                model.PorcentajeIvaRetorno = gastosCompania.PorcentajeIvaRetorno;
                model.IBNR = IBNR;

                // Insertar Actividad Tomador
                var tomador = await this.tomadorReader.GetTomadorAsync(codigoCotizacion);
                tomador.Actividad = model.Actividad;

                var factorG = this.CalcularFactorG(model);

                await this.informacionNegocioWriter.InsertarInformacionNegocioAsync(codigoCotizacion, model, factorG);
                await this.informacionNegocioWriter.ActualizarDirectorComercialAsync(codigoCotizacion, model.UsuarioDirectorComercial, model.NombreDirectorComercial, model.EmailDirectorComercial);
                await this.tomadorWriter.ActualizarTomadorAsync(codigoCotizacion, tomador);
                // si cambio a negocio directo, eliminar todos los intermediarios   
                if (model.EsNegocioDirecto && model.EsNegocioDirecto != informacionNegocio.EsNegocioDirecto)
                {
                    await this.intermediarioWriter.DeleteIntermediariosCotizacionAsync(codigoCotizacion);
                }

                // update cotizacion modified flag to true
                await this.datosCotizacionWriter.UpdateModifiedFlagCotizacionAsync(codigoCotizacion, true);
                await this.informacionNegocioWriter.UpdateSelfAuthorizeFlagASync(codigoCotizacion, false);

                // Actualizar el estado de la cotizacion
                if (informacionNegocio.CotizacionState < CotizacionState.OnInformacionNegocio)
                {
                    await this.cotizacionStateWriter.UpdateCotizacionStateAsync(codigoCotizacion, CotizacionState.OnInformacionNegocio);
                }

                return new ActionResponseBase();
            }

            // return 304 Not Modified
            return new ActionResponseBase();
        }

        private decimal CalcularFactorG(InformacionNegocio model)
        {
            var ivaRetorno = model.PorcentajeRetorno * (model.PorcentajeIvaRetorno / 100);
            var ivaComision = model.PorcentajeComision * (model.PorcentajeIvaComision / 100);

            var result = model.PorcentajeOtrosGastos + model.PorcentajeGastosCompania + model.UtilidadCompania + model.PorcentajeRetorno + ivaRetorno + model.PorcentajeComision + ivaComision;

            return result;
        }

        public async Task<ActionResponseBase> SeleccionarTasaAsync(int codigoCotizacion, int version, decimal tasaSeleccionada)
        {
            await this.informacionNegocioWriter.UpdateCotizacionSelectedTasaAsync(codigoCotizacion, tasaSeleccionada);
            return new ActionResponseBase();
        }

        public async Task<ActionResponseBase> UpdateCompanyData(int codigoCotizacion, int version, decimal gastosCompania, decimal utilidadesCompania)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            informacionNegocio.PorcentajeGastosCompania = gastosCompania;
            informacionNegocio.UtilidadCompania = utilidadesCompania;

            await this.informacionNegocioWriter.ActualizarInformacionNegocioAsync(codigoCotizacion, informacionNegocio);

            return new ActionResponseBase();
        }
        public async Task<string> GetUsuarioCreadorAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    //CommandText = "select MAX(CAST(VC_numero_cotizacion as INT)) from PMP.TB_Cotizacion",
                    CommandText = "SELECT VC_cod_usuario FROM PMP.TB_Movimientos WHERE IN_cod_cotizacion = @VAR_cotizacion AND IN_cod_version = @VAR_version AND IN_cod_estado_cotizacion < 1107",
                    Connection = conn
                };
                cmd.Parameters.Add("@VAR_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_version", SqlDbType.Int).Value = version;
                try
                {
                    var response = await cmd.ExecuteScalarAsync();
                    if (response != DBNull.Value)
                    {
                        return response.ToString();
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    throw new Exception("GetusuarioQueryAsync", ex);
                }
            }
        }
    }
}
