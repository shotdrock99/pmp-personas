using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosSlipTableReader : IDatosSlipReader
    {
        public async Task<IEnumerable<VariableValorSlip>> LeerValoresSlipAsync(int codigoCotizacion, int codigoRamo, int codigoSector, int codigoSubramo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = codigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = codigoSector;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                var valoresSlipLista = new List<VariableValorSlip>();

                try
                {
                    var valoresReader = await cmd.ExecuteReaderAsync();
                    while (await valoresReader.ReadAsync())
                    {
                        var valores = new VariableValorSlip
                        {
                            CodigoSeccion = (int)valoresReader["IN_cod_seccion_slip"],
                            NombreSeccion = (string)valoresReader["VC_seccion_slip"],
                            CodigoAmparo = (int)valoresReader["cod_amparo"],
                            CodigoRamo = (int)valoresReader["cod_ramo"],
                            CodigoSubRamo = (int)valoresReader["cod_subramo"],
                            CodigoTipoSeccion = (int)valoresReader["TipoGrupo"],
                            CodigoVariable = (int)valoresReader["IN_cod_variable"],
                            NombreVariable = (string)valoresReader["VC_descripcion_variable"],
                            TipoDato = (string)valoresReader["VC_tipo_dato"],
                            ValorVariable = valoresReader["IN_valor_variable"].ToString(),
                            ValorTope = valoresReader["IN_valor_tope_variable"].ToString(),
                            Activo = valoresReader["Activo"].ToString()
                        };
                        valoresSlipLista.Add(valores);
                    }

                    return valoresSlipLista;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSlipReader :: LeerValoresSlipAsync", ex);
                }
            }
        }
        

        public async Task<GetTextosSeccionSlipResponse> LeerTextosSlipAsync(int codigoCotizacion, int codigoRamo, int codigoSector, int codigoSubramo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = codigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = codigoSector;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var amparosSlipLista = new List<TextosSeccionSlip>();
                var clausulassSlipLista = new List<TextosSeccionSlip>();
                var textosSlipLista = new List<TextosSeccionSlip>();
                var disposicionesSlipLista = new List<TextosSeccionSlip>();
                var amparos = new AmparosTextosSeccionSlip();
                var clausulas = new ClausulasTextosSeccionSlip();
                var infoGeneral = new InfoGeneralTextosSeccionSlip();
                var dispociciones = new DisposicionesTextosSeccionSlip();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var codigoGrupo = SqlReaderUtilities.SafeGet<int>(reader, "IN_seccion_grupo");
                        var codigoSeccion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_seccion_slip");
                        var codigoAmparo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_amparo");
                        var especial = SqlReaderUtilities.SafeGet<int>(reader, "IN_autorizacion_gsp_esp");
                        var valores = new TextosSeccionSlip();
                        if (codigoGrupo == 2)
                        {
                            valores.CodigoSeccion = codigoSeccion.ToString();
                            valores.CodigoAmparo = codigoAmparo.ToString();
                            valores.Texto = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                            valores.Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip");
                            // valores.Especial = SqlReaderUtilities.SafeGet<bool>(reader, "Especial");
                            valores.Especial = especial == 1 ? true : false;
                            amparosSlipLista.Add(valores);
                        }
                        else if (codigoGrupo == 3)
                        {
                            valores.CodigoSeccion = codigoSeccion.ToString();
                            valores.CodigoAmparo = codigoAmparo.ToString();
                            valores.Texto = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                            valores.Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip");
                            // valores.Especial = SqlReaderUtilities.SafeGet<bool>(reader, "Especial");
                            valores.Especial = especial == 1 ? true : false;
                            clausulassSlipLista.Add(valores);
                        }
                        else if (codigoGrupo == 4)
                        {
                            valores.CodigoSeccion = codigoSeccion.ToString();
                            valores.CodigoAmparo = codigoAmparo.ToString();
                            valores.Texto = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                            valores.Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip");
                            // valores.Especial = SqlReaderUtilities.SafeGet<bool>(reader, "Especial");
                            valores.Especial = especial == 1 ? true : false;
                            disposicionesSlipLista.Add(valores);
                        }
                        else
                        {
                            valores.CodigoSeccion = codigoSeccion.ToString();
                            valores.CodigoAmparo = codigoAmparo.ToString();
                            valores.Texto = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                            valores.Seccion = SqlReaderUtilities.SafeGet<string>(reader, "VC_seccion_slip");
                            // valores.Especial = SqlReaderUtilities.SafeGet<bool>(reader, "Especial");
                            valores.Especial = especial == 1 ? true : false;
                            textosSlipLista.Add(valores);
                        }
                    }
                    amparos.Amparos = amparosSlipLista;
                    clausulas.Clausulas = clausulassSlipLista;
                    infoGeneral.InfoGeneral = textosSlipLista;
                    dispociciones.Disposiciones = disposicionesSlipLista;

                    return new GetTextosSeccionSlipResponse
                    {
                        Amparos = amparos,
                        Clausulas = clausulas,
                        InfoGeneral = infoGeneral,
                        Disposiciones = dispociciones
                    };
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSlipReader :: LeerValoresSlipAsync", ex);
                }
            }
        }

        public async Task<AmparoSlip> LeerTextoAmparoAsync(int codigoCotizacion, int codigoAmparo, int codigoRamo, int codigoSector, int codigoSubramo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_amparo", SqlDbType.Int).Value = codigoAmparo;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = codigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = codigoSector;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;

                var amparo = new AmparoSlip();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {

                        amparo.DescripcionAmparo = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                    }

                    return amparo;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSlipReader :: LeerTextoAmparopAsync", ex);
                }
            }
        }

        public async Task<Clausula> LeerTextoClausulaAsync(int codigoCotizacion, int codigoSeccion, int codigoRamo, int codigoSector, int codigoSubramo)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_ValoresRegistroSlip"
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_seccion_slip", SqlDbType.Int).Value = codigoSeccion;
                cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = codigoRamo;
                cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = codigoSubramo;
                cmd.Parameters.Add("@VAR_IN_cod_sector", SqlDbType.Int).Value = codigoSector;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 10;
                cmd.Connection = conn;

                var clausula = new Clausula();

                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {

                        clausula.DescripcionClausula = SqlReaderUtilities.SafeGet<string>(reader, "Texto");
                    }

                    return clausula;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosSlipReader :: LeerTextoClausulaAsync", ex);
                }
            }
        }
    }
}
