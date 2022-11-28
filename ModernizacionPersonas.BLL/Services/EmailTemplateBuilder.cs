using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Services
{
    public class EmailTemplateBuilder
    {
        public static string Build(string htmlTemplate, dynamic data)
        {
            var template = Handlebars.Compile(htmlTemplate);
            var result = template(data);

            return result;
        }
    }
}
