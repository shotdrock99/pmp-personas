using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class PdfCartaAceptacionService
    {
        private readonly IDatosPersonasReader peronasReader;
        private readonly DatosParametrizacionReader datosParametrizacionPersonasReader;
        private readonly TomadorSlip tomador;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private readonly InformacionNegocio informacionNegocio;
        private readonly SoligesproDataUsuariosReader soligespro;
        private readonly ConfiguracionSlipDataProvider ConfiguracionSlipDataProvider;
        private string basePath = "";


        public PdfCartaAceptacionService(InformacionNegocio informacionNegocio, TomadorSlip tomador)
        {
            this.peronasReader = new InformacionPersonasReader();
            this.datosParametrizacionPersonasReader = new DatosParametrizacionReader();
            this.tomador = tomador;
            this.informacionNegocio = informacionNegocio;
            this.soligespro = new SoligesproDataUsuariosReader();
            this.ConfiguracionSlipDataProvider = new ConfiguracionSlipDataProvider();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";

        }

        public async Task CreatePdfAsync(List<Asegurado> asegurados)
        {
            if (asegurados.Count > 0)
            {
                var pdf = new PdfDocument();
                foreach (var asegurado in asegurados)
                {
                    // Create a MigraDoc document
                    await CreatePageAsync(pdf, asegurado, this.informacionNegocio.CodigoCotizacion);
                }

                // Save the document...
                var directoryPath = $@"{this.basePath}\{this.informacionNegocio.CodigoCotizacion}";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var path = $@"{this.basePath}\{this.informacionNegocio.CodigoCotizacion}\{this.informacionNegocio.NumeroCotizacion}_CartaAceptacion.pdf";
                pdf.Save(path);
            }
        }

        private async Task CreatePageAsync(PdfDocument pdf, Asegurado asegurado, int codigoCotizacion)
        {
            var codigoRamo = informacionNegocio.CodigoRamo;
            var ramo = await this.peronasReader.TraerRamoAsync(codigoRamo);
            var sucursal = await this.datosParametrizacionPersonasReader.TraerSucursalAsync(this.informacionNegocio.CodigoSucursal);
            var gerente = await this.soligespro.GetUserGerenteAsync(this.informacionNegocio.CodigoSucursal);
            var agencia = sucursal.NombreSucursal;
            var variablesSlip = await this.ConfiguracionSlipDataProvider.ObtenerVariablesSlipAsync(codigoCotizacion, codigoRamo, this.informacionNegocio.CodigoSector, informacionNegocio.CodigoSubramo);
            var ciudad = await this.datosParametrizacionPersonasReader.TraerMunicipioAsync(this.tomador.CodigoDepartamento, this.tomador.CodigoCiudad);
            var nombreCliente = $"{asegurado.PrimerNombre} {asegurado.SegundoNombre} {asegurado.PrimerApellido} {asegurado.SegundoApellido}";
            var representanteLegal = this.tomador.Nombre;

            //traer la ciudad de las variables del Slip
            var ciudadSrt = "";
            foreach (var i in variablesSlip)
            {

                if (i.Nombre == "Ciudad Poliza" && i.Valor != "")
                {
                    ciudadSrt = i.Valor;
                }
            }
            if (ciudadSrt == "")
            {
                ciudadSrt = ciudad.NombreMunicipio;
            }

            var page = pdf.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 13, XFontStyle.Bold);

            // gfx.DrawString("", font, XBrushes.Black, new XRect(100, 100, page.Width - 200, 300), XStringFormats.Center);

            var document = new Document();
            document.AddSection();
            var paragraph = document.LastSection.AddParagraph();

            CreateParagraph(paragraph, $"Agencia: {this.informacionNegocio.CodigoSucursal}");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, $"{ciudadSrt}, {DateTime.Now.ToString("d MMMM yyyy")}");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Señores");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, $"{representanteLegal}");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Gerente");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Ciudad");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Referencia: Respuesta de vinculación.");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Respetados señores:");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraphJustify(paragraph, "Nos permitimos informarle que de acuerdo con las políticas internas de nuestra  compañía y teniendo en cuenta que las disposiciones legales " +
