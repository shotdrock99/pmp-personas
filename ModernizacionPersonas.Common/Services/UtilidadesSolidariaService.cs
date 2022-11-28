using System.Linq;
using System.Threading.Tasks;
using UtilidadesSolidariaServiceReference;

namespace ModernizacionPersonas.Common
{
    public class UtilidadesSolidariaService
    {
        WCFSolidariaUtilidadesClient clientService;
        public UtilidadesSolidariaService()
        {
            clientService = ServiceConnectionFactory.GetUtilidadesSolidariaClient();
        }

        public async Task<Resultado> SendEmailAsync(Correo email)
        {
            var result = await clientService.EnvioCorreoAsync(email);
            return result.EnvioCorreoResult;
        }

        public async Task<string> FetchFuncionarioByTokenAsync(string token)
        {
            var result = await clientService.UsuarioTokenAsync(token, 2, 0);
            var userName = result.UsuarioTokenResult.Nodes[1].Elements("NewDataSet")
                .Select(x => x.Element("Table"))
                .Select(x => x.Element("codigoUsuario")).FirstOrDefault().Value;
            return userName;
        }
    }
}