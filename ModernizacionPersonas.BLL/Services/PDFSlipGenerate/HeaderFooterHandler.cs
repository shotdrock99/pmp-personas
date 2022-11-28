using iTextSharp.text;
using iTextSharp.text.pdf;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static ModernizacionPersonas.BLL.Services.PDFSlipGenerate.ImageBuilder;

namespace ModernizacionPersonas.BLL.Services.PDFSlipGenerate
{
    public class HeaderFooterHandler : PdfPageEventHelper
    {
        private readonly int codigoRamo;
        public HeaderFooterHandler(int codigoRamo)
        {
            this.codigoRamo = codigoRamo;
        }
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            var tHeader = new PdfPTable(1)
            {
                TotalWidth = document.PageSize.Width
            };

            var headerImage = new ImageBuilder().GetHeaderImage(codigoRamo);
            var cHeader = new PdfPCell(headerImage, true);
            cHeader.FixedHeight = document.TopMargin;
            cHeader.Border = PdfPCell.NO_BORDER;
            tHeader.AddCell(cHeader);

            tHeader.WriteSelectedRows(0, -1, 0, document.PageSize.Height, writer.DirectContent);

            var tFooter = new PdfPTable(1)
            {
                TotalWidth = document.PageSize.Width
            };

            var footerImage = new ImageBuilder().GetFooterImage();
            var cFooter = new PdfPCell(footerImage, true);
            cFooter.FixedHeight = document.TopMargin;
            cFooter.Border = PdfPCell.NO_BORDER;
            tFooter.AddCell(cFooter);

            tFooter.WriteSelectedRows(0, -1, 0, 49, writer.DirectContent);
        }
    }
}
