using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ModernizacionPersonas.BLL.Services
{
    public static class HtmlTemplateBuilder
    {
        public static string BuildHtmlTemplate(dynamic data, string templateName)
        {
            var resourceName = $"ModernizacionPersonas.BLL.Templates.{templateName}";
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string htmlTemplate = reader.ReadToEnd();
                var htmlBody = EmailTemplateBuilder.Build(htmlTemplate, data);

                return htmlBody;
            }
        }
    }
}
