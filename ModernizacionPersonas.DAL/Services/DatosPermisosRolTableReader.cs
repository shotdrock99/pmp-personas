using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosPermisosRolTableReader : IDatosPermisosRolReader
    {
        private const string SP_NAME = "PMP.USP_TB_PermisosxRol";

        public async Task<IEnumerable<Permiso>> GetPermisosAsync()
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
                var result = new List<Permiso>();
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var permiso = new Permiso
                            {
                                Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_permiso"),
                                Nombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_permiso"),
                                Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_permiso")
                            };

                            result.Add(permiso);
                        }

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableReader :: GetRolesAsync", ex);
                }
            }
        }

        public async Task<IEnumerable<Permiso>> GetPermisosRolAsync(int codigoRol)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_rol", SqlDbType.Int).Value = codigoRol;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;
                var result = new List<Permiso>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var item = new Permiso
                        {
                            Codigo = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_permiso"),
                            Nombre = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_permiso"),
                            Descripcion = SqlReaderUtilities.SafeGet<string>(reader, "VC_descripcion_permiso")
                        };

                        result.Add(item);
                    }

                    return result;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosPermisosRolTableReader :: GetPermisosRolAsync", ex);
                }
            }
        }
    }
}
