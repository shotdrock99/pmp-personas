using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosEdadesTableWriter : IDatosEdadesWriter
    {
        public async Task<int> InsertarEdadAmparoAsync(int codigoGrupoasegurado, int codigoGrupoAmparo, EdadAmparoGrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EdadPermanencia"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoasegurado;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = codigoGrupoAmparo;
                cmd.Parameters.Add("@VAR_IN_edad_min_ingreso", SqlDbType.Int).Value = model.EdadMinAsegurado;
                cmd.Parameters.Add("@VAR_IN_edad_max_ingreso", SqlDbType.Int).Value = model.EdadMaxAsegurado;
                cmd.Parameters.Add("@VAR_IN_edad_max_permanencia", SqlDbType.Int).Value = model.edadMaxPermanencia;
                cmd.Parameters.Add("@VAR_IN_dias_carencia", SqlDbType.VarChar).Value = model.DiasCarencia;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var codigo = await cmd.ExecuteScalarAsync();
                    return int.Parse(codigo.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: CrearEdadesAsync", ex);
                }
            }
        }

        public async Task ActualizarEdadesAsync(int codigoEdades, EdadAmparoGrupoAsegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EdadPermanencia"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = model.CodigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = model.CodigoAmparo;
                cmd.Parameters.Add("@VAR_IN_edad_min_ingreso", SqlDbType.Int).Value = model.EdadMinAsegurado;
                cmd.Parameters.Add("@VAR_IN_edad_max_ingreso", SqlDbType.Int).Value = model.EdadMaxAsegurado;
                cmd.Parameters.Add("@VAR_IN_dias_carencia", SqlDbType.VarChar).Value = model.DiasCarencia;
                cmd.Parameters.Add("@VAR_IN_cod_edad_permanencia", SqlDbType.Int).Value = codigoEdades;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: ActualizarEdadesAsync", ex);
                }
            }
        }

        public async Task EliminarEdadesAsync(int codigoEdades)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_EdadPermanencia"
                };
                cmd.Parameters.Add("@VAR_IN_cod_edad_permanencia", SqlDbType.Int).Value = codigoEdades;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosEdadesTableWriter :: EliminarEdadesAsync", ex);
                }
            }
        }
    }
}
