using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using ModernizacionPersonas.DAL.Entities.SISEEntities;

namespace ModernizacionPersonas.DAL.SISEServices
{
    public class SISEAseguradosWriter
    {
        const string SP_NAME = "usp_pmp_listado_asegurados";

        public async Task<InsertarAseguradosResponse> InsertarAseguradosAsync(SISEListadoAseguradosArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.AddWithValue("@VAR_IN_id_proceso", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_cotiza", args.CodigoCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_version", args.VersionCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_ramo", args.CodigoRamo);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_subramo", args.CodigoSubramo);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", args.CodigoSector);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_tipo_tasa", args.CodigoTipoTasa);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_grupo_suma_aseg", args.CodigoGrupoAsegurados);
                cmd.Parameters.AddWithValue("@VAR_IN_cnt_aseg", args.NumeroAsegurados);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil1", args.CodigoPerfil1);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil2", args.CodigoPerfil2);
                cmd.Parameters.AddWithValue("@VAR_SN_listado", SqlDbType.Bit).Value = args.ConListaAsegurados ? 1 : 0;
                cmd.Parameters.Add("@VAR_TB_amparos", SqlDbType.Structured).Value = this.CreateAmparosDataTable(args.Amparos, args.ValorAsegurado);
                cmd.Parameters.Add("@VAR_TB_asegurados", SqlDbType.Structured).Value = this.CreateAseguradosDataTableConValorAsegurado(args.Asegurados, args.TipoEstructura);
                cmd.Parameters.Add("@VAR_TB_rangos", SqlDbType.Structured).Value = this.CreateRangosDataTable(args.Rangos, args.EdadPromedio);
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                try
                {
                    var cmdResponse = await cmd.ExecuteScalarAsync();
                    return new InsertarAseguradosResponse
                    {
                        Message = cmdResponse.ToString()
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosWriter :: InsertarAseguradosAsync", ex);
                }
            }
        }

        public async Task<bool> RemoverAseguradosAsync(SISEListadoAseguradosArgs args)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.AddWithValue("@VAR_IN_id_proceso", 0);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_cotiza", args.CodigoCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_version", args.VersionCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_ramo", args.CodigoRamo);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_subramo", args.CodigoSubramo);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_sector", args.CodigoSector);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_tipo_tasa", args.CodigoTipoTasa);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_grupo_suma_aseg", args.CodigoGrupoAsegurados);
                cmd.Parameters.AddWithValue("@VAR_IN_cnt_aseg", args.NumeroAsegurados);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil1", args.CodigoPerfil1);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_perfil2", args.CodigoPerfil2);
                cmd.Parameters.AddWithValue("@VAR_SN_listado", SqlDbType.Bit).Value = args.ConListaAsegurados ? 1 : 0;
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", 2);
                cmd.Connection = conn;
                try
                {
                    var cmdResponse = await cmd.ExecuteScalarAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAseguradosWriter :: RemoverAseguradosAsync", ex);
                }
            }
        }

        private DataTable CreateRangosDataTable(IEnumerable<Rango> rangos, int edadPromedio)
        {
            DataTable table = new DataTable();
            table.Columns.Add("rango", typeof(int));
            table.Columns.Add("edad_min", typeof(int));
            table.Columns.Add("edad_max", typeof(int));
            table.Columns.Add("cnt_aseg", typeof(int));
            table.Columns.Add("edad_prom", typeof(int));
            table.Columns.Add("edad_max_aseg", typeof(int));
            table.Columns.Add("vr_aseg", typeof(decimal));
            

            var rangoIdx = 0;
            foreach (var item in rangos)
            {
                var row = new object[7];
                row[0] = rangoIdx;
                row[1] = item.EdadMinAsegurado;
                row[2] = item.EdadMaxAsegurado;
                row[3] = item.NumeroAsegurados;
                if(edadPromedio > 0)
                {
                    if (edadPromedio <= item.EdadMaxAsegurado && edadPromedio >= item.EdadMinAsegurado)
                    {
                        row[4] = edadPromedio;
                        row[5] = item.EdadMaxRango;
                    }
                    else
                    {
                        row[4] = 0;
                        row[5] = item.EdadMaxRango;
                    }
                }
                else
                {
                    row[4] = (item.EdadMinAsegurado + item.EdadMaxAsegurado) / 2;
                    row[5] = item.EdadMaxAsegurado;
                }
                
                
                row[6] = item.ValorAsegurado;
                
                             

                table.Rows.Add(row);
                rangoIdx++;
            }
            return table;
        }

        private DataTable CreateAseguradosDataTableConValorAsegurado(IEnumerable<Asegurado> asegurados, int tipoEstructura)
        {
            
            DataTable table = new DataTable();
            table.Columns.Add("nro_doc", typeof(string));
            table.Columns.Add("nombre", typeof(string));
            table.Columns.Add("edad", typeof(int));
            table.Columns.Add("vr_aseg", typeof(decimal));
            int cont = 1;
            foreach (var item in asegurados)
            {
                if(item.VetadoSarlaft == false)
                {
                    var row = new object[4];
                    if (tipoEstructura == 1 || tipoEstructura == 2) row[0] = item.NumeroDocumento;
                    if (tipoEstructura == 3 || tipoEstructura == 4) row[0] = cont;
                    row[1] = item.NumeroDocumento;
                    row[2] = item.Edad;
                    row[3] = item.ValorAsegurado;

                    table.Rows.Add(row);
                }
                cont++;
            }
            return table;
        }

        private DataTable CreateAseguradosDataTable(IEnumerable<Asegurado> s)
        {
            DataTable table = new DataTable();
            table.Columns.Add("nro_doc", typeof(string));
            table.Columns.Add("edad", typeof(int));
            foreach (var item in s)
            {
                var row = new object[2];
                row[0] = item.NumeroDocumento;
                row[1] = item.Edad;

                table.Rows.Add(row);
            }
            return table;
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            int edad = 0;
            edad = DateTime.Now.Year - fechaNacimiento.Year;
            if (DateTime.Now.DayOfYear < fechaNacimiento.DayOfYear)
                edad = edad - 1;

            return edad;
        }

        private DataTable CreateAmparosDataTablePorOpcion(IEnumerable<AmparoGrupoAsegurado> amparos, decimal valorAsegurado)
        {
            var table = new DataTable();
            //table.Columns.Add("id", typeof(int));
            table.Columns.Add("cod_amparo", typeof(int));
            table.Columns.Add("vr_aseg", typeof(decimal));
            foreach (var amparo in amparos)
            {
                foreach (var opcion in amparo.OpcionesValores)
                {
                    if (opcion.ValorAsegurado > 0)
                    {
                        var row = new object[2];
                        row[0] = amparo.CodigoAmparo;
                        row[1] = opcion.ValorAsegurado;

                        table.Rows.Add(row);
                    }
                }
            }
            return table;
        }

        private DataTable CreateAmparosDataTable(IEnumerable<AmparoGrupoAsegurado> amparos, decimal valorAsegurado)
        {
            var table = new DataTable();
            table.Columns.Add("cod_amparo", typeof(int));
            foreach (var amparo in amparos)
            {
                var row = new object[1];
                row[0] = amparo.CodigoAmparo;

                table.Rows.Add(row);
            }
            return table;
        }
    }

    public class SISEListadoAseguradosArgs
    {
        public int CodigoCotizacion { get; set; }
        public int VersionCotizacion { get; set; }
        public int CodigoGrupoAsegurados { get; set; }
        public int CodigoTipoSumaAsegurada { get; set; }
        public int CodigoRamo { get; set; }
        public int CodigoSubramo { get; set; }
        public int CodigoTipoTasa { get; set; }
        public bool ConListaAsegurados { get; set; }
        public int CodigoPerfil1 { get; set; }
        public int CodigoPerfil2 { get; set; }
        public IEnumerable<Asegurado> Asegurados { get; set; }
        public IEnumerable<AmparoGrupoAsegurado> Amparos { get; set; }
        public IEnumerable<Rango> Rangos { get; set; }
        public decimal ValorAsegurado { get; set; }
        public int NumeroAsegurados { get; set; }
        public int CodigoSector { get; set; }
        public int EdadPromedio { get; set; }
        public int TipoEstructura { get; set; }
    }
}
