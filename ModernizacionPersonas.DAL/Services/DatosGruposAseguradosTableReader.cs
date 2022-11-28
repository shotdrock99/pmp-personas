using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosGruposAseguradosTableReader : IDatosGruposAseguradoReader
    {
        private const string SP_NAME = "PMP.USP_TB_GrupoAsegurado";

        public async Task<IEnumerable<GrupoAsegurado>> GetGruposAseguradosAsync(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                try
                {
                    var gruposAsegurados = new List<GrupoAsegurado>();
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var grupoAsegurado = new GrupoAsegurado();
                        this.MapFromReader(reader, grupoAsegurado);

                        gruposAsegurados.Add(grupoAsegurado);
                    }

                    return gruposAsegurados;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosGruposAseguradosTableReader :: ObtenerGruposAseguradosAsync", ex);
                }
            }
        }

        public async Task<GrupoAsegurado> GetGrupoAseguradoAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var grupoAsegurado = new GrupoAsegurado();

                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        this.MapFromReader(reader, grupoAsegurado);
                    }

                    return grupoAsegurado;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosGruposAseguradosTableWriter :: ObtenerGrupoAseguradoAsync", ex);
                }
            }
        }

        private void MapFromReader(SqlDataReader reader, GrupoAsegurado grupoAsegurado)
        {
            var conListado = reader["TN_listado_asegurado"] != DBNull.Value ? reader["TN_listado_asegurado"] : false;
            var conDistribucion = SqlReaderUtilities.SafeGet<int>(reader, "IN_distribucion");
            int estructura = reader["IN_tipo_estructura"] != DBNull.Value ? SqlReaderUtilities.SafeGet<int>(reader, "IN_tipo_estructura") : 0;
            grupoAsegurado.CodigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_cotizacion");
            grupoAsegurado.CodigoGrupoAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_grupo_asegurado");
            grupoAsegurado.CodigoTipoSuma = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_suma_asegurada");
            grupoAsegurado.NombreGrupoAsegurado = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_grupo_asegurado");
            grupoAsegurado.ValorMinAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_min_asegurado");
            grupoAsegurado.ValorMaxAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_max_asegurado");
            grupoAsegurado.ValorAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_total_asegurado");
            grupoAsegurado.NumeroAsegurados = SqlReaderUtilities.SafeGet<int>(reader, "IN_numero_asegurados");
            grupoAsegurado.EdadPromedioAsegurados = SqlReaderUtilities.SafeGet<int>(reader, "IN_edad_promedio");
            grupoAsegurado.PorcentajeAsegurados = SqlReaderUtilities.SafeGet<int>(reader, "IN_porcentaje_asegurado");
            grupoAsegurado.ConDistribucionAsegurados = conDistribucion == 1 ? true : false;
            grupoAsegurado.TipoEstructura = estructura;            
            //var numSalarios = (int)grupoAseguradoReader["IN_numero_smmlv_asegurado"];
            grupoAsegurado.NumeroSalariosAsegurado = 0;
        }
    }
}
