using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernizacionPersonas.BLL.Services;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task PdfSlip_TestMethod()
        {
            var codigoCotizacion = 154;
            var provider = new SlipDataProvider();
            var response = await provider.GenerateSlipAsync(codigoCotizacion, 0, "");

            var pdfBuilderService = new PDFSlipBuilderService(codigoCotizacion, response.Data);
            pdfBuilderService.CreatePdf();
        }
    }
}