"que regulan la actividad de las compañías de seguros que operan en Colombia consagran como regla general la facultad de estas entidades de seleccionar y asumir," +
"en forma autónoma, los riesgos objeto de aseguramiento, con excepción de aquellos cubiertos a través de los seguros obligatorios establecidos por la ley, " +
" tal como lo dispone el artículo 191 del Estatuto Orgánico del Sistema Financiero “Solamente por ley podrán crearse seguros obligatorios”. "+
"En efecto, el artículo 1056 del Código de Comercio, disposición de carácter general vigente desde 1971 año en que se expidió el Decreto 410, " +
"establece que “Con las restricciones legales, el asegurador podrá, a su arbitrio, asumir  todos o algunos de los riesgos a que estén expuestos el interés o la cosa asegurados," +
"el patrimonio o la persona del asegurado”. El cual se traduce no en otra cosa sino en el principio de la autonomía de la voluntad en materia contractual;" +
$"nos abstenemos de otorgar la cobertura de seguro  para el producto {ramo.NombreAbreviado} referente al cliente {nombreCliente}" +
$"identificado con cédula de ciudadanía número {asegurado.NumeroDocumento}.");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraphJustify(paragraph, "Así mismo, es pertinente indicar que cuando una compañía de seguros no asuma los riesgos que el usuario requiera, " +
"este tiene la oportunidad de acudir a las demás entidades aseguradoras autorizadas para explotar el respectivo ramo.");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Atentamente,");
            paragraph.AddLineBreak();
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, $"{gerente.NombreUsuario}");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, "Gerente");
            paragraph.AddLineBreak();
            CreateParagraph(paragraph, $"Agencia: {agencia}");


            var docRenderer = new DocumentRenderer(document);
            docRenderer.PrepareDocument();
            // Render the paragraph. You can render tables or shapes the same way.
            docRenderer.RenderObject(gfx, XUnit.FromCentimeter(2), XUnit.FromCentimeter(5), "15cm", paragraph);
        }

        private void CreateParagraph(Paragraph paragraph, string text, ParagraphAlignment paragraphAlignment = ParagraphAlignment.Left)
        {
            paragraph.Format.Alignment = paragraphAlignment;
            paragraph.AddText(text);
        }
        private void CreateParagraphJustify(Paragraph paragraph, string text, ParagraphAlignment paragraphAlignment = ParagraphAlignment.Justify)
        {
            paragraph.Format.Alignment = paragraphAlignment;
            paragraph.AddText(text);
        }

        private Paragraph CreateParagraph(Section section, string text)
        {
            var paragraph = section.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Justify;
            paragraph.AddText(text);

            return paragraph;
        }

        public Document CreateDocument()
        {
            // Create a new MigraDoc document
            var document = new Document();
            document.Info.Title = "Hello, MigraDoc";
            document.Info.Subject = $"Demonstrates an excerpt of the capabilities of MigraDoc.";
            document.Info.Author = "Stefan Lange";

            DefineStyles(document);

            DefineContentSection(document);

            DefineParagraphs(document);
            DefineTables(document);
            DefineCharts(document);

            return document;
        }


        /// <summary>
        /// Defines the styles used in the document.
        /// </summary>
        public void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Times New Roman";

            // Heading1 to Heading9 are predefined styles with an outline level. An outline level
            // other than OutlineLevel.BodyText automatically creates the outline (or bookmarks) 
            // in PDF.

            style = document.Styles["Heading1"];
            style.Font.Name = "Tahoma";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Font.Color = Colors.DarkBlue;
            style.ParagraphFormat.PageBreakBefore = true;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading2"];
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.ParagraphFormat.PageBreakBefore = false;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 6;

            style = document.Styles["Heading3"];
            style.Font.Size = 10;
            style.Font.Bold = true;
            style.Font.Italic = true;
            style.ParagraphFormat.SpaceBefore = 6;
            style.ParagraphFormat.SpaceAfter = 3;

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called TextBox based on style Normal
            style = document.Styles.AddStyle("TextBox", "Normal");
            style.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
            style.ParagraphFormat.Borders.Width = 2.5;
            style.ParagraphFormat.Borders.Distance = "3pt";
            style.ParagraphFormat.Shading.Color = Colors.SkyBlue;

            // Create a new style called TOC based on style Normal
            style = document.Styles.AddStyle("TOC", "Normal");
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right, TabLeader.Dots);
            style.ParagraphFormat.Font.Color = Colors.Blue;
        }

        /// <summary>
        /// Defines page setup, headers, and footers.
        /// </summary>
        void DefineContentSection(Document document)
        {
            Section section = document.AddSection();
            section.PageSetup.OddAndEvenPagesHeaderFooter = true;
            section.PageSetup.StartingNumber = 1;

            HeaderFooter header = section.Headers.Primary;
            header.AddParagraph("\tOdd Page Header");

            header = section.Headers.EvenPage;
            header.AddParagraph("Even Page Header");

            // Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph paragraph = new Paragraph();
            paragraph.AddTab();
            paragraph.AddPageField();

            // Add paragraph to footer for odd pages.
            section.Footers.Primary.Add(paragraph);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            section.Footers.EvenPage.Add(paragraph.Clone());
        }

        public void DefineParagraphs(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Paragraph Layout Overview", "Heading1");
            paragraph.AddBookmark("Paragraphs");

            DemonstrateFormattedText(document);
        }

        void DemonstrateAlignment(Document document)
        {
            document.LastSection.AddParagraph("Alignment", "Heading2");

            document.LastSection.AddParagraph("Left Aligned", "Heading3");

            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Left;
            paragraph.AddText("");

            document.LastSection.AddParagraph("Right Aligned", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Right;
            paragraph.AddText("");

            document.LastSection.AddParagraph("Centered", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("");

            document.LastSection.AddParagraph("Justified", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Alignment = ParagraphAlignment.Justify;
            paragraph.AddText("");
        }

        void DemonstrateIndent(Document document)
        {
            document.LastSection.AddParagraph("Indent", "Heading2");

            document.LastSection.AddParagraph("Left Indent", "Heading3");

            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.Format.LeftIndent = "2cm";
            paragraph.AddText("");

            document.LastSection.AddParagraph("Right Indent", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.RightIndent = "1in";
            paragraph.AddText("");

            document.LastSection.AddParagraph("First Line Indent", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.FirstLineIndent = "12mm";
            paragraph.AddText("");

            document.LastSection.AddParagraph("First Line Negative Indent", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.LeftIndent = "1.5cm";
            paragraph.Format.FirstLineIndent = "-1.5cm";
            paragraph.AddText("");
        }

        void DemonstrateFormattedText(Document document)
        {
            document.LastSection.AddParagraph("Formatted Text", "Heading2");

            //document.LastSection.AddParagraph("Left Aligned", "Heading3");

            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.AddText("Text can be formatted ");
            paragraph.AddFormattedText("bold", TextFormat.Bold);
            paragraph.AddText(", ");
            paragraph.AddFormattedText("italic", TextFormat.Italic);
            paragraph.AddText(", or ");
            paragraph.AddFormattedText("bold & italic", TextFormat.Bold | TextFormat.Italic);
            paragraph.AddText(".");
            paragraph.AddLineBreak();
            paragraph.AddText("You can set the ");
            FormattedText formattedText = paragraph.AddFormattedText("size ");
            formattedText.Size = 15;
            paragraph.AddText("the ");
            formattedText = paragraph.AddFormattedText("color ");
            formattedText.Color = Colors.Firebrick;
            paragraph.AddText("the ");
            formattedText = paragraph.AddFormattedText("font", new Font("Verdana"));
            paragraph.AddText(".");
            paragraph.AddLineBreak();
            paragraph.AddText("You can set the ");
            formattedText = paragraph.AddFormattedText("subscript");
            formattedText.Subscript = true;
            paragraph.AddText(" or ");
            formattedText = paragraph.AddFormattedText("superscript");
            formattedText.Superscript = true;
            paragraph.AddText(".");
        }

        void DemonstrateBordersAndShading(Document document)
        {
            document.LastSection.AddPageBreak();
            document.LastSection.AddParagraph("Borders and Shading", "Heading2");

            document.LastSection.AddParagraph("Border around Paragraph", "Heading3");

            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Borders.Width = 2.5;
            paragraph.Format.Borders.Color = Colors.Navy;
            paragraph.Format.Borders.Distance = 3;
            paragraph.AddText("");

            document.LastSection.AddParagraph("Shading", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Shading.Color = Colors.LightCoral;
            paragraph.AddText("");

            document.LastSection.AddParagraph("Borders & Shading", "Heading3");

            paragraph = document.LastSection.AddParagraph();
            paragraph.Style = "TextBox";
            paragraph.AddText("");
        }

        public void DefineTables(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Table Overview", "Heading1");
            paragraph.AddBookmark("Tables");

            DemonstrateSimpleTable(document);
        }

        public void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph("Simple Tables", "Heading2");

            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph("");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph("");

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

        public void DemonstrateCellMerge(Document document)
        {
            document.LastSection.AddParagraph("Cell Merge", "Heading2");

            Table table = document.LastSection.AddTable();
            table.Borders.Visible = true;
            table.TopPadding = 5;
            table.BottomPadding = 5;

            Column column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Left;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Right;

            table.Rows.Height = 35;

            Row row = table.AddRow();
            row.Cells[0].AddParagraph("Merge Right");
            row.Cells[0].MergeRight = 1;

            row = table.AddRow();
            row.VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].AddParagraph("Merge Down");

            table.AddRow();
        }

        public void DefineCharts(Document document)
        {
            Paragraph paragraph = document.LastSection.AddParagraph("Chart Overview", "Heading1");
            paragraph.AddBookmark("Charts");

            document.LastSection.AddParagraph("Sample Chart", "Heading2");

            Chart chart = new Chart();
            chart.Left = 0;

            chart.Width = Unit.FromCentimeter(16);
            chart.Height = Unit.FromCentimeter(12);
            Series series = chart.SeriesCollection.AddSeries();
            series.ChartType = ChartType.Column2D;
            series.Add(new double[] { 1, 17, 45, 5, 3, 20, 11, 23, 8, 19 });
            series.HasDataLabel = true;

            series = chart.SeriesCollection.AddSeries();
            series.ChartType = ChartType.Line;
            series.Add(new double[] { 41, 7, 5, 45, 13, 10, 21, 13, 18, 9 });

            XSeries xseries = chart.XValues.AddXSeries();
            xseries.Add("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N");

            chart.XAxis.MajorTickMark = TickMarkType.Outside;
            chart.XAxis.Title.Caption = "X-Axis";

            chart.YAxis.MajorTickMark = TickMarkType.Outside;
            chart.YAxis.HasMajorGridlines = true;

            chart.PlotArea.LineFormat.Color = Colors.DarkGray;
            chart.PlotArea.LineFormat.Width = 1;

            document.LastSection.Add(chart);
        }

    }
}
