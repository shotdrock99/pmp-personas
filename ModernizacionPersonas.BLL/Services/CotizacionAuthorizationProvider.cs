using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.BLL.Utilities;
using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class CotizacionAuthorizationProvider
    {
        private readonly IDatosInformacionNegocioReader informacionNegocioReader;
        private readonly IDatosPersonasReader datosPersonasReader;
        private readonly IDatosCotizacionReader datosCotizacionReader;
        private readonly IAuthorizationsDataReader authorizationsReader;
        private readonly IAuthorizationsUsersDataReader authorizationUsersReader;
        private readonly IDatosRolesReader rolesReader;
        private readonly IDatosGruposAseguradoReader gruposAseguradosReader;
        private readonly ITransactionsDataReader transaccionesReader;
        private readonly DatosCotizacionUtilities datosCotizacionUtilities;
        private readonly SoligesproDataUsuariosReader soligesproDataUsuariosReader;

        public CotizacionAuthorizationProvider()
        {
            this.informacionNegocioReader = new DatosInformacionNegocioTableReader();
            this.gruposAseguradosReader = new DatosGruposAseguradosTableReader();
            this.datosPersonasReader = new InformacionPersonasReader();
            this.datosCotizacionReader = new DatosCotizacionTableReader();
            this.authorizationsReader = new AuthorizationsDataTableReader();
            this.authorizationUsersReader = new AuthorizationsUsersDataTableReader();
            this.rolesReader = new DatosRolesTableReader();
            this.transaccionesReader = new TransactionsDataTableReader();
            this.soligesproDataUsuariosReader = new SoligesproDataUsuariosReader();

            this.datosCotizacionUtilities = new DatosCotizacionUtilities(this.datosPersonasReader);
        }

        public async Task<IEnumerable<CotizacionItemList>> GetCotizacionesPorAutorizarAsync(string userName, string userRole, CotizacionFilter filterArgs)
        {
            try
            {
                IEnumerable<CotizacionItemList> data = new List<CotizacionItemList>();
                IEnumerable<CotizacionItemList> data2 = new List<CotizacionItemList>();
                var user = await this.soligesproDataUsuariosReader.GetUserAsync(userName);
                var filterData = new List<CotizacionItemList>();
                data = await this.datosCotizacionReader.GetPendingAuthorizationCotizacionesAsync(filterArgs);
                
                filterArgs.CodigoUsuario = null;
                data2 = await this.datosCotizacionReader.GetPendingAuthorizationCotizacionesAsync(filterArgs);

                foreach(var cot in data)
                {
                    var cotAux = data2.Where(x => x.CodigoCotizacion == cot.CodigoCotizacion);
                    filterData.AddRange(cotAux);
                }

                //Se excluye el control por roles para la visualización de roles.
                // 16-03-2022 Sebastián Valencia

                               
                //filterData = filterData.Where(x => x.UsuarioNotificado == userName).ToList();
                filterData = filterData.Where(x => x.UsuarioRealNotifica == 1).ToList();
                filterData = filterData.Distinct().ToList();
                filterData = filterData.GroupBy(i => i.CodigoCotizacion).Select(i => i.FirstOrDefault()).ToList();


                await this.datosCotizacionUtilities.AgreggateCotizacionModelAsync(filterData);
                return filterData;
            }
            catch (Exception ex)
            {
                throw new Exception("CotizacionDataProvider :: GetCotizacionesAsync", ex);
            }
        }

        public async Task<IEnumerable<AuthorizationUser>> GetAuthorizationsUsersAsync(int codigoCotizacion, int version)
        {
            var response = await this.authorizationUsersReader.GetAuthorizationsUsersAsync(codigoCotizacion, version);
            await this.AggregateUserInfo(response);
            return response;
        }

        private async Task AggregateUserInfo(IEnumerable<AuthorizationUser> users)
        {
            var roles = await this.rolesReader.GetRolesAsync();
            foreach (var user in users)
            {
                var rol = roles.Where(x => x.Codigo == user.CodigoRol).FirstOrDefault();
                user.NombreRol = rol.Nombre;
            }
        }

        public async Task<GetAuthorizationInfoResponse> GetCotizacionAuthorizationsAsync(int codigoCotizacion, int version)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var NumCot = int.Parse(informacionNegocio.NumeroCotizacion);
            var groups = await this.gruposAseguradosReader.GetGruposAseguradosAsync(codigoCotizacion);

            var result = new GetAuthorizationInfoResponse();
            var response = await this.authorizationsReader.GetAuthorizationsByCotizacionAsync(NumCot, informacionNegocio.Version);
            var ageControls = response.Authorizations.Where(x => x.CampoEntrada.StartsWith("edad_") &&  x.CodigoAmparo != 0);
            var authorizations = new List<CotizacionAuthorizationDTO>();
            var exceptItems = response.Authorizations.Except(ageControls);

            var amparos = await this.datosPersonasReader.TraerAmparosAsync(informacionNegocio.CodigoRamo, informacionNegocio.CodigoSubramo, informacionNegocio.CodigoSector);
            foreach (var item in exceptItems)
            {
                var dto = CotizacionAuthorizationDTO.Create(item);
                var grupo = groups.Where(x => x.CodigoGrupoAsegurado == item.CodigoGrupoAsegurado).FirstOrDefault();
                if (grupo != null)
                {
                    dto.NombreGrupoAsegurado = grupo.NombreGrupoAsegurado;
                  
                }

                var amparo = amparos.Where(x => x.CodigoAmparo == item.CodigoAmparo).FirstOrDefault();
                if (amparo != null)
                {
                    dto.NombreAmparo = amparo.NombreAmparo;
                }

                dto.SiseAuth = item.SiseAuth;

                authorizations.Add(dto);
            }

            var aggregatedResult = new List<CotizacionAuthorizationDTO>();
            foreach (var item in ageControls)
            {
                var dto = CotizacionAuthorizationDTO.Create(item);
                var grupo = groups.Where(x => x.CodigoGrupoAsegurado == item.CodigoGrupoAsegurado).FirstOrDefault();
                if (grupo != null)
                {
                    dto.NombreGrupoAsegurado = grupo.NombreGrupoAsegurado;
                }

                var amparo = amparos.Where(x => x.CodigoAmparo == item.CodigoAmparo).FirstOrDefault();
                if (amparo != null)
                {
                    dto.NombreAmparo = amparo.NombreAmparo;
                }

                aggregatedResult.Add(dto);
            }

            var items = new List<CotizacionAuthorizationDTO>();
            // group by groups
            var queryResult = from a in aggregatedResult
                              group a by a.CodigoGrupoAsegurado into newGroup
                              select newGroup;

            for (int i = 0; i < queryResult.Count(); i++)
            {
                var groupItems = queryResult.ToArray()[i];
                var group = groups.Where(x => x.CodigoGrupoAsegurado == (int)groupItems.FirstOrDefault().CodigoGrupoAsegurado).FirstOrDefault();
                items.Add(new CotizacionAuthorizationDTO
                {
                    CodigoCotizacion = codigoCotizacion,
                    NombreGrupoAsegurado = group.NombreGrupoAsegurado,
                    Items = groupItems
                });
            }

            if (ageControls.Count() > 0)
            {
                authorizations.Add(new CotizacionAuthorizationDTO
                {
                    CodigoCotizacion = codigoCotizacion,
                    CodigoSucursal = informacionNegocio.CodigoSucursal,
                    CodigoRamo = informacionNegocio.CodigoRamo,
                    CodigoSubramo = informacionNegocio.CodigoSubramo,
                    CampoEntrada = "edad",
                    MensajeValidacion = "Modificación de edades mínimas o máximas requieren autorización.",
                    Items = items
                });
            }

            result.Authorizations = authorizations.Where(x => x.CampoEntrada != "actividad" );
            var autoriExc = result.Authorizations.Where(x => String.IsNullOrEmpty(x.NombreSeccion) && x.CodigoAmparo == 0 && x.CodigoTipoAutorizacion == 2 && x.SiseAuth == false);
            result.Authorizations = result.Authorizations.Except(autoriExc);    
            foreach(var auth in result.Authorizations) {
                if (auth.CampoEntrada.Contains("condiciones")){
                    auth.NombreSeccion = "CONDICIONES ESPECIALES";
                }
            }
            return result;
        }

        public async Task<GetAuthorizationInfoResponse> GetAuthorizationInfoAsync(int codigoCotizacion, string userName)
        {
            var informacionNegocio = await this.informacionNegocioReader.LeerInformacionNegocioAsync(codigoCotizacion);
            var user = await this.soligesproDataUsuariosReader.GetUserAsync(informacionNegocio.LastAuthorName);
            var lastRoleModifyUser = user.Cargo;
            var numCot = int.Parse(informacionNegocio.NumeroCotizacion);

            var authControlsResponse = await this.GetCotizacionAuthorizationsAsync(codigoCotizacion, 0);
            
            var users = await this.GetAuthorizationsUsersAsync(numCot, informacionNegocio.Version);
            var activeUsers = users.Where(x => x.Activo);
            return new GetAuthorizationInfoResponse
            {
                LastModifyUser = informacionNegocio.LastAuthorName,
                LastRoleModifyUser = lastRoleModifyUser,
                CodigoCotizacion = codigoCotizacion,
                Version = authControlsResponse.Version,
                NumeroCotizacion = authControlsResponse.NumeroCotizacion,
                CodigoEstadoCotizacion = authControlsResponse.CodigoEstadoCotizacion,
                Authorizations = authControlsResponse.Authorizations,
                Users = users
            };
        }
    }
}
