using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosBloqueoTableReader : IDatosBloqueoReader

    {
        public async Task<IEnumerable<Bloqueo>> GetBloqueosAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Bloqueo"
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var bloqueos = new List<Bloqueo>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var bloqueo = new Bloqueo
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario")
                        };

                        bloqueos.Add(bloqueo);
                    }

                    return bloqueos;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCausalTableReader :: LeerBloqueosAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<Bloqueo>> GetBloqueosByCodigoCotizacionAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Bloqueo"
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.VarChar).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var bloqueos = new List<Bloqueo>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var bloqueo = new Bloqueo
                        {
                            CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion"),
                            CodigoUsuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario")
                        };

                        bloqueos.Add(bloqueo);
                    }

                    return bloqueos;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosCausalTableReader :: LeerBloqueoAsync (Cotizacion)", ex);
                }
            }
        }

    }
}
