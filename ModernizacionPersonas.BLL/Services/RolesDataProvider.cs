using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class RolesDataProvider
    {
        private readonly IDatosRolesReader datosRolesReader;
        private readonly IDatosRolesWriter datosRolesWriter;
        private readonly PermisosDataProvider permisosDataProvider;

        public RolesDataProvider()
        {
            this.datosRolesReader = new DatosRolesTableReader();
            this.datosRolesWriter = new DatosRolesTableWriter();
            this.permisosDataProvider = new PermisosDataProvider();

        }

        public async Task<IEnumerable<Rol>> GetRolesAsync()
        {
            var control = false;
            try
            {
                var roles = await this.datosRolesReader.GetRolesAsync();
                var rolesSISe = await this.datosRolesReader.GetRolesSiseAsync();
                control = await ValidarActualizacionesRolAsync(roles, rolesSISe);
                if (control) {
                    roles = await this.datosRolesReader.GetRolesAsync();
                }
                return roles;
            }
            catch (Exception ex)
            {
                throw new Exception("RolesDataProvider :: GetRoless" + control + ex.StackTrace.ToString() + ex.Message.ToString(), ex);
            }
        }

        public async Task<ActionResponseBase> CreateRolAsync(Rol rol)
        {
            try
            {
                var nuevoRol = await this.datosRolesWriter.GuardarRolAsync(rol);
                await this.permisosDataProvider.CreatePermisosRolAsync(rol.Permisos, nuevoRol);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("RolesDataProvider :: CreateRolAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdateRolAsync(Rol rol)
        {
            try
            {
                await this.datosRolesWriter.ActualizarRolAsync(rol);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("RolesDataProvider :: UpdateRolAsync", ex);
            }
        }
        public async Task<bool> ValidarActualizacionesRolAsync(IEnumerable<Rol> roles, IEnumerable<RolSISE> rolesSISE)
        {
            try
            {
                bool Flag = false;
                //Validar si en sise se actualizó algun nombre de cargo
                foreach (var ind in rolesSISE)
                {
                    foreach (var i in roles)
                    {
                        if (i.Codigo == ind.Codigo)
                        {
                            if (i.Nombre != ind.Nombre)
                            {
                                i.Nombre = ind.Nombre;
                                await this.datosRolesWriter.ActualizarRolAsync(i);
                                Flag = true;
                            }

                        }
                    }
                }
                //Validar si en sise se han agregado cargos
                if (roles.Count() != rolesSISE.Count())
                {
                    var lst3 = new List<RolSISE>();

                    foreach (var s2 in rolesSISE)
                    {
                        bool existed = false;

                        foreach (var s3 in roles)
                        {
                            if (s2.Codigo == s3.Codigo)
                            {
                                existed = true;
                                break;
                            }
                        }

                        if (!existed)
                        {
                            lst3.Add(s2);
                        }
                    }
                    foreach (var i in lst3)
                    {
                        var unit = new Rol
                        {
                            Nombre = i.Nombre,
                            Codigo = i.Codigo,
                            Descripcion = i.Nombre,
                            Usuario = "CAMORENO",

                        };
                        Flag = true;
                        await this.datosRolesWriter.GuardarRolAsync(unit);
                    }

                }
                return Flag;
            }
            catch (Exception ex)
            {
                throw new Exception("RolesDataProvider :: validarActualizacionesRolAsync", ex);
            }
        }
    }
}
