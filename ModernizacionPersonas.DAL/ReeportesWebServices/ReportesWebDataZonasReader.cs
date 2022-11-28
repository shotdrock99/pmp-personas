using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.ReportesWebServices
{
    public class ReportesWebDataZonasReader
    {
        public async Task<IEnumerable<SucursalReportes>> GetSucursalesInfoAsync(int codigoZona)
        {
            using (var conn = ConnectionFactory.ReportesWebConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "select  sc.cod_suc, sc.txt_nom_suc, sc.txt_direccion, sc.txt_telefono from trweb_tzona_suc zsuc INNER JOIN tsuc sc ON sc.cod_suc = zsuc.cod_suc where zsuc.cod_zona = {codigoZona}",
                    Connection = conn
                };
                try
                {
                    var sucursales = new List<SucursalReportes>();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {

                        var result = new SucursalReportes()
                        {
                            CodigoSucursal = SqlReaderUtilities.SafeGet<int>(reader, "cod_suc"),
                            NombreSucursal = SqlReaderUtilities.SafeGet<string>(reader, "txt_nom_suc"),
                            Direccion = SqlReaderUtilities.SafeGet<string>(reader, "txt_direccion").ToString(),
                            Telefono = SqlReaderUtilities.SafeGet<string>(reader, "txt_telefono"),
                        };

                        sucursales.Add(result);

                    }
                    return sucursales;
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: GetNumeroCotizacion", ex);
                }
            }
        }

        public async Task<ZonasResponse> GetZonasAsync()
        {
            using (var conn = ConnectionFactory.ReportesWebConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "select * from trweb_tzona where sn_activo = 1",
                    Connection = conn
                };
                try
                {
                    var zonas = new List<ZonaReportes>();
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var codigoZona = SqlReaderUtilities.SafeGet<int>(reader, "cod_zona");
                        var result = new ZonaReportes()
                        {
                            CodigoZona = int.Parse(codigoZona.ToString()),
                            Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc"),
                            CodigoGrupo = SqlReaderUtilities.SafeGet<int>(reader, "cod_grupo"),
                            Activo = SqlReaderUtilities.SafeGet<int>(reader, "sn_activo"),
                        };

                        zonas.Add(result);
                    }

                    return new ZonasResponse()
                    {
                        Zonas = zonas
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: GetNumeroCotizacion", ex);
                }
            }
        }
    }

    public class ZonaReportes
    {
        public int CodigoZona { set; get; }
        public string Descripcion { set; get; }
        public int CodigoGrupo { set; get; }
        public int Activo { set; get; }
    }

    public class ZonasResponse
    {
        public List<ZonaReportes> Zonas { set; get; }
        public ZonasResponse()
        {
            Zonas = new List<ZonaReportes>();
        }
    }

    public class SucursalReportes
    {
        public int CodigoSucursal { set; get; }
        public string NombreSucursal { set; get; }
        public string Direccion { set; get; }
        public string Telefono { set; get; }

    }
}
