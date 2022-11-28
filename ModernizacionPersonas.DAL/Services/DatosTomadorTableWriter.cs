using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTomadorTableWriter : IDatosTomadorWriter
    {
        const string SP_NAME = "PMP.USP_TB_Tomador";

        public async Task<int> CrearTomadorAsync(int codigoCotizacion, Tomador model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.Int).Value = model.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = model.NumeroDocumento;
                cmd.Parameters.Add("@VAR_VC_nombres_tomador", SqlDbType.VarChar).Value = model.Nombres.Replace("null","").Replace("undefined", "");
                cmd.Parameters.Add("@VAR_VC_apellido1_tomador", SqlDbType.VarChar).Value = model.PrimerApellido;
                cmd.Parameters.Add("@VAR_VC_apellido2_tomador", SqlDbType.VarChar).Value = model.SegundoApellido;
                cmd.Parameters.Add("@VAR_VC_direccion_tomador", SqlDbType.VarChar).Value = model.Direccion;
                cmd.Parameters.Add("@VAR_IN_cod_pais_tomador", SqlDbType.Int).Value = model.CodigoPais;
                cmd.Parameters.Add("@VAR_IN_cod_departamento_tomador", SqlDbType.Int).Value = model.CodigoDepartamento;
                cmd.Parameters.Add("@VAR_IN_cod_municipio_tomador", SqlDbType.Int).Value = model.CodigoMunicipio;
                cmd.Parameters.Add("@VAR_VC_email_tomador", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_VC_contacto_tomador", SqlDbType.VarChar).Value = model.NombreContacto;
                cmd.Parameters.Add("@VAR_VC_telefono1_contacto_tomador", SqlDbType.VarChar).Value = model.Telefono1Contacto;
                cmd.Parameters.Add("@VAR_VC_telefono2_contacto_tomador", SqlDbType.VarChar).Value = model.Telefono2Contacto;
                cmd.Parameters.Add("@VAR_IN_cod_actividad_economica", SqlDbType.Int).Value = model.CodigoActividad;
                cmd.Parameters.Add("@VAR_BT_licitacion", SqlDbType.Bit).Value = model.Licitacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    await DeleteTomadorAsync(codigoCotizacion);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var codigoTomador = 0;
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            codigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador");
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return codigoTomador;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTomadorTableWriter :: CrearTomadorAsync", ex);
                }
            }
        }

        public async Task ActualizarTomadorAsync(int codigoCotizacion, Tomador model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_tomador", SqlDbType.Int).Value = model.CodigoTomador;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.Int).Value = model.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = model.NumeroDocumento;
                cmd.Parameters.Add("@VAR_VC_nombres_tomador", SqlDbType.VarChar).Value = model.Nombres;
                cmd.Parameters.Add("@VAR_VC_apellido1_tomador", SqlDbType.VarChar).Value = model.PrimerApellido;
                cmd.Parameters.Add("@VAR_VC_apellido2_tomador", SqlDbType.VarChar).Value = model.SegundoApellido;
                cmd.Parameters.Add("@VAR_VC_direccion_tomador", SqlDbType.VarChar).Value = model.Direccion;
                cmd.Parameters.Add("@VAR_IN_cod_pais_tomador", SqlDbType.Int).Value = model.CodigoPais;
                cmd.Parameters.Add("@VAR_IN_cod_departamento_tomador", SqlDbType.Int).Value = model.CodigoDepartamento;
                cmd.Parameters.Add("@VAR_IN_cod_municipio_tomador", SqlDbType.Int).Value = model.CodigoMunicipio;
                cmd.Parameters.Add("@VAR_VC_email_tomador", SqlDbType.VarChar).Value = model.Email;
                cmd.Parameters.Add("@VAR_VC_contacto_tomador", SqlDbType.VarChar).Value = model.NombreContacto;
                cmd.Parameters.Add("@VAR_VC_telefono1_contacto_tomador", SqlDbType.VarChar).Value = model.Telefono1Contacto;
                cmd.Parameters.Add("@VAR_VC_telefono2_contacto_tomador", SqlDbType.VarChar).Value = model.Telefono2Contacto;
                cmd.Parameters.Add("@VAR_IN_cod_actividad_economica", SqlDbType.Int).Value = model.CodigoActividad;
                cmd.Parameters.Add("@VAR_BT_licitacion", SqlDbType.Bit).Value = model.Licitacion;
                cmd.Parameters.Add("@VAR_VC_nombre_tomador_slip", SqlDbType.VarChar).Value = model.TomadorSlip;
                cmd.Parameters.Add("@VAR_VC_actividad_economica", SqlDbType.VarChar).Value = model.Actividad;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        var result = new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            NumeroCotizacion = numeroCotizacion,
                            CodigoEstadoCotizacion = estadoCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosTomadorTableWriter :: ActualizarTomadorAsync", ex);
                }
            }
        }

        public async Task DeleteTomadorAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
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
                    throw new Exception("DatosTomadorTableWriter :: EliminarTomadorAsync", ex);
                }
            }
        }
    }
}
