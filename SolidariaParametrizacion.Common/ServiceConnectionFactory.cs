using ParametrizacionServiceReference;
using SolidariaParametrizacion.Common.Configuration;
using System;
using System.ServiceModel;

namespace SolidariaParametrizacion.Common
{
    public class ServiceConnectionFactory
    {
        private static readonly BasicHttpBinding basicBinding = new BasicHttpBinding();
        private static AppConfiguration appConfiguration = new AppConfiguration();

        public static Service1Client GetParametrizacionPersonasClient()
        {
            try
            {
                var uri = appConfiguration.ParametrizacionUri;
                var client = new Service1Client();
                var endpoint = new EndpointAddress(uri);
                //var channelFactory = new ChannelFactory<IWCFAdministracionPersonas>(basicBinding, endpoint);

                client.Endpoint.Address = endpoint;
                return client;
            }
            catch (Exception ex)
            {
                throw new Exception("ServiceConfigurationFactory :: GetParametrizacionPersonasClient", ex);
            }
        }
    }
}
