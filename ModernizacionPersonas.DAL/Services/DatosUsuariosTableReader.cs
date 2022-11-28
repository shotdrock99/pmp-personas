using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosUsuariosTableReader : IDatosUsuariosReader
    {
        const string SP_NAME = "PMP.USP_TB_Usuarios";

        public async Task<ApplicationUserDTO> GetUsuarioPersonasById(int codigoUsuario)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,

                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 2;
                cmd.Connection = conn;

                var result = new List<ApplicationUserDTO>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    var actived = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo");

                    var item = new ApplicationUserDTO
                    {
                        UserId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_usuario"),
                        UserName = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_usuario"),
                        RoleId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_rol"),
                        RoleName = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_rol"),
                        Name = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_completo_usuario"),
                        Email = SqlReaderUtilities.SafeGet<string>(reader, "VC_correo_usuario"),
                        Active = actived == 0 ? false : true,
                        Usuario = SqlReaderUtilities.SafeGet<string>(reader, "VC_cod_usuario"),
                        Movimiento = SqlReaderUtilities.SafeGet<string>(reader, "VC_movimiento_desc"),
                        FechaMovimiento = SqlReaderUtilities.SafeGet<DateTime>(reader, "DT_fecha_movimiento"),
                    };                        
                    

                    return item;
                }

                catch (Exception ex)
                {
                    throw new Exception("DatosUsuariosTableReader :: GetUsuariosPersonasAsync", ex);
                }
            }
        }

        public Task<ApplicationUserDTO> GetUsuarioPersonasByName()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ApplicationUserDTO>> GetUsuariosPersonasAsync()
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

                var result = new List<ApplicationUserDTO>();
                try
                {
                    var reader = await cmd.ExecuteReaderAsync();
                    
                    while (await reader.ReadAsync())
                    {
                        var actived = SqlReaderUtilities.SafeGet<int>(reader, "IN_activo");
                        var item = new ApplicationUserDTO
                        {
                            UserId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_usuario"),
                            UserName = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_usuario"),
                            RoleId = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_rol"),
                            RoleName = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_rol"),
                            Name = SqlReaderUtilities.SafeGet<string>(reader, "VC_nombre_completo_usuario"),
                            Email = SqlReaderUtilities.SafeGet<string>(reader, "VC_correo_usuario"),
                            Active = actived == 0 ? false : true,
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
                    throw new Exception("DatosUsuariosTableReader :: GetUsuariosPersonasAsync", ex);
                }
            }
        }
    }
}
