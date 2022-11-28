using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Entities.SISEEntities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.SISEServices
{
    public class SISEAseguradosSummaryProcessor
    {
        const string SP_NAME = "usp_pmp_listado_asegurados";

        public async Task<GetTasasAmparosResponse> GetTasasAmparosAsync(SISEAseguradosProcessorArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_grupo_suma_aseg", SqlDbType.Int).Value = args.CodigoGrupoAsegurados;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@VAR_IN_id_proceso", SqlDbType.Int).Value = args.CodigoProceso;
                cmd.Parameters.Add("@VAR_IN_cod_perfil1", SqlDbType.Int).Value = args.CodigoPerfilEdades;
                cmd.Parameters.Add("@VAR_IN_cod_perfil2", SqlDbType.Int).Value = args.CodigoPerfilValores;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = args.CodigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = args.CodigoSubRamo;
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", args.CodigoSector);
                cmd.Parameters.Add("@VAR_IN_cnt_aseg", SqlDbType.Int).Value = args.CntAsegurados;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_tasa", SqlDbType.Int).Value = args.CodigoTipoTasa;
                cmd.Parameters.Add("@VAR_IN_cod_cotiza", SqlDbType.Int).Value = args.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_SN_listado", SqlDbType.Int).Value = args.Listado == true ? 1 : 0; //TODO: Sin listado
                cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;

                var tasasAmparos = new List<TasaAmparo>();
                var tasasAsegurados = new List<TasaAsegurado>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        tasasAsegurados = await this.GetTasasAseguradosAsync(reader);

                        reader.NextResult();
                        tasasAmparos = await this.GetTasasAmparosAsync(reader);

                        return new GetTasasAmparosResponse
                        {
                            TasasAsegurados = tasasAsegurados,
                            TasasAmparos = tasasAmparos
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosSummaryProcessor :: ProcessAsync", ex);
                }
            }
        }

        private async Task<List<TasaAmparo>> GetTasasAmparosAsync(SqlDataReader reader)
        {
            var result = new List<TasaAmparo>();
            while (await reader.ReadAsync())
            {
                var codigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "cod_amparo");
                var tasa = SqlReaderUtilities.SafeGet<decimal>(reader, "tasa_riesgo");
                result.Add(new TasaAmparo { CodigoAmparo = codigoAmparo, Tasa = tasa });
            }

            return result;
        }

        private async Task<List<TasaAsegurado>> GetTasasAseguradosAsync(SqlDataReader reader)
        {
            var result = new List<TasaAsegurado>();
            while (await reader.ReadAsync())
            {
                var codigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "cod_amparo");
                var tasa = SqlReaderUtilities.SafeGet<decimal>(reader, "tasa_riesgo");
                var valorAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "nm_vr_aseg");
                var numeroDocumento = SqlReaderUtilities.SafeGet<string>(reader, "vc_nro_doc");

                result.Add(new TasaAsegurado
                {
                    NumeroDocumento = numeroDocumento,
                    ValorAsegurado = valorAsegurado,
                    CodigoAmparo = codigoAmparo,
                    Tasa = tasa
                });
            }

            return result;
        }

        public async Task<IEnumerable<TasaRangoEdad>> CalcularTasasPorRangosAsync(SISECalcularTasaRangoArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_id_proceso", SqlDbType.Int).Value = args.CodigoProceso;
                cmd.Parameters.Add("@VAR_IN_cod_cotiza", SqlDbType.Int).Value = args.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@VAR_IN_cod_grupo_suma_aseg", SqlDbType.Int).Value = args.CodigoGrupoAsegurados;
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil1", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil2", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_ramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_subramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", args.CodigoSector);
                cmd.Parameters.AddWithValue("@VAR_IN_cnt_aseg", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_tipo_tasa", 0);
                cmd.Parameters.AddWithValue("@VAR_SN_listado", 0);
                cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 4;

                cmd.Connection = conn;

                var tasas = new List<TasaRangoEdad>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var indiceRango = SqlReaderUtilities.SafeGet<int>(reader, "in_id_rango");
                        var edadMinima = SqlReaderUtilities.SafeGet<int>(reader, "in_edad_min");
                        var edadMaxima = SqlReaderUtilities.SafeGet<int>(reader, "in_edad_max");
                        var conteoAsegurados = SqlReaderUtilities.SafeGet<int>(reader, "in_cnt_aseg");
                        var valorAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "nm_vr_aseg");
                        var edadPromedio = SqlReaderUtilities.SafeGet<int>(reader, "edad_promedio");
                        var tasa = SqlReaderUtilities.SafeGet<decimal>(reader, "tasa_");

                        tasas.Add(new TasaRangoEdad { IndiceRango = indiceRango, Tasa = tasa });
                    }

                    return tasas;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosSummaryProcessor :: CalcularTasasPorRangosAsync", ex);
                }
            }
        }

        public async Task<PerfilEdades> ObtenerRangosxEdadAsync(SISEObtenerRangoPerfilArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_id_proceso", SqlDbType.Int).Value = args.CodigoProceso;
                cmd.Parameters.Add("@VAR_IN_cod_cotiza", SqlDbType.Int).Value = args.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@VAR_IN_cod_perfil1", SqlDbType.Int).Value = args.CodigoPerfil;
                cmd.Parameters.Add("@VAR_IN_cod_perfil2", SqlDbType.Int).Value = 0;
                cmd.Parameters.AddWithValue("@VAR_IN_cod_grupo_suma_aseg", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_ramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_subramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cnt_aseg", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_tipo_tasa", 0);
                cmd.Parameters.AddWithValue("@VAR_SN_listado", 0);
                cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 5;

                cmd.Connection = conn;

                var data = new PerfilEdades();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var indiceRango = (int)reader["in_id_rango"];
                        var edadMin = (int)reader["in_edad_min"];
                        var edadMax = (int)reader["in_edad_max"];
                        var cantidadAsegurados = (int)reader["in_cnt_aseg"];
                        var pjePart1 = (decimal)reader["pje_part1"];
                        var pjePart2 = (decimal)reader["pje_part2"];
                        var valorAsegurado = (decimal)reader["nm_vr_aseg"];
                        var promedio = (decimal)reader["promedio"];
                        var edadMaxAseg = (int)reader["in_edad_max_aseg"]; // Revisar valor retornado, es repetido

                        data.Rangos.Add(new PerfilEdadesRango
                        {
                            AseguradoMayorEdad = edadMaxAseg,
                            CantidadAsegurados = cantidadAsegurados,
                            EdadDesde = edadMin,
                            EdadHasta = edadMax,
                            Indice = indiceRango,
                            PorcentajeParticipacionAsegurados = 0,
                            PorcentajeParticipacionValorAsegurado = 0,
                            PromedioValorAsegurado = promedio,
                            ValorAseguradoTotal = valorAsegurado
                        });
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosSummaryProcessor :: ObtenerRangosxEdadAsync", ex);
                }
            }
        }

        public async Task<PerfilValores> ObtenerRangosxValorAsync(SISEObtenerRangoPerfilArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_id_proceso", SqlDbType.Int).Value = args.CodigoProceso;
                cmd.Parameters.Add("@VAR_IN_cod_cotiza", SqlDbType.Int).Value = args.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_version", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@VAR_IN_cod_perfil1", SqlDbType.Int).Value = args.CodigoPerfil;
                cmd.Parameters.Add("@VAR_IN_cod_perfil2", SqlDbType.Int).Value = 0;
                cmd.Parameters.AddWithValue("@VAR_IN_cod_grupo_suma_aseg", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_ramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_subramo", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cnt_aseg", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_tipo_tasa", 0);
                cmd.Parameters.AddWithValue("@VAR_SN_listado", 0);
                cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 6;

                cmd.Connection = conn;

                var data = new PerfilValores();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var indiceRango = (int)reader["rango"];
                        var valorDesde = (decimal)reader["imp_suma_desde"];
                        var valorHasta = (decimal)reader["imp_suma_hasta"];
                        var cantidadAsegurados = (int)reader["cnt_aseg"];
                        var pjePart1 = (decimal)reader["pje_part1"];
                        var pjePart2 = (decimal)reader["pje_part2"];
                        var valorAseguradoTotal = (decimal)reader["vr_aseg_total"];
                        var promedio = (decimal)reader["vr_prom_total"];

                        data.Rangos.Add(new PerfilValoresRango
                        {
                            CantidadAsegurados = cantidadAsegurados,
                            Indice = indiceRango,
                            PorcentajeParticipacionAsegurados = pjePart1,
                            PorcentajeParticipacionValorAsegurado = pjePart2,
                            PromedioValorAsegurado = promedio,
                            ValorAseguradoDesde = valorDesde,
                            ValorAseguradoHasta = valorHasta,
                            ValorAseguradoTotal = valorAseguradoTotal
                        });
                    }

                    return data;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosSummaryProcessor :: ObtenerRangosxValorAsync", ex);
                }
            }
        }
    }

    public class TasaRangoEdad
    {
        public int IndiceRango { get; set; }
        public decimal Tasa { get; set; }
    }

    public class GetTasasAmparosResponse
    {
        public List<TasaAsegurado> TasasAsegurados { get; set; }
        public List<TasaAmparo> TasasAmparos { get; set; }
    }

    public class SISECalcularTasaRangoArgs
    {
        public int CodigoProceso { get; set; }
        public int CodigoCotizacion { get; set; }
        public int CodigoGrupoAsegurados { get; set; }
        public int CodigoSector { get; set; }
    }

    public class SISEObtenerRangoPerfilArgs
    {
        public int CodigoProceso { get; set; }
        public int CodigoCotizacion { get; set; }
        public object CodigoPerfil { get; set; }
    }

    public class SISEAseguradosProcessorArgs
    {
        public int CodigoProceso { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubRamo { get; set; }
        public int CodigoCotizacion { get; set; }
        public int CodigoGrupoAsegurados { get; set; }
        public int CodigoPerfilEdades { get; set; }
        public int CodigoPerfilValores { get; set; }
        public int CodigoTipoTasa { get; set; }
        public int CntAsegurados { get; set; }
        public bool Listado { get; set; }
        public int CodigoSector { get; set; }
    }
}
