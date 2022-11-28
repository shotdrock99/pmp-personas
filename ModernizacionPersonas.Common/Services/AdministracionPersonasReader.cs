using AdministracionPersonasServiceReference;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModernizacionPersonas.Common
{
    public class AdministracionPersonasReader
    {
        WCFAdministracionPersonasClient clientService;

        public AdministracionPersonasReader()
        {
            clientService = ServiceConnectionFactory.GetAdministracionPersonasClient();
        }

        public async Task<Persona> TraerIntermediarioAsync(int codigoIntermediario)
        {
            var args = GetTraerIntermediarioArgs(codigoIntermediario);
            var result = await clientService.TraerPersonaAsync(args.ToString());
            return result.Body.TraerPersonaResult;
        }

        public async Task<Persona> TraerTomadorAsync(int codigoTipoDocumento, string numeroDocumento)
        {
            var args = GetTraerTomadorArgs(codigoTipoDocumento, numeroDocumento);
            var result = await clientService.TraerPersonaAsync(args.ToString());
            return result.Body.TraerPersonaResult;
        }

        
        private XDocument GetTraerTomadorArgs(int codigoTipoDocumento, string numeroDocumento)
        {
            XDocument result =
                new XDocument(
                    new XElement("Solicitud",
                    new XElement("Opcion",
                    new XElement("Param",
                        new XAttribute("Nombre", "TipoOpcion"),
                        new XAttribute("Valor", "TraerAseguradoxDOC"),
                        new XAttribute("Tipo", "A"))),
                    new XElement("Parametros",
                    new XElement("Param",
                        new XAttribute("nombre", "tipo_doc"),
                        new XAttribute("valor", $"{codigoTipoDocumento}")),
                    new XElement("Param",
                        new XAttribute("nombre", "nro_doc"),
                        new XAttribute("valor", $"{numeroDocumento}"))
                    )));

                return result;
        }

        private XDocument GetTraerIntermediarioArgs(int codigoIntermediario)
        {
            XDocument result =
                new XDocument(
                    new XElement("Solicitud",
                    new XElement("Opcion",
                    new XElement("Param",
                        new XAttribute("Nombre", "TipoOpcion"),
                        new XAttribute("Valor", "TraerAseguradoxCOD"),
                        new XAttribute("Tipo", "I"))),
                    new XElement("Parametros",
                    new XElement("Param",
                        new XAttribute("nombre", "codigo"),
                        new XAttribute("valor", $"{codigoIntermediario}"))
                    )));

            return result;
        }
    }
}
