using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolidariaParametrizacion.Common;
using System;
using System.Threading.Tasks;

namespace SolidariaParametrizacion.Test
{
    [TestClass]
    public class DatosParametrizacionReaderTest
    {
        [TestMethod]
        public async Task TraerSucursales_Test()
        {
            DatosParametrizacionReader reader = new DatosParametrizacionReader();
            var result = await reader.TraerSucursalesAsync();

            Console.Write(result);
        }
    }
}
