using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.BLL.Entities
{
    public class CanOpenCotizacionResponse
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public bool IsValid { get; set; }
    }
}
