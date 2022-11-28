using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SolidariaParametrizacion.Common.Configuration
{
    public class AppConfiguration
    {
        public string ProxyURL { get; } = string.Empty;
        public string ConnectionString { get; } = string.Empty;
        public string SISEConnectionString { get; } = string.Empty;
        public string AdministracionPersonasUri { get; } = string.Empty;
        public string ParametrizacionPersonasUri { get; } = string.Empty;
        public string ParametrizacionUri { get; } = string.Empty;

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            var proxyUrlSection = root.GetSection("ApplicationConfig:ProxyURL");
            this.ProxyURL = proxyUrlSection.Value;

            // Connection string 
            var connectionStrings = root.GetSection("ApplicationConfig:ConnectionString");
            if (connectionStrings == null)
            {
                throw new Exception("No existe una sección \"ConnectionString\" en el archivo de configuración.");
            }

            ConnectionString = connectionStrings.GetSection("DataConnection").Value;
            SISEConnectionString = connectionStrings.GetSection("SISEDataConnectionString").Value;

            // Services urls
            var services = root.GetSection("ApplicationConfig:Services");
            if (services == null)
            {
                throw new Exception("No existe una sección \"Services\" en el archivo de configuración.");
            }

            AdministracionPersonasUri = services.GetSection("AdministracionPersonasUri").Value;
            ParametrizacionPersonasUri = services.GetSection("ParametrizacionPersonasUri").Value;
            ParametrizacionUri = services.GetSection("ParametrizacionUri").Value;
        }

        private IConfigurationSection GetSection(IConfigurationRoot root, string sectionName)
        {
            var section = root.GetSection(sectionName);
            if (section == null)
            {
                throw new Exception("No existe una sección \"ProxyURL\" en el archivo de configuración.");
            }

            return section;
        }
    }
}
