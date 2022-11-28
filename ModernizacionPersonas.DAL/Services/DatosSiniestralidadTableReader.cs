using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSiniestralidadTableReader : IDatosSiniestralidadReader
    {
        public async Task<IEnumerable<Siniestralidad>> GetSiniestralidadAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Siniestralidad"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var siniestros = new List<Siniestralidad>();

                try
                {
                    var sinietrosReader = await cmd.ExecuteReaderAsync();
                    while (await sinietrosReader.ReadAsync())
                    {
                        var siniestralidad = new Siniestralidad
                        {
                            CodigoSiniestralidad = (int)sinietrosReader["IN_cod_siniestralidad"],
                            CodigoCotizacion = (int)sinietrosReader["IN_cod_cotizacion"],
                            Anno = (int)sinietrosReader["IN_anho"],
                            FechaInicial = (DateTime)sinietrosReader["DA_fecha_inicio"],
                            FechaFinal = (DateTime)sinietrosReader["DA_fecha_fin"],
                            ValorIncurrido = (decimal)sinietrosReader["MO_valor_incurrido"],
                            NumeroCasos = (int)sinietrosReader["IN_numero_casos"]
                        };

                        siniestros.Add(siniestralidad);
                    }

                    return siniestros;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSiniestralidadTableReader :: LeerSiniestralidadAsync", ex);
                }
            }
        }
    }
}
