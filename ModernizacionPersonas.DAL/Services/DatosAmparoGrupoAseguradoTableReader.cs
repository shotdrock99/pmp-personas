using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAmparoGrupoAseguradoTableReader : IDatosAmparoGrupoAseguradoReader
    {
        public async Task<IEnumerable<AmparoGrupoAsegurado>> LeerAmparoGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AmparoGrupoAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var amparos = new List<AmparoGrupoAsegurado>();
                try
                {
                    var amparoAseguradoReader = await cmd.ExecuteReaderAsync();

                    while (await amparoAseguradoReader.ReadAsync())
                    {
                        AmparoGrupoAsegurado amparo = new AmparoGrupoAsegurado
                        {
                            CodigoAmparoGrupoAsegurado = (int)amparoAseguradoReader["IN_cod_amparo_grupo_asegurado"],
                            CodigoAmparo = (int)amparoAseguradoReader["IN_cod_amparo"],
                            CodigoGrupoAsegurado = (int)amparoAseguradoReader["IN_cod_grupo_asegurado"]
                        };

                        amparos.Add(amparo);
                    }

                    return amparos;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosAmparoGrupoAseguradoTableReader :: LeerAmparoGrupoAseguradoAsync", ex);
                }
            }
        }
    }
}
