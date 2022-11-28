using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosParametrizacionEmailTableReader : IDatosParametrizacionEmailReader
    {
        public async Task<GetEmailTextoResponse> LeerParametrizacionEmailAsync(int tomadorIntermediario, int codigoRamo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionEmail"
                };
                cmd.Parameters.Add("@VAR_IN_tom_comercial", SqlDbType.Int).Value = tomadorIntermediario;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var valoresEmailLista = new List<EmailParametrizacion>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var valoresEmail = new EmailParametrizacion()
                        {
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_email"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoTemplate = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_template"),
                            CodigoTomadorComercial = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tom_comercial"),
                            CodigoParametrizacionEmail = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_parametrizacion_email"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "VC_texto_email") ,
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresEmailLista.Add(valoresEmail);
                    }

                    return new GetEmailTextoResponse
                    {
                        EmailTextos = valoresEmailLista
                    };
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionEmailTableReader :: LeerParametrizacionEmailAsync", ex);
                }
            }
        }

        public async Task<EmailParametrizacion> LeerParametrizacionEmailCodigoAsync(int codigoSeccion,int codigoTemplate)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionEmail"
                };
                cmd.Parameters.Add("@VAR_IN_cod_seccion_email", SqlDbType.Int).Value = codigoSeccion;
                cmd.Parameters.Add("@VAR_IN_cod_template", SqlDbType.Int).Value = codigoTemplate;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var valoresEmail = new EmailParametrizacion();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        valoresEmail.CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_email");
                        valoresEmail.NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email");
                        valoresEmail.CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo");
                        valoresEmail.CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo");
                        valoresEmail.CodigoTemplate = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_template");
                        valoresEmail.CodigoTomadorComercial = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tom_comercial");
                        valoresEmail.CodigoParametrizacionEmail = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_parametrizacion_email");
                        valoresEmail.Texto = SqlReaderUtilities.SafeGet<string>(reader, "VC_texto_email");
                        valoresEmail.Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario");
                        valoresEmail.Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc");
                        valoresEmail.FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento");
                    };

                    return valoresEmail;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionEmailTableReader :: LeerParametrizacionEmailCodigoAsync", ex);
                }
            }
        }
    }
}
