using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class PermisosDataProvider
    {
        private readonly IDatosPermisosRolReader datosPermisosRolReader;
        private readonly IDatosPermisosRolWriter datosPermisosRolWriter;
        public PermisosDataProvider()
        {
            this.datosPermisosRolReader = new DatosPermisosRolTableReader();
            this.datosPermisosRolWriter = new DatosPermisosRolTableWriter();
        }

        public async Task<IEnumerable<Permiso>> GetPermisosAsync()
        {
            try
            {
                var permisos = await this.datosPermisosRolReader.GetPermisosAsync();
                return permisos;
            }
            catch (Exception ex)
            {
                throw new Exception("PermisosDataProvider :: GetPermisos", ex);
            }
        }

        public async Task<ActionResponseBase> CreatePermisosRolAsync(IEnumerable<Permiso> permisos, int codigoRol)
        {
            try
            {
                foreach (var permiso in permisos)
                {
                    var permisoRol = new PermisoRol
                    {
                        CodigoPermiso = permiso.Codigo,
                        CodigoRol = codigoRol
                    };

                    await this.datosPermisosRolWriter.GuardarPermisoRolAsync(permisoRol);
                }

                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("PermisosDataProvider :: CreatePermisosRolAsync", ex);
            }
        }

        public async Task<ActionResponseBase> UpdatePermisosRolAsync(IEnumerable<Permiso> permisos, int codigoRol)
        {
            try
            {
                await this.datosPermisosRolWriter.EliminarPermisosRolAsync(codigoRol);

                foreach (var permiso in permisos)
                {
                    var permisoRol = new PermisoRol
                    {
                        CodigoPermiso = permiso.Codigo,
                        CodigoRol = codigoRol
                    };

                    await this.datosPermisosRolWriter.GuardarPermisoRolAsync(permisoRol);
                }

                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("PermisosDataProvider :: UpdatePermisosRolAsync", ex);
            }
        }
    }
}
