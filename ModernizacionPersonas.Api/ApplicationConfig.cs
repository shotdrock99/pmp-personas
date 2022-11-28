using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api
{
    public class ApplicationConfig
    {
        public string ProxyURL { get; set; }
        public ConnectionStrings ConnectionString { get; set; }
        public Services Services { get; set; }
    }

    public class ConnectionStrings
    {
        public string DataConnection { get; set; }
        public string SISEDataConnectionString { get; set; }
    }

    public class Services
    {
        public string AdministracionPersonasUri { get; set; }
        public string ParametrizacionPersonasUri { get; set; }
        public string ParametrizacionUri { get; set; }
    }
}
