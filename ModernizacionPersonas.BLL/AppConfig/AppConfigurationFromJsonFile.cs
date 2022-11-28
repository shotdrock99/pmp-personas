using Microsoft.Extensions.Configuration;
using System.IO;

namespace ModernizacionPersonas.BLL.AppConfig
{
    class AppConfigurationFromJsonFile
    {
        private IConfigurationRoot ConfigurationRoot;
        public string NameEnvironemnt { get; set; }

        public AppConfigurationFromJsonFile()
        {
            this.InitConfigurationRoot();
            this.InitSettings();
        }

        /// <summary>
        /// Gets the appsettings file and saves it the configurations in the atribut ConfigurationRoot
        /// </summary>
        private void InitConfigurationRoot()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            string configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(configFilePath, false);
            this.ConfigurationRoot = configurationBuilder.Build();
        }

        /// <summary>
        /// Initialize all variables that store configuration data
        /// </summary>
        private void InitSettings()
        {
            this.InitMainSettings();
        }

        /// <summary>
        /// Initialize the main settings variables
        /// </summary>
        private void InitMainSettings()
        {
            this.NameEnvironemnt = this.ConfigurationRoot["NameEnvironemnt"];
        }

        /// <summary>
        /// Checks if the environemnt is production
        /// </summary>
        /// <returns> True if the environemnt is production </returns>
        public bool IsProduction()
        {
            return this.NameEnvironemnt == "Production";
        }
    }
}
