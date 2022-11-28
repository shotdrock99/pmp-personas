using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosEnvioSlipTableReader : IDatosEnvioSlipReader
    {
        public async Task<IEnumerable<EnvioSlip>> LeerEnvioSlipAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EnvioSlip"
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var envios = new List<EnvioSlip>();
                try
                {
                    var envioReader = await cmd.ExecuteReaderAsync();
                    while (await envioReader.ReadAsync())
                    {
                        var envio = new EnvioSlip();

                        envio.CodigoEnvio = SqlReaderUtilities.SafeGet<int>(envioReader, "IN_cod_envio_slip");
                        envio.CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(envioReader, "IN_cod_cotizacion");
                        envio.Destinatarios = SqlReaderUtilities.SafeGet<string>(envioReader, "VC_para");
                        envio.DestinatariosOcultos = SqlReaderUtilities.SafeGet<string>(envioReader, "VC_para_oculto");
                        envio.FechaEnvio = SqlReaderUtilities.SafeGet<DateTime>(envioReader, "DT_fecha_envio");
                        envio.Texto = SqlReaderUtilities.SafeGet<string>(envioReader, "VC_texto_envio");

                        envios.Add(envio);
                    }

                    return envios;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: LeerEdadesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<AdjuntoEnvioSlip>> LeerAdjuntoEnvioSlipAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AdjuntoEnvioSlip"
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var envios = new List<AdjuntoEnvioSlip>();
                try
                {
                    var envioReader = await cmd.ExecuteReaderAsync();
                    while (await envioReader.ReadAsync())
                    {
                        var envio = new AdjuntoEnvioSlip();

                        envio.CodigoAdjunto = SqlReaderUtilities.SafeGet<int>(envioReader, "IN_cod_adjunto_envio_slip");
                        envio.Adjunto = SqlReaderUtilities.SafeGet<Byte[]>(envioReader, "VB_file");
                        envio.FileName = SqlReaderUtilities.SafeGet<string>(envioReader, "VC_filename");

                        envios.Add(envio);
                    }

                    return envios;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: LeerEdadesAsync", ex);
                }
            }
        }

        
    }

    public class AdjuntoEnvioSlip
    {
        public int CodigoAdjunto { get; internal set; }
        public byte[] Adjunto { get; internal set; }
        public string FileName { get; set; }
    }
}
