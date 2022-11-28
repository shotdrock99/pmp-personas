using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAseguradoTableReader : IDatosAseguradoReader
    {

        public async Task<IEnumerable<Asegurado>> LeerAseguradosAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var asegurados = new List<Asegurado>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        var asegurado = new Asegurado
                        {
                            CodigoTipoDocumento = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_tipo_documento"),
                            NumeroDocumento = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_documento"),
                            PrimerNombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_primer_nombre_asegurado"),
                            SegundoNombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_segundo_nombre_asegurado"),
                            PrimerApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido1_asegurado"),
                            SegundoApellido = SqlReaderUtilities.SafeGet<string>(reader, "VC_apellido2_asegurado"),
                            FechaNacimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DA_fecha_nacimiento"),
                            //asegurado.CodigoGenero = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_genero");
                            NumeroSueldosAsegurado = SqlReaderUtilities.SafeGet<int>(reader, "IN_numero_sueldos"),
                            SaldoDeudaAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_saldo_deuda"),
                            ValorAhorroAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_ahorro"),
                            ValorAportesAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_aportes"),
                            ValorSueldoAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_sueldos"),
                            ValorAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_asegurado"),
                            ValorPrimaAsegurado = SqlReaderUtilities.SafeGet<decimal>(reader, "MO_valor_prima"),
                            VetadoSarlaft = SqlReaderUtilities.SafeGet<int>(reader, "IN_vetado_sarlaft").Equals(1) ? true : false,
                            CentroCosto = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_centro_costo")
                        };

                        asegurados.Add(asegurado);
                    }

                    return asegurados;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTeableReader :: LeerAseguradoAsync", ex);
                }
            }
        }
    }
}
