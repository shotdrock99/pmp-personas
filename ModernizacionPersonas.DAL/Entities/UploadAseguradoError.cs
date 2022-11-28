using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.DAL.Entities
{
    public class UploadAseguradoError
    {
        public Asegurado Asegurado { get; set; }

        public List<string> Errors { get; set; }
    }
}
