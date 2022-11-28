using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace ModernizacionPersonas.DAL.Services
{
    public class AuthorizationsDataTableReader : IAuthorizationsDataReader
    {
        const string SP_NAME = "PMP.USP_TB_Autorizaciones";

        public async Task<GetAuthorizationControlsResponse> GetAuthorizationsByCotizacionAsync(int codigoCotizacion, int versionCot)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = versionCot;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var result = new List<CotizacionAuthorization>();
                try
                {
                    var version = versionCot;
                    var codigoEstado = 0;
                    var numeroCotizacion = "";
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                        codigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                        var requireAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_requiere_autorizacion");
                        var siseAuth = reader["IN_cod_amparo"].GetType() != typeof(DBNull);

                        var item = new CotizacionAuthorization
                        {
                            CodigoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_autorizacion"),
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            Version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version"),
                            CodigoGrupoAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_grupo_asegurado"),
                            CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_suc"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            CampoEntrada = SqlReaderUtilities.SafeGet<string>(reader, "VC_campo_entrada"),
                            ValorEntrada = SqlReaderUtilities.SafeGet<decimal>(reader, "NM_valor_entrada"),
                            CodigoTipoAutorizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_autorizacion"),
                            RequiereAutorizacion = requireAutorizacion == 1 ? true : false,
                            CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            MensajeValidacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_txt_respuesta"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_seccion"),
                            SiseAuth = siseAuth
                        };

                        if (requireAutorizacion == 1)
                        {
                            result.Add(item);
                        }
                    }

                    return new GetAuthorizationControlsResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Version = version,
                        CodigoEstadoCotizacion = codigoEstado,
                        NumeroCotizacion = numeroCotizacion,
                        Authorizations = result
                    };
                }

                catch (Exception ex)
                {
                    throw new Exception("AuthorizationsDataTableReader :: GetAuthorizationsByCodigoCotizacion", ex);
                }
            }
        }
    }
}
