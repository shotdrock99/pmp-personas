using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Web.Configuration
{
    public class AppConfiguration
    {
        public string ProxyURL { get; } = string.Empty;
        public string ApiBaseURL { get; } = string.Empty;

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            var proxyUrlSection = this.GetSection(root, "ProxyURL");
            var apiUrlSection = this.GetSection(root, "ApiURL");

            this.ProxyURL = proxyUrlSection.Value;
            this.ApiBaseURL = apiUrlSection.Value;
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
