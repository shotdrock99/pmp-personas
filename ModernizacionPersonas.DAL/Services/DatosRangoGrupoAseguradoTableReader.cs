using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosRangoGrupoAseguradoTableReader : IDatosRangoGrupoAseguradoReader
    {

        public async Task<IEnumerable<Rango>> LeerRangoGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_RangoGrupoAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var listRango = new List<Rango>();
                try
                {
                    var rangoAseguradoReader = await cmd.ExecuteReaderAsync();

                    while (await rangoAseguradoReader.ReadAsync())
                    {
                        Rango rango = new Rango
                        {
                            CodigoRangoGrupoAsegurado = (int)rangoAseguradoReader["IN_cod_rango_grupo_asegurado"],
                            CodigoGrupoAsegurado = (int)rangoAseguradoReader["IN_cod_grupo_asegurado"],
                            EdadMaxAsegurado = (int)rangoAseguradoReader["IN_edad_maxima"],
                            EdadMinAsegurado = (int)rangoAseguradoReader["IN_edad_minima"],
                            NumeroAsegurados = (int)rangoAseguradoReader["IN_numero_asegurados"],
                            ValorAsegurado = (decimal)rangoAseguradoReader["MO_valor_asegurado"]
                        };

                        listRango.Add(rango);
                    }
                    return listRango;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosRangoGrupoAseguradoTableReader :: LeerRangoGrupoAseguradoAsync", ex);
                }
            }
        }
    }
}
