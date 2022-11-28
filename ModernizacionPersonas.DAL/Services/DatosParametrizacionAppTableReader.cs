using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosParametrizacionAppTableReader : IDatosParametrizacionAppReader

    {
        public async Task<IEnumerable<ParametrizacionApp>> GetValoresVariablesApppAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionApp"
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var variables = new List<ParametrizacionApp>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var item = new ParametrizacionApp
                        {
                            CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_variable_app"),
                            NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable"),
                            ValorVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_valor_variable"),
                            TipoValorVariable= SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_valor_variable")
                        };

                        variables.Add(item);
                    }

                    return variables;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionAppTableReader :: GetValoresVariablesApppAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<ParametrizacionApp>> GetValorVariableAsync(int codigoParametriacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_ParametrizacionApp"
                };

                cmd.Parameters.Add("@VAR_IN_cod_variable_app", SqlDbType.VarChar).Value = codigoParametriacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var variables = new List<ParametrizacionApp>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var item = new ParametrizacionApp
                        {
                            CodigoVariable = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_variable_app"),
                            NombreVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_variable"),
                            ValorVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_valor_variable"),
                            TipoValorVariable = SqlReaderUtilities.SafeGet<string>(reader, "VC_tipo_valor_variable")
                        };

                        variables.Add(item);
                    }

                    return variables;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosParametrizacionAppTableReader :: GetValorVariableAsync (codigoParametriacion)", ex);
                }
            }
        }

    }
}
