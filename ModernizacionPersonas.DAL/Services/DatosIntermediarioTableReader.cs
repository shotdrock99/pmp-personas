using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosIntermediarioTableReader : IDatosIntermediarioReader
    {
        public async Task<IEnumerable<Intermediario>> GetIntermediariosAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Intermediario"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var intermediarios = new List<Intermediario>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var intermediario = new Intermediario
                        {
                            CodigoTipoDocumento = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_documento"),
                            NumeroDocumento = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_documento"),
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_intermediario"),
                            Clave = SqlReaderUtilities.SafeGet<int>(reader, "IN_clave_intermediario"),
                            PrimerNombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_intermediario"),
                            SegundoNombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre2_intermediario"),
                            PrimerApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido_intermediario"),
                            SegundoApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido2_intermediario"),
                            CodigoEstado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_intermediario"),
                            Participacion = (decimal)reader["NM_participacion_intermediario"],
                            Direccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_direccion_intermediario"),
                            Telefono = SqlReaderUtilities.SafeGet<string>(reader, "VC_telefono_intermediario"),
                            Email = SqlReaderUtilities.SafeGet<string>(reader, "VC_email_intermediario"),
                            CodigoDepartamento = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_departamento_intermediario"),
                            CodigoMunicipio = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_municipio_intermediario"),
                            IntermediarioSlip = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_intermediario_slip")
                        };

                        intermediarios.Add(intermediario);
                    }

                    return intermediarios;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosIntermediarioTableWriter :: LeerIntermediarioAsync", ex);
                }
            }
        }
    }
}
