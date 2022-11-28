using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAsegurabilidadTableReader : IDatosAsegurabilidadReader
    {        
        public async Task<List<Asegurabilidad>> LeerAsegurabilidadAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurabilidad"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;                
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var listAsegurabilidad = new List<Asegurabilidad>();

                try
                {
                    var asegurabilidadReader = await cmd.ExecuteReaderAsync();
                    while (await asegurabilidadReader.ReadAsync())
                    {
                        var asegurabilidad = new Asegurabilidad
                        {
                            CodigoAsegurabilidad = (int)asegurabilidadReader["IN_cod_asegurabilidad"],
                            EdadDesde = (int)asegurabilidadReader["IN_edad_desde"],
                            EdadHasta = (int)asegurabilidadReader["IN_edad_hasta"],
                            ValorIndividualDesde = (decimal)asegurabilidadReader["MO_valor_individual_desde"],
                            ValorIndividualHasta = (decimal)asegurabilidadReader["MO_valor_individual_hasta"],
                            Requisitos = asegurabilidadReader["VC_requisitos"].ToString()
                        };

                        listAsegurabilidad.Add(asegurabilidad);
                    }

                    return listAsegurabilidad;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSiniestralidadTableReader :: LeerSiniestralidadAsync", ex);
                }
            }
        }        
    }
}
