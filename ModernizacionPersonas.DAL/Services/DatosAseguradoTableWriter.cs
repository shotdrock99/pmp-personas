using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z.BulkOperations;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosAseguradoTableWriter : IDatosAseguradoWriter
    {
        private int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }

        private async Task<InsertarBloqueAseguradosResponse> ContarAseguradosAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 6;
                cmd.Connection = conn;
                var procesados = 0L;
                var edadPromedio = 0;
                try
                {
                    var readerConteo = await cmd.ExecuteReaderAsync();
                    while (await readerConteo.ReadAsync())
                    {
                        procesados = int.Parse(readerConteo[0].ToString());
                        edadPromedio = (int)readerConteo[1];
                    }

                    return new InsertarBloqueAseguradosResponse
                    {
                        // la ejecucion del SP debe retornar los registros procesados
                        RegistrosProcesados = procesados,
                        EdadPromedio = edadPromedio
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: ContarAseguradosAsync", ex);
                }

            }
        }

        public async Task<InsertarBloqueAseguradosResponse> ValidarAseguradosAsync(int codigoGrupoAsegurado, string numeroDocumento)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = numeroDocumento;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                cmd.Connection = conn;
                var procesados = 0L;
                var edadPromedio = 0;
                try
                {
                    var readerConteo = await cmd.ExecuteReaderAsync();
                    while (await readerConteo.ReadAsync())
                    {
                        procesados = int.Parse(readerConteo[0].ToString());
                        edadPromedio = (int)readerConteo[1];
                    }

                    return new InsertarBloqueAseguradosResponse
                    {
                        // la ejecucion del SP debe retornar los registros procesados
                        RegistrosProcesados = procesados,
                        EdadPromedio = edadPromedio
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: ValidarAseguradosAsync", ex);
                }

            }
        }

        private async Task InsertarNumAseguradosAsync(int codigoGrupoAsegurado, int numeroAsegurados, int edadPromedio)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_GrupoAsegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_IN_numero_asegurados", SqlDbType.Int).Value = numeroAsegurados;
                cmd.Parameters.Add("@VAR_IN_edad_promedio", SqlDbType.Int).Value = edadPromedio;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    var codigoAsegurado = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradosTableWriter :: InsertarNumAseguradosAsync", ex);
                }
            }
        }

        public async Task InsertarAseguradoAsync(Asegurado asegurado, int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };

                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.VarChar).Value = asegurado.NumeroDocumento;
                cmd.Parameters.Add("@VAR_DA_fecha_nacimiento", SqlDbType.DateTime).Value = asegurado.FechaNacimiento;
                cmd.Parameters.Add("@VAR_VC_primer_nombre_asegurado", SqlDbType.VarChar).Value = asegurado.PrimerNombre;
                cmd.Parameters.Add("@VAR_VC_segundo_nombre_asegurado", SqlDbType.VarChar).Value = asegurado.SegundoNombre;
                cmd.Parameters.Add("@VAR_VC_apellido1_asegurado", SqlDbType.VarChar).Value = asegurado.PrimerApellido;
                cmd.Parameters.Add("@VAR_VC_apellido2_asegurado", SqlDbType.VarChar).Value = asegurado.SegundoApellido;
                cmd.Parameters.Add("@VAR_MO_valor_asegurado", SqlDbType.Money).Value = asegurado.ValorAsegurado;
                cmd.Parameters.Add("@VAR_IN_vetado_sarlaf", SqlDbType.Int).Value = asegurado.VetadoSarlaft.Equals(true) ? 1: 0;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;

                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: InsertarAseguradoAsync", ex);
                }
            }
        }

        public async Task<InsertarBloqueAseguradosResponse> InsertarBloqueAseguradosAsync(IEnumerable<Asegurado> model, int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var aseguradosCnt = 0L;
                var aseguradosInsertCnt = 0L;
                var insertCount = 0L;
                var procesados = 0L;
                var duplicados = 0L;
                var edad = 0;
                var responseCont = await ContarAseguradosAsync(codigoGrupoAsegurado);
                aseguradosCnt = responseCont.RegistrosProcesados;
                var tableData = new DataTable();

                var unicos = model.GroupBy(x => x.NumeroDocumento).Select(y => y.FirstOrDefault());
                tableData = unicos.AsDataTable();
                insertCount = tableData.Rows.Count;

                try
                {
                    foreach (Asegurado asegurado in unicos)
                    {
                        var validaDuplicado = await ValidarAseguradosAsync(codigoGrupoAsegurado, asegurado.NumeroDocumento);
                        if (validaDuplicado.RegistrosProcesados < 1)
                        {
                            await InsertarAseguradoAsync(asegurado, codigoGrupoAsegurado);
                        }
                        else
                        {
                            await ActualizarAseguradoAsync(codigoGrupoAsegurado, asegurado);
                        }
                    }

                    var responseInsertCont = await ContarAseguradosAsync(codigoGrupoAsegurado);
                    aseguradosInsertCnt = responseInsertCont.RegistrosProcesados;
                    edad = responseInsertCont.EdadPromedio;

                    await InsertarNumAseguradosAsync(codigoGrupoAsegurado, unchecked((int)aseguradosInsertCnt), unchecked((int)edad));

                    procesados = (aseguradosInsertCnt - aseguradosCnt);
                    procesados = procesados < 0 ? procesados * -1 : procesados;

                    duplicados = insertCount - procesados;
                    duplicados = duplicados < 0 ? duplicados * -1 : duplicados;

                    return new InsertarBloqueAseguradosResponse
                    {
                        TotalAsegurados = aseguradosInsertCnt,
                        RegistrosProcesados = procesados,
                        RegistrosDuplicados = duplicados,
                        EdadPromedio = edad
                    };
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: InsertarBloqueAseguradosAsync", ex);
                }
            }
        }



        public async Task ActualizarAseguradoAsync(int codigoGrupoAsegurado, Asegurado model)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@VAR_VC_numero_documento", SqlDbType.Int).Value = model.NumeroDocumento;
                cmd.Parameters.Add("@VAR_DA_fecha_nacimiento", SqlDbType.DateTime).Value = model.FechaNacimiento;
                cmd.Parameters.Add("@VAR_IN_numero_sueldos", SqlDbType.Int).Value = model.NumeroSueldosAsegurado;
                cmd.Parameters.Add("@VAR_MO_saldo_deuda", SqlDbType.Money).Value = model.SaldoDeudaAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_ahorro", SqlDbType.Money).Value = model.ValorAhorroAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_aportes", SqlDbType.Money).Value = model.ValorAportesAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_sueldos", SqlDbType.Money).Value = model.ValorSueldoAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_asegurado", SqlDbType.Money).Value = model.ValorAsegurado;
                cmd.Parameters.Add("@VAR_MO_valor_prima", SqlDbType.Money).Value = model.ValorPrimaAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: ActualizarAseguradoAsync", ex);
                }
            }
        }

        public async Task EliminarAseguradosAsync(int codigoGrupoAsegurado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Asegurado"
                };
                cmd.Parameters.Add("@VAR_IN_cod_grupo_asegurado", SqlDbType.Int).Value = codigoGrupoAsegurado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 5;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosAseguradoTableWriter :: EliminarAseguradosAsync", ex);
                }
            }
        }
    }
}
