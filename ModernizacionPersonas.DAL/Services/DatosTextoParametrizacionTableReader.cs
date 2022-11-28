using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosTextoParametrizacionTableReader : IDatosTextosParametrizacionReader
    {
        private const string SP_NAME = "PMP.USP_TB_TextosParametrizacion";
        public async Task<IEnumerable<TextoSlip>> LeerTextosParametrizacionAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var valoresTextoParamLista = new List<TextoSlip>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var texto = new TextoSlip()
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion"),
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "TX_texto_parametrizacion"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresTextoParamLista.Add(texto);
                    }

                    return valoresTextoParamLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextosParametrizacionAsync", ex);
                }
            }
        }

        public async Task<TextoSlip> LeerTextoParametrizacionAsync(int codigoTextoParametrizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionEmail"
                };
                cmd.Parameters.Add("@VAR_IN_cod_seccion_email", SqlDbType.Int).Value = codigoTextoParametrizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var result = new TextoSlip();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion");
                        result.CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip");
                        result.CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector");
                        result.NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip");
                        result.CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo");
                        result.CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo");
                        result.CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo");
                        result.Texto = SqlReaderUtilities.SafeGet<string>(reader, "VC_texto_email");
                        result.Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario");
                        result.Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc");
                        result.FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento");
                    };

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextoParametrizacionAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionAmparoAsync(int codigoAmparo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = codigoAmparo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                cmd.Connection = conn;
                var valoresTextoParamLista = new List<TextoSlip>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var texto = new TextoSlip()
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion"),
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "TX_texto_parametrizacion"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresTextoParamLista.Add(texto);
                    }

                    return valoresTextoParamLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextoParametrizacionAmparoAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionRamoAsync(int codigoRamo, int codigoSubramo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = codigoSubramo;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 8;
                cmd.Connection = conn;
                var valoresTextoParamLista = new List<TextoSlip>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var texto = new TextoSlip()
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion"),
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "TX_texto_parametrizacion"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresTextoParamLista.Add(texto);
                    }

                    return valoresTextoParamLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextoParametrizacionRamoAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionSeccionAsync(int codigoSeccion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = codigoSeccion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                var valoresTextoParamLista = new List<TextoSlip>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var texto = new TextoSlip()
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion"),
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "TX_texto_parametrizacion"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresTextoParamLista.Add(texto);
                    }

                    return valoresTextoParamLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextoParametrizacionSeccionAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<TextoSlip>> LeerTextoParametrizacionSectorAsync(int codigoSector)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = codigoSector;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                var valoresTextoParamLista = new List<TextoSlip>();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var texto = new TextoSlip()
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_texto_parametrizacion"),
                            CodigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            CodigoSector = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_sector"),
                            NombreSeccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_email"),
                            CodigoRamo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_ramo"),
                            CodigoSubramo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_subramo"),
                            CodigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo"),
                            Texto = SqlReaderUtilities.SafeGet<string>(reader, "TX_texto_parametrizacion"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };
                        valoresTextoParamLista.Add(texto);
                    }

                    return valoresTextoParamLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosTextoParametrizacionTableReader :: LeerTextoParametrizacionSeccionAsync", ex);
                }
            }
        }

    }
}
