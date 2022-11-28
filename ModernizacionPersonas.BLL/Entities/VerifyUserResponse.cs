using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.Api.Entities
{
    public class VerifyUserResponse
    {
        public string message { get; set; }
        public bool isException { get; set; }
        public string exception { get; set; }
        public respon response { get; set; }
    }
    public class responseTomadorInt
    {
        public string Respuesta { get; set; }
        public string estado { get; set; }
    }
    public class VerifyUserResponseTomador
    {
        public string message { get; set; }
        public bool isException { get; set; }
        public string exception { get; set; }
        public string response { get; set; }
    }

    public class respon
    {
        public int p_in_id { get; set; }
        public string p_st_UserName { get; set; }
        public string p_st_Password { get; set; }
        public string p_st_Token { get; set; }
    }
    public class dataTomador
    {
        public int documentype { get; set; }
        public string documentNumber { get; set; }
    }
    public class credentials
    {
        public string P_st_UserName { get; set; }
        public string P_st_Password { get; set; }
    }
}
