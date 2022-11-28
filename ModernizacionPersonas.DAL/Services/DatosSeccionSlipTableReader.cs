using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSeccionSlipTableReader : IDatosSeccionSlipReader
    {
        private const string SP_NAME = "PMP.USP_TB_SeccionesSlip";

        public async Task<IEnumerable<SeccionSlip>> GetSeccionesAsync()
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
                var result = new List<SeccionSlip>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var item = new SeccionSlip
                            {
                                Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                                Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip"),
                                Grupo = SqlReaderUtilities.SafeGet<int>(reader, "IN_seccion_grupo"),
                                Especial = SqlReaderUtilities.SafeGet<int>(reader, "IN_especial"),
                                Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                                Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                                Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                                FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                            };

                            result.Add(item);
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosSeccionSlipTableReader :: GetSeccionesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<SeccionSlip>> GetSeccionAsync(int codigoSeccion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = codigoSeccion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var result = new List<SeccionSlip>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new SeccionSlip
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip"),
                            Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip"),
                            Grupo = SqlReaderUtilities.SafeGet<int>(reader, "IN_seccion_grupo"),
                            Especial = SqlReaderUtilities.SafeGet<int>(reader, "IN_especial"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo"),
                            Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                            Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                            FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSeccionSlipTableReader :: GetSeccionAsync", ex);
                }
            }
        }
    }
}
