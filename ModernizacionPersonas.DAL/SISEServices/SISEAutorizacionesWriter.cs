using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.SISEServices
{
    public class SISEAutorizacionesWriter
    {
        const string SP_NAME = "usp_pmp_autorizaciones";

        public async Task InsertarAutorizacionesAsync(int codigoCotizacion, List<CotizacionAuthorization> autorizaciones)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };


                cmd.Parameters.Add("@VAR_TB_autorizaciones", SqlDbType.Structured).Value = this.CreateAutorizacionesDataTable(autorizaciones);
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 1;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAutorizacionesWriter :: InsertarAutorizacionesAsync", ex);
                }
            }
        }

        public async Task RemoverAutorizacionesAsync(int codigoCotizacion, int version)
        {
            using (var conn = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };


                cmd.Parameters.AddWithValue("@VAR_IN_cod_cotizacion", codigoCotizacion);
                cmd.Parameters.AddWithValue("@VAR_IN_cod_version", version);
                cmd.Parameters.AddWithValue("@VAR_IN_tipo_tran", 2);
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("SISEAutorizacionesWriter :: RemoverAutorizacionesAsync", ex);
                }
            }
        }

        private DataTable CreateAutorizacionesDataTable(IEnumerable<CotizacionAuthorization> autorizaciones)
        {
            DataTable table = new DataTable();
            table.Columns.Add("id_cotizacion", typeof(int));
            table.Columns.Add("id_version", typeof(int));
            table.Columns.Add("cod_grupo_aseg", typeof(int));
            table.Columns.Add("cod_suc", typeof(int));
            table.Columns.Add("cod_ramo", typeof(int));
            table.Columns.Add("cod_subramo", typeof(int));
            table.Columns.Add("cod_amparo", typeof(int));
            table.Columns.Add("campo_entrada", typeof(string));
            table.Columns.Add("valor_entrada", typeof(decimal));
            table.Columns.Add("cod_tipo_aut", typeof(int));
            table.Columns.Add("sn_requiere_aut", typeof(int));
            table.Columns.Add("cod_usuario", typeof(string));
            table.Columns.Add("txt_respuesta", typeof(string));
            table.Columns.Add("txt_tipo_aut", typeof(string));

            foreach (var item in autorizaciones)
            {
                var row = new object[14];
                row[0] = item.CodigoCotizacion;
                row[1] = item.Version;
                row[2] = item.CodigoGrupoAsegurado;
                row[3] = item.CodigoSucursal;
                row[4] = item.CodigoRamo;
                row[5] = item.CodigoSubramo;
                row[6] = item.CodigoAmparo;
                row[7] = item.CampoEntrada;
                row[8] = item.ValorEntrada;
                row[9] = item.CodigoTipoAutorizacion;
                row[10] = item.RequiereAutorizacion;
                row[11] = item.CodigoUsuario;
                row[12] = item.MensajeValidacion;
                row[13] = item.TipoAutorizacion;

                table.Rows.Add(row);
            }
            return table;
        }

    }

}
