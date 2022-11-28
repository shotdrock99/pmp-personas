using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosEdadesTableReader : IDatosEdadesReader
    {
        public async Task<EdadAmparoGrupoAsegurado> LeerEdadesAsync(int codigoGrupoAsegurado, int codigoAmparo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EdadPermanencia"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = codigoAmparo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var edades = new EdadAmparoGrupoAsegurado();
                try
                {
                    var edadesReader = await cmd.ExecuteReaderAsync();
                    while (await edadesReader.ReadAsync())
                    {
                        edades.CodigoEdadPermanencia = (int)edadesReader["IN_cod_edad_permanencia"];
                        edades.CodigoAmparo = (int)edadesReader["IN_cod_amparo"];
                        edades.EdadMinAsegurado = (int)edadesReader["IN_edad_min_ingreso"];
                        edades.EdadMaxAsegurado = (int)edadesReader["IN_edad_max_ingreso"];
                        edades.edadMaxPermanencia = (int)edadesReader["IN_edad_max_permanecia"];
                        edades.DiasCarencia = (int)edadesReader["IN_dias_carencia"];
                    }
                    return edades;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: LeerEdadesAsync", ex);
                }
            }
        }
    }
}
