using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosIntermediarioTableWriter : IDatosIntermediarioWriter
    {
        private const string SP_NAME = "PMP.USP_TB_Intermediario";
        public async Task<int> CreateIntermediarioAsync(int codigoCotizacion, Intermediario model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_intermediario", SqlDbType.Int).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_NM_participacion_intermediario", SqlDbType.Decimal).Value = model.Participacion;
                cmd.Parameters.Add("@VAR_IN_clave_intermediario", SqlDbType.Int).Value = model.Clave;
                cmd.Parameters.Add("@VAR_VC_nombre_intermediario", SqlDbType.VarChar).Value = model.PrimerNombre;
                cmd.Parameters.Add("@VAR_VC_nombre2_intermediario", SqlDbType.VarChar).Value = model.SegundoNombre;
                cmd.Parameters.Add("@VAR_VC_apellido_intermediario", SqlDbType.VarChar).Value = model.PrimerApellido;
                cmd.Parameters.Add("@VAR_VC_apellido2_intermediario", SqlDbType.VarChar).Value = model.SegundoApellido;
                cmd.Parameters.Add("@VAR_VC_email_intermediario", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.Int).Value = model.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = model.NumeroDocumento;
                cmd.Parameters.Add("@VAR_IN_cod_estado_intermediario", SqlDbType.Int).Value = model.CodigoEstado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var codigoIntermediario = 0;
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            codigoIntermediario = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_intermediario");
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return codigoIntermediario;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosIntermediarioTableWriter :: CrearIntermediarioAsync", ex);
                }
            }
        }

        public async Task UpdateIntermediarioAsync(int codigoCotizacion, Intermediario model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_NM_participacion_intermediario", SqlDbType.Decimal).Value = model.Participacion;
                cmd.Parameters.Add("@VAR_VC_nombre_intermediario", SqlDbType.VarChar).Value = model.PrimerNombre;
                cmd.Parameters.Add("@VAR_VC_nombre2_intermediario", SqlDbType.VarChar).Value = model.SegundoNombre;
                cmd.Parameters.Add("@VAR_VC_apellido_intermediario", SqlDbType.VarChar).Value = model.PrimerApellido;
                cmd.Parameters.Add("@VAR_VC_apellido2_intermediario", SqlDbType.VarChar).Value = model.SegundoApellido;
                cmd.Parameters.Add("@VAR_IN_cod_estado_intermediario", SqlDbType.Int).Value = model.CodigoEstado;
                cmd.Parameters.Add("@VAR_IN_cod_intermediario", SqlDbType.Int).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_email_intermediario", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_VC_direccion_intermediario", SqlDbType.VarChar).Value = model.Direccion;
                cmd.Parameters.Add("@VAR_IN_cod_departamento_intermediario", SqlDbType.Int).Value = model.CodigoDepartamento;
                cmd.Parameters.Add("@VAR_IN_cod_municipio_intermediario", SqlDbType.Int).Value = model.CodigoMunicipio;
                cmd.Parameters.Add("@VAR_VC_telefono_intermediario", SqlDbType.VarChar).Value = model.Telefono;
                cmd.Parameters.Add("@VAR_VC_nombre_intermediario_slip", SqlDbType.VarChar).Value = model.IntermediarioSlip;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var codigoIntermediario = 0;
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            codigoIntermediario = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_intermediario");
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        var result = new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            NumeroCotizacion = numeroCotizacion,
                            CodigoEstadoCotizacion = estadoCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosIntermediarioTableWriter :: ActualizarIntermediarioAsync", ex);
                }
            }
        }

        public async Task DeleteIntermediarioAsync(int codigoIntermediario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_intermediario", SqlDbType.Int).Value = codigoIntermediario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosIntermediarioTableWriter :: EliminarIntermediarioAsync", ex);
                }
            }
        }

        public async Task DeleteIntermediariosCotizacionAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosIntermediarioTableWriter :: DeleteIntermediariosCotizacionAsync", ex);
                }
            }
        }
    }
}
