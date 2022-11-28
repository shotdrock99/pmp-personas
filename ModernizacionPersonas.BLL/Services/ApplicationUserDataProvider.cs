using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class ApplicationUserDataProvider
    {
        private readonly IDatosUsuariosReader datosUsuariosReader;
        private readonly ISoligesproDataUsuariosReader soligesproUsersDataReader;
        private readonly IDatosUsuariosWriter datosUsuariosWriter;
        private readonly AdministracionPersonasReader administracionPersonasReader;

        public ApplicationUserDataProvider()
        {
            this.datosUsuariosReader = new DatosUsuariosTableReader();
            this.soligesproUsersDataReader = new SoligesproDataUsuariosReader();
            this.datosUsuariosWriter = new DatosUsuarioTableWriter();
            this.administracionPersonasReader = new AdministracionPersonasReader();
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            var users = await this.datosUsuariosReader.GetUsuariosPersonasAsync();
            var result = (from uid in users.Select(x => x.UserId).Distinct()
                          join u in users on uid equals u.UserId into g
                          select new ApplicationUser
                          {
                              UserId = uid,
                              //DocumentId = u.DocumentId,
                              UserName = g.Select(x => x).FirstOrDefault().UserName,
                              Activo = g.Select(x => x).FirstOrDefault().Active,
                              Name = g.Select(x => x).FirstOrDefault().Name,
                              Email = g.Select(x => x).FirstOrDefault().Email,
                              Usuario = g.Select(x => x).FirstOrDefault().Usuario,
                              FechaMovimiento = g.Select(x => x).FirstOrDefault().FechaMovimiento,
                              Rol = g.Select(x =>
                              {
                                  return new ApplicationRole
                                  {
                                      RoleId = x.RoleId,
                                      RoleName = x.RoleName
                                  };
                              }).FirstOrDefault()

                          }).ToList();

            return result;
        }

        public async Task<ApplicationUser> GetUserAsync(string userName)
        {
            var users = await this.GetUsersAsync();
            var user = users.Where(x => x.UserName.ToUpper() == userName.ToUpper()).FirstOrDefault();
            if (user != null)
            {

                // consultar informacion Soligespro
                var externalInfo = await this.soligesproUsersDataReader.GetUserAsync(userName);
                if (externalInfo.LoginUsuario != null)
                {
                    user.ExternalInfo = externalInfo;
                    return user;
                }
            }

            return null;
        }

        public async Task<ActionResponseBase> UpdateUserAsync(ApplicationUser usuario)
        {
            try
            {
                await this.datosUsuariosWriter.ActualizarUsuarioAsyn(usuario);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("UserDataProvider :: UpdateUserAsync", ex);
            }
        }

        public async Task<ActionResponseBase> CreateUserAsync(ApplicationUser usuario)
        {
            try
            {
                await this.datosUsuariosWriter.CrearUsuarioAsync(usuario);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("UserDataProvider :: CreateUserAsync", ex);
            }
        }

        public async Task<ActionResponseBase> DisableUserAsync(int userId, string userName)
        {
            try
            {
                await this.datosUsuariosWriter.ActivarDesactivarUsuarioAsync(userId, userName);
                return new ActionResponseBase();
            }
            catch (Exception ex)
            {
                throw new Exception("UserDataProvider :: DisableUserAsync", ex);
            }
        }

        public async Task<ApplicationUserResponse> ValidateUserAsync(string userName)
        {
            var soligesproUser = await this.soligesproUsersDataReader.GetUserAsync(userName);
            if (soligesproUser.LoginUsuario != null)
            {
                var users = await this.GetUsersAsync();
                var user = users.Where(x => x.UserName == soligesproUser.LoginUsuario).FirstOrDefault();
                if (user == null)
                {
                    return new ApplicationUserResponse(new ApplicationUser { ExternalInfo = soligesproUser }, null);
                }

                return new ApplicationUserResponse(null, "El usuario ya se encuentra registrado");
            }

            return new ApplicationUserResponse(null, "El usuario no existe en Soligespro");
        }

        public async Task<ApplicationUserResponse> ValidateIntermediario(int codigoIntermediario)
        {
            var intermediario = await this.administracionPersonasReader.TraerIntermediarioAsync(codigoIntermediario);
            if (intermediario != null || intermediario.Codigo != null)
            {
                var users = await this.GetUsersAsync();
                var user = users.Where(x => x.UserName == intermediario.Codigo.ToString()).FirstOrDefault();
                if (user == null)
                {
                    return new ApplicationUserResponse(new ApplicationUser
                    {
                        Name = string.Concat(intermediario.PrimerNombre, intermediario.PrimerApellido),
                        Email = intermediario.correo.Where(x => x.TipoCorreo == 10).Select(x => x.CorreoElectronico).FirstOrDefault()
                    }, null);

                }

                return new ApplicationUserResponse(null, "El intermediario ya se encuentra registrado");
            }

            return new ApplicationUserResponse(null, "El intermediario no existe en Soligespro");
        }
    }
}
