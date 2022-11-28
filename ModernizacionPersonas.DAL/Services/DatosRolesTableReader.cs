using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosRolesTableReader : IDatosRolesReader
    {
        private readonly IDatosPermisosRolReader permisorolesReader;
        private const string SP_NAME = "PMP.USP_TB_Rol";

        public DatosRolesTableReader()
        {
            this.permisorolesReader = new DatosPermisosRolTableReader();
        }
        public async Task<IEnumerable<Rol>> GetRolesAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {

                var cmd2 = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd2.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 1;
                cmd2.Connection = conn;
                var result = new List<Rol>();
                try
                {
                    using (var reader = await cmd2.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var causal = new Rol
                            {
                                Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_rol"),
                                Nombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_rol"),
                                Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_rol"),
                                Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                                Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                                FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                            };
                            var permisosList = await permisorolesReader.GetPermisosRolAsync(causal.Codigo);
                            causal.Permisos = permisosList;

                            result.Add(causal);
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosRolesTableReader :: GetRolesAsync", ex);
                }
            }
        }
        public async Task<IEnumerable<RolSISE>> GetRolesSiseAsync()
        {

            using (var conSiSe = ConnectionFactory.SISEConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "usp_pmp_autorizaciones"
                };

                cmd.Parameters.Add("@VAR_IN_tipo_tran", SqlDbType.Int).Value = 3;
                cmd.Connection = conSiSe;
                var resultSISE = new List<RolSISE>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var causal = new RolSISE
                        {
                            Codigo = (int)SqlReaderUtilities.SafeGet<decimal>(reader, "cod_rol"),
                            Nombre = SqlReaderUtilities.SafeGet<string>(reader, "txt_desc_rol"),
                        };
                        resultSISE.Add(causal);
                    }
                    return resultSISE;
                }

            }

        }

    }
}
