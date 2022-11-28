using iTextSharp.text;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ModernizacionPersonas.BLL.Services.PDFSlipGenerate
{
    public class ImageBuilder
    {
        public Image GetHeaderImage(int codigoRamo)
        {
            string path = string.Empty;
            switch (codigoRamo)
            {
                case 1:
                    path = Path.GetFullPath("images/header_acc_personales.png");
                    break;
                // ACC. PERSONALES
                case 37:
                    path = Path.GetFullPath("images/header_acc_personales.png");
                    break;
                // ACC. ESCOLARES
                case 2:
                    path = Path.GetFullPath("images/header_acc_escolares.png");
                    break;
                // VIDA GRUPO
                case 15:
                    path = Path.GetFullPath("images/header_vidagrupo.png");
                    break;
                // VIDA GRUPO DEUDORES
                case 16:
                    path = Path.GetFullPath("images/header_vidagrupo_deudores.png");
                    break;
            }
            return Image.GetInstance(path);
        }

        public Image GetFooterImage()
        {
            string path = Path.GetFullPath("images/footer_acc_escolares.png");
            return Image.GetInstance(path);
        }
    }
}
