using AdministracionPersonasServiceReference;
using ModernizacionPersonas.Common.Configuration;
using System;
using System.ServiceModel;
using UtilidadesSolidariaServiceReference;

namespace ModernizacionPersonas.Common
{
    public class ServiceConnectionFactory
    {
        private static readonly BasicHttpBinding basicBinding = new BasicHttpBinding();
        private static AppConfiguration appConfiguration = new AppConfiguration();

        public static PersonasServiceReference.Service1Client GetParametrizacionPersonasClient()
        {
            try
            {
                var uri = appConfiguration.ParametrizacionPersonasUri;
                var client = new PersonasServiceReference.Service1Client();
                var endpoint = new EndpointAddress(uri);
                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetParametrizacionPersonasClient", ex);
            }
        }

        public static WCFSolidariaUtilidadesClient GetUtilidadesSolidariaClient()
        {
            try
            {
                var uri = appConfiguration.UtilidadesSolidariaUri;
                var client = new WCFSolidariaUtilidadesClient();
                var endpoint = new EndpointAddress(uri);
                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetAdministracionPersonasClient", ex);
            }
        }

        public static SARLAFTServiceReference.ServiceClient GetSARLAFTServiceClient()
        {
            try
            {
                var uri = appConfiguration.SARLAFTUri;
                var client = new SARLAFTServiceReference.ServiceClient();
                var endpoint = new EndpointAddress(uri);
                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetSARLAFTServiceClient", ex);
            }
        }

        public static WCFAdministracionPersonasClient GetAdministracionPersonasClient()
        {
            try
            {
                var uri = appConfiguration.AdministracionPersonasUri;
                var client = new WCFAdministracionPersonasClient();
                var endpoint = new EndpointAddress(uri);
                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetAdministracionPersonasClient", ex);
            }
        }

        public static ParametrizacionServiceReference.Service1Client GetParametrizacionClient()
        {
            try
            {
                var uri = appConfiguration.ParametrizacionUri;
                var client = new ParametrizacionServiceReference.Service1Client();
                var endpoint = new EndpointAddress(uri);
                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetParametrizacionClient", ex);
            }
        }
    }
}
