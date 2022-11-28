using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTomadorTableReader : IDatosTomadorReader
    {
        public async Task<Tomador> GetTomadorAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Tomador"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var tomador = new Tomador();
                    while (await reader.ReadAsync())
                    {
                        tomador.CodigoCotizacion = codigoCotizacion;
                        tomador.CodigoTomador = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tomador");
                        tomador.CodigoTipoDocumento = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_documento");
                        tomador.AseguradoraActual = SqlReaderUtilities.SafeGet<string>(reader, "VC_aseguradora_actual");
                        tomador.NumeroDocumento = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_documento");
                        tomador.Nombres = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombres_tomador");
                        tomador.PrimerApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido1_tomador");
                        tomador.SegundoApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido2_tomador");
                        tomador.Direccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_direccion_tomador");
                        tomador.CodigoPais = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_pais_tomador");
                        tomador.CodigoDepartamento = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_departamento_tomador");
                        tomador.CodigoMunicipio = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_municipio_tomador");
                        tomador.Email = SqlReaderUtilities.SafeGet<string>(reader, "VC_email_tomador");
                        tomador.NombreContacto = SqlReaderUtilities.SafeGet<string>(reader, "VC_contacto_tomador");
                        tomador.Telefono1Contacto = SqlReaderUtilities.SafeGet<string>(reader, "VC_telefono1_contacto_tomador");
                        tomador.Telefono2Contacto = SqlReaderUtilities.SafeGet<string>(reader, "VC_telefono2_contacto_tomador");
                        tomador.CodigoActividad = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_actividad_economica");
                        tomador.Licitacion = SqlReaderUtilities.SafeGet<bool>(reader, "BT_licitacion");
                        tomador.DescripcionTipoRiesgo = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_tipo_riesgo");
                        tomador.TomadorSlip = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_tomador_slip");
                        tomador.Actividad = SqlReaderUtilities.SafeGet<string>(reader, "VC_actividad_economica");
                    }

                    return tomador;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTomadorTableReader :: LeerTomadorAsync", ex);
                }
            }
        }
    }
}
