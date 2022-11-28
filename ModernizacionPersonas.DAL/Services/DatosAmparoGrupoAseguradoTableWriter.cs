using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAmparoGrupoAseguradoTableWriter : IDatosAmparoGrupoAseguradoWriter
    {
        public async Task<int> CrearAmparoGrupoAseguradoAsync(AmparoGrupoAsegurado model)
        {
            //throw new NotImplementedException();

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AmparoGrupoAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var res = await cmd.ExecuteScalarAsync();
                    var codigoAmparoGrupoAsegurado = int.Parse(res.ToString());
                    return codigoAmparoGrupoAsegurado;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAmparoGrupoAseguradoTableWriter :: CrearAmparoGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task ActualizarAmparoGrupoAseguradoAsync(int codigoAmparoGrupoAsegurado, AmparoGrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AmparoGrupoAsegurado"
                };

                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = codigoAmparoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAmparoGrupoAseguradoTableWriter :: ActualizarAmparoGrupoAseguradoAsync", ex);
                }
            }
        }

        public async Task EliminarAmparoGrupoAseguradoAsync(int codigoAmparoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_AmparoGrupoAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_amparo_grupo_asegurado", SqlDbType.Int).Value = codigoAmparoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAmparoGrupoAseguradoTableWriter :: EliminarAmparoGrupoAseguradoAsync", ex);
                }
            }
        }
    }
}
