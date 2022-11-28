using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services.PDFSlipGenerate
{
    public class PDFSlipBuilderService
    {
        private readonly Slip slip;
        private readonly string numeroCotizacion;
        private readonly int codigoCotizacion;
        private readonly int version;
        private readonly Font arial;
        private readonly Font arialBold;
        private readonly Font arialTable;
        private readonly Font arialBoldTable;
        private readonly IndemnizacionesData indemnizacionesData;
        private readonly stringTable stringTable;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private List<SlipVariable> slipVariables; 

        public PDFSlipBuilderService(Slip slip, int codigoCotizacion, string numeroCotizacion, int version, List<SlipVariable> slipVariables)
        {
            this.slip = slip;
            this.slipVariables = slipVariables;
            this.codigoCotizacion = codigoCotizacion;
            this.numeroCotizacion = numeroCotizacion;
            this.version = version;
            this.arial = FontFactory.GetFont("Arial", 8, Font.NORMAL);
            this.arialBold = FontFactory.GetFont("Arial", 8, Font.BOLD);
            this.arialTable = FontFactory.GetFont("Arial", 7, Font.NORMAL);
            this.arialBoldTable = FontFactory.GetFont("Arial", 7, Font.BOLD);
            this.indemnizacionesData = new IndemnizacionesData();
            this.stringTable = new stringTable();
            this.AppConfig = new AppConfigurationFromJsonFile();
        }

        public async Task<byte[]> GenerateSlipDF()
        {
            var document = new Document(PageSize.A4, 71f, 71f, 96f, 71f);
            var ms = new MemoryStream();
            //var fs = new FileStream($@"\\FILESERVERIBM\PMP_Repositorio\{ this.codigoCotizacion}\SLIP.pdf", FileMode.Create);
            var writer = PdfWriter.GetInstance(document, ms);

            document.AddTitle($"Slip_Cotización_{numeroCotizacion}_Versión_{version}");
            document.AddAuthor("Aseguradora Solidaria de Colombia Entidad Cooperativa");
            document.AddCreator("Personas");
            document.Open();

            //Set Header and Footer on every single page
            writer.PageEvent = new HeaderFooterHandler(slip.CodigoRamo);

            //General Information
            await this.GeneralInformationSection(document);

            //Secciones
            await this.SeccionesSection(document);

            //Amparos
            await this.AmparosSection(document, 1);

            //Grupos Asegurados
            await this.GruposAseguradosSection(document);

            //Claúsulas
            if (slip.Clausulas.Clausulas.Count > 0)
            {
                await this.ClausulasSection(document);
            }

            //Condiciones
            if (slip.Condiciones.Condiciones.Length > 0)
            {
                await this.CondicionesSection(document);
            }


            //Disposiciones Finales
            if (slip.Disposiciones.Disposiciones.Count > 0)
            {
                await this.DisposicionesFinalesSection(document);
            }

            //Add Tablas Indemnizaciones
            await this.AmparosSection(document, 2);

            document.Close();

            //Add watermark to each page
            byte[] documentBytes = ms.ToArray();
            if (!this.AppConfig.IsProduction())
            {
                documentBytes = AddWatermark(documentBytes, BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED), "Borrador"); ;
            }
            return documentBytes;
        }

        public async Task GeneralInformationSection(Document document)
        {
            DateTime today = DateTime.Today;
            var ciudadSrt = "";
            foreach (var i in slipVariables)
            {
               
                if (i.Nombre == "Ciudad Poliza" && i.Valor != "")
                {
                    ciudadSrt = i.Valor;
                }
            }
            if (ciudadSrt == "")
            {
                ciudadSrt = slip.Ciudad;
            }
            //Saludo inicial
            document.Add(new Paragraph($" {ciudadSrt}, {today.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-CO"))}", arial));
            document.Add(new Phrase("\n"));

            //Señores
            document.Add(new Paragraph("Señores", arialBold));

            //Nombre señores
            document.Add(new Paragraph(slip.Tomador.Nombre, arial));

            //Dirección
            var direccion = new Paragraph();
            direccion.Add(new Chunk("Dirección: ", arialBold));
            direccion.Add(new Chunk(slip.Tomador.Direccion, arial));
            document.Add(direccion);

            //Teléfono
            var tel = new Paragraph();
            tel.Add(new Chunk("Teléfono: ", arialBold));
            tel.Add(new Chunk(slip.Tomador.Telefono, arial));
            document.Add(tel);
            document.Add(new Phrase("\n"));

            //Asunto
            var asunto = new Paragraph();
            asunto.Add(new Chunk("ASUNTO: ", arialBold));
            asunto.Add(new Chunk("COTIZACIÓN ", arial));
            var numCot = int.Parse(this.numeroCotizacion);
            asunto.Add(new Chunk(numCot + " VR. " + this.version, arialBold));
            var slipAsunto = slip.Asunto.Replace("COTIZACIÓN", "");
            asunto.Add(new Chunk(slipAsunto, arial));
            document.Add(asunto);

            //RS
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph("Respetados Señores", arial));
            document.Add(new Phrase("\n"));

            //TP
            var tPoliza = new Paragraph()
            {
                Alignment = Element.ALIGN_JUSTIFIED,
                Leading = 10f
            };
            var fp = new Phrase("Aseguradora Solidaria de Colombia se permite presentar los términos y condiciones de la póliza ", arial);
            fp.Add(new Chunk(slip.TipoPoliza, arialBold));
            if (slip.Vigencia.Desde.ToString() == "01/01/0001 12:00:00" || slip.Vigencia.Desde.ToString() == "01/01/0001 12:00:00 AM" || slip.Vigencia.Desde.ToString() == "01/01/0001 12:00:00 a.m.")
            {
                fp.Add(new Phrase(", con vigencia por definir. "));
            }
            else
            {
                fp.Add(new Phrase(" para la vigencia comprendida entre el "));
                fp.Add(new Chunk(slip.Vigencia.Desde?.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-CO"))));
                fp.Add(new Chunk(" hasta el "));
                fp.Add(new Chunk(slip.Vigencia.Hasta?.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture("es-CO"))));

            }
            tPoliza.Add(fp);
            document.Add(tPoliza);

            //Tomador
            document.Add(new Phrase("\n"));
            var tomador = new Paragraph();
            tomador.Add(new Chunk("TOMADOR: ", arialBold));
            tomador.Add(new Chunk(slip.Tomador.Nombre, arial));
            document.Add(tomador);
            var id = new Paragraph();
            var tipoDoc = slip.Tomador.CodigoTipoDocumento == 1 ? "CC" : "NIT";
            id.Add(new Chunk($"IDENTIFICACIÓN TOMADOR ({tipoDoc}): ", arialBold));
            id.Add(new Chunk(slip.Tomador.NumeroDocumento, arial));
            document.Add(id);

            //Actividad
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph("ACTIVIDAD: ", arialBold));
            document.Add(new Paragraph(slip.Tomador.Actividad, arial));

        }

        public async Task SeccionesSection(Document document)
        {
            foreach (var seccion in slip.InfoGeneral.InfoGeneral)
            {
                document.Add(new Phrase("\n"));
                document.Add(new Paragraph(seccion.Seccion, arialBold));
                document.Add(new Paragraph(seccion.Texto, arial)
                {
                    Alignment = Element.ALIGN_JUSTIFIED,
                    Leading = 10f
                });
            }
        }

        public async Task AmparosSection(Document document, int proceso)
        {

            if (proceso == 1)
            {
                document.Add(new Phrase("\n"));
                document.Add(new Paragraph(slip.Amparos.Seccion, arialBold));
                document.Add(new Phrase("\n"));
                document.Add(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, 1));
                foreach (var amparo in slip.Amparos.Amparos)
                {
                    document.Add(new Phrase("\n"));
                    document.Add(new Paragraph(amparo.Seccion, arialBold));
                    document.Add(new Paragraph(amparo.Texto, arial)
                    {
                        Alignment = Element.ALIGN_JUSTIFIED,
                        Leading = 10f
                    });

                }
            }
            else
            {
                var tieneAmp4 = false;
                var tieneAmp51 = false;
                List<TextosSeccionSlip> lstAmparos = new List<TextosSeccionSlip>();
                foreach (var amparo in slip.Amparos.Amparos)
                {
                    if (amparo.CodigoAmparo == "4")
                    {
                        tieneAmp4 = true;
                    }
                    if (amparo.CodigoAmparo == "51")
                    {
                        tieneAmp51 = true;
                    }

                }
                if (tieneAmp4 && tieneAmp51)
                {
                    foreach (var amparo in slip.Amparos.Amparos)
                    {
                        if (amparo.CodigoAmparo != "4")
                        {
                            lstAmparos.Add(amparo);
                        }
                    }
                }
                else
                {
                    foreach (var amparo in slip.Amparos.Amparos)
                    {

                        lstAmparos.Add(amparo);

                    }
                }
                foreach (var amparo in lstAmparos)
                {
                    document.Add(new Phrase("\n"));

                    if (amparo.CodigoAmparo == "51" || amparo.CodigoAmparo == "6" || amparo.CodigoAmparo == "4")
                    {
                        //Tabla de Indemnizaciones
                        await this.IndemnizacionesTable(document, amparo.CodigoAmparo);
                    }
                    if (amparo.CodigoAmparo == "15" && amparo.CodigoSeccion == "29")
                    {
                        this.GastosDeTrasladoTable(document);
                    }

                }
            }

        }
        public void GastosDeTrasladoTable(Document document)
        {
            document.NewPage();
            document.Add(new Paragraph("ANEXO TABLA GASTOS DE TRASLADO", arialBoldTable) { Alignment = Element.ALIGN_CENTER });
            document.Add(new Phrase("\n"));
            var tGastos = new PdfPTable(4)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            //Columns Definition
            var tName = new Paragraph("SERVICIOS GASTOS DE TRASLADO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var nameCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingBottom = 5f
            };
            nameCell.AddElement(tName);
            tGastos.AddCell(nameCell);
            var cellP = new PdfPCell()
            {
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            var tableH = new PdfPTable(24)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            var tHeaderC0 = new Paragraph("EVALUACION DE LA CONDICION DEL ASEGURADO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC0 = new PdfPCell()
            {
                Colspan = 8,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC0.AddElement(tHeaderC0);
            tableH.AddCell(cellC0);

            var tHeaderC1 = new Paragraph("MECANISMO DE TRASLADO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC1 = new PdfPCell()
            {
                Colspan = 8,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC1.AddElement(tHeaderC1);
            tableH.AddCell(cellC1);

            var tHeaderC2 = new Paragraph("EJEMPLO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC2 = new PdfPCell()
            {
                Colspan = 8,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC2.AddElement(tHeaderC2);
            tableH.AddCell(cellC2);

            //Consult and insert data
            var GastosTraslado = stringTable.GetGastosTraslado();
            GastosTraslado.ForEach(i =>
            {
                var cellH0 = new PdfPCell(new Paragraph(i.column1, arialTable) { Alignment = Element.ALIGN_JUSTIFIED })
                {
                    Colspan = 8,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                var cellH1 = new PdfPCell(new Paragraph(i.column2, arialTable) { Alignment = Element.ALIGN_JUSTIFIED })
                {
                    Colspan = 8,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                var cellH2 = new PdfPCell(new Paragraph(i.column3, arialTable) { Alignment = Element.ALIGN_JUSTIFIED })
                {
                    Colspan = 8,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                tableH.AddCell(cellH0);
                tableH.AddCell(cellH1);
                tableH.AddCell(cellH2);
            });
            cellP.AddElement(tableH);
            tGastos.AddCell(cellP);

            //Insert Table to Document
            document.Add(tGastos);

            //Insert text after table
            document.Add(new Phrase("\n"));

        }
        public async Task IndemnizacionesTable(Document document, string codAmparo)
        {
            //document.Add(new Phrase("<div style='page -break-before:always'>&nbsp;</div>"));
            document.NewPage();

            if (codAmparo == "6")
            {
                document.Add(new Paragraph("ANEXO TABLA PORCENTUAL", arialBoldTable) { Alignment = Element.ALIGN_CENTER });

            }
            else if (codAmparo == "4" || codAmparo == "51")
            {
                document.Add(new Paragraph("ANEXO TABLA DESMEMBRACION", arialBoldTable) { Alignment = Element.ALIGN_CENTER });
            }

            document.Add(new Phrase("\n"));
            //Table definition
            var tIndemnizaciones = new PdfPTable(4)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };

            //Columns Definition
            var tName = new Paragraph("TABLA DE INDEMNIZACIONES", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var nameCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingBottom = 5f
            };
            nameCell.AddElement(tName);
            tIndemnizaciones.AddCell(nameCell);

            var cellP = new PdfPCell()
            {
                Colspan = 4,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            var tableH = new PdfPTable(23)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            var tHeaderC0 = new Paragraph("NRO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC0 = new PdfPCell()
            {
                Colspan = 3,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC0.AddElement(tHeaderC0);
            tableH.AddCell(cellC0);

            var tHeaderC1 = new Paragraph("CLASE DE PERDIDA", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC1 = new PdfPCell()
            {
                Colspan = 15,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC1.AddElement(tHeaderC1);
            tableH.AddCell(cellC1);

            var tHeaderC2 = new Paragraph("% DE INDEMNIZACIÓN", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC2 = new PdfPCell()
            {
                Colspan = 5,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC2.AddElement(tHeaderC2);
            tableH.AddCell(cellC2);

            //Consult and insert data
            var indemnizaciones = new List<IndemnizacionesData>();
            var inutilizaciones = new List<IndemnizacionesData>();

            if (codAmparo == "51" || codAmparo == "4")
            {
                indemnizaciones = indemnizacionesData.GetIndemnizaciones();

            }
            else if (codAmparo == "6")
            {
                inutilizaciones = indemnizacionesData.GetInutilizaciones();

            }


            if (indemnizaciones.Count > 0 && inutilizaciones.Count == 0)
            {
                indemnizaciones.ForEach(i =>
                {
                    var cellH0 = new PdfPCell(new Paragraph($"{i.Index}", arialTable) { Alignment = Element.ALIGN_CENTER })
                    {
                        Colspan = 3,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    var cellH1 = new PdfPCell(new Paragraph(i.Descripcion, arialTable) { Alignment = Element.ALIGN_JUSTIFIED })
                    {
                        Colspan = 15,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    var cellH2 = new PdfPCell(new Paragraph($"{i.Valor}%", arialTable) { Alignment = Element.ALIGN_CENTER })
                    {
                        Colspan = 5,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    tableH.AddCell(cellH0);
                    tableH.AddCell(cellH1);
                    tableH.AddCell(cellH2);
                });
            }
            else if (inutilizaciones.Count > 0 && indemnizaciones.Count == 0)
            {
                inutilizaciones.ForEach(i =>
                {
                    var cellH0 = new PdfPCell(new Paragraph($"{i.Index}", arialTable) { Alignment = Element.ALIGN_CENTER })
                    {
                        Colspan = 3,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    var cellH1 = new PdfPCell(new Paragraph(i.Descripcion, arialTable) { Alignment = Element.ALIGN_JUSTIFIED })
                    {
                        Colspan = 15,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    var cellH2 = new PdfPCell(new Paragraph($"{i.Valor}%", arialTable) { Alignment = Element.ALIGN_CENTER })
                    {
                        Colspan = 5,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    tableH.AddCell(cellH0);
                    tableH.AddCell(cellH1);
                    tableH.AddCell(cellH2);
                });
            }

            //Insert Cells into Table
            cellP.AddElement(tableH);
            tIndemnizaciones.AddCell(cellP);

            //Insert Table to Document
            document.Add(tIndemnizaciones);

            //Insert text after table
            document.Add(new Phrase("\n"));
            /* document.Add(new Paragraph(@"Las pérdidas no enunciadas en la tabla anterior, aunque sean de menor impacto, serán indemnizadas en relación con su gravedad, comparándolas con las aquí indicadas.", arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });
             document.Add(new Phrase("\n"));
             document.Add(new Paragraph(@"Cuando a consecuencia de un accidente, se afecten varias desmembraciones o inutilizaciones, estas no se acumularan entre sí, sino que la indemnización se determinara por la mayor de dichas desmembraciones o inutilizaciones.", arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });
             document.Add(new Phrase("\n"));
             document.Add(new Paragraph(@"En caso de constar en la solicitud que el asegurado es zurdo, se invertirán los porcentajes de indemnización fijados por la pérdida de los miembros superiores.", arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });
             document.Add(new Phrase("\n"));
             document.Add(new Paragraph(@"Esta cobertura es excluyente de los amparos de muerte e incapacidad total y permanente otorgados por esta póliza.", arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });*/
        }

        public async Task GruposAseguradosSection(Document document)
        {
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(slip.GruposAsegurados.Seccion, arialBold));
            document.Add(new Phrase("\n"));
            document.Add(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, 1));

            var opciones = 0;
            foreach (var ga in slip.GruposAsegurados.GruposAsegurados)
            {
                document.Add(new Paragraph(ga.Nombre, arialBold));
                foreach (var op in ga.ValoresAmparos)
                {
                    opciones = op.Opciones.Count;
                }
                await this.ValoresAmparosTable(document, ga.ValoresAmparos, ga.snTasaMensual, opciones);
                await this.AseguradosDiariosTable(document, ga.ValoresAmparos, ga.snTasaMensual, opciones);
                if (ga.snTasaMensual)
                {
                    await this.MaximoValoAseguradoIndividualSection(document, ga.valorMaximo);
                }
                await this.EdadesAmparosTable(document, ga.Edades);
            }
        }

        public async Task ValoresAmparosTable(Document document, List<ValoresAseguradosAmparoSlip> valores, bool esTasaMensual, int opciones)
        {
            document.Add(new Paragraph("VALORES ASEGURADOS", arialBoldTable));

            var nameTipoSuma =
                valores[0].CodigoTipoSumaAsegurada == 1
                ? "SUMA FIJA"
                : valores[0].CodigoTipoSumaAsegurada == 2
                ? "MULTIPLO DE SUELDOS"
                : valores[0].CodigoTipoSumaAsegurada == 3
                ? "SUMA VARIABLE POR ASEGURADO"
                : valores[0].CodigoTipoSumaAsegurada == 5
                ? "SUMA UNIFORME MAS MULTIPLO DE SUELDOS"
                : valores[0].CodigoTipoSumaAsegurada == 6
                ? "SALDO DEUDORES"
                : "S.M.M.L.V.";

            if (opciones == 1)
            {
                var tValores1 = new PdfPTable(4)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    SpacingBefore = 5
                };

                //Columns Definition
                var cellP = new PdfPCell()
                {
                    Colspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    PaddingTop = 0,
                    PaddingBottom = 0
                };

                var tableH = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                };

                var tHeaderC = new Paragraph($"{nameTipoSuma}", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 20,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC.AddElement(tHeaderC);
                tableH.AddCell(cellC);

                var tHeaderC1 = new Paragraph("AMPAROS", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
                var cellC1 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 10,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC1.AddElement(tHeaderC1);
                tableH.AddCell(cellC1);

                var tHeaderC2 = new Paragraph("Valor Asegurado Individual", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC2 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = (esTasaMensual) ? 10 : 5,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC2.AddElement(tHeaderC2);
                tableH.AddCell(cellC2);

                if (!esTasaMensual)
                {
                    var tHeaderC3 = new Paragraph("Valor Asegurado", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                    var cellC3 = new PdfPCell()
                    {
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Colspan = 5,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                    };
                    cellC3.AddElement(tHeaderC3);
                    tableH.AddCell(cellC3);
                }


                //Insert data
                valores.ForEach(v =>
                {
                    var cellH1 = new PdfPCell(new Paragraph(v.NombreAmparo == "PRIMA ANUAL TOTAL" && esTasaMensual ? "TASA MENSUAL POR MIL A APLICAR" : v.NombreAmparo, arialTable) { Alignment = Element.ALIGN_LEFT })
                    {
                        Colspan = 10,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    tableH.AddCell(cellH1);
                    v.Opciones.ForEach(o =>
                    {
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 0 && v.NombreAmparo != "PRIMA ANUAL TOTAL")
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "PRIMA ANUAL TOTAL" ? o.ValorAseguradoIndividual.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : esTasaMensual ? $"{o.tasaMensual:#0.##}%" : "")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 0 && v.NombreAmparo == "PRIMA ANUAL TOTAL")
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "PRIMA ANUAL TOTAL" ? o.ValorAseguradoIndividual.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : esTasaMensual ? $"{o.tasaMensual:#0.##}‰" : "")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 1)
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{o.ValorAseguradoIndividual:#0.##}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 2)
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{o.ValorAseguradoIndividual:#0.##}%", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 3)
                        {
                            var cellH2 = new PdfPCell(new Paragraph("", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo == "ASISTENCIA")
                        {
                            var cellH2 = new PdfPCell(new Paragraph("SI", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (!esTasaMensual)
                        {
                            var cellH3 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? o.ValorAsegurado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : "")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH3);
                        }
                    });
                });

                //Insert Cells into Table
                cellP.AddElement(tableH);
                tValores1.AddCell(cellP);

                //Insert Table to Document
                document.Add(tValores1);
            }
            else if (opciones == 3)
            {
                var tValores3 = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    SpacingBefore = 5
                };
                tValores3.DefaultCell.Border = PdfPCell.NO_BORDER;

                //Columns Definition
                var cellP = new PdfPCell()
                {
                    Colspan = 20,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    PaddingTop = 0,
                    PaddingBottom = 0
                };

                var tableH = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                };

                var tHeaderC = new Paragraph($"{nameTipoSuma}", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 20,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC.AddElement(tHeaderC);
                tableH.AddCell(cellC);

                var tHeaderC1 = new Paragraph("AMPAROS", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
                var cellC1 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 8,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC1.AddElement(tHeaderC1);
                tableH.AddCell(cellC1);

                var tHeaderC2 = new Paragraph("Valor Opción 1", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC2 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC2.AddElement(tHeaderC2);
                tableH.AddCell(cellC2);

                var tHeaderC3 = new Paragraph("Valor Opción 2", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC3 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC3.AddElement(tHeaderC3);
                tableH.AddCell(cellC3);

                var tHeaderC4 = new Paragraph("Valor Opción 3", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC4 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC4.AddElement(tHeaderC4);
                tableH.AddCell(cellC4);

                //Insert data
                valores.ForEach(v =>
                {
                    var cellH1 = new PdfPCell(new Paragraph(v.NombreAmparo, arialTable) { Alignment = Element.ALIGN_LEFT })
                    {
                        Colspan = 8,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    tableH.AddCell(cellH1);

                    var i = 0;
                    var cellH2 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorAsegurado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 4,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH2);
                    i++;
                    var cellH3 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorAsegurado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 4,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH3);
                    i++;
                    var cellH4 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorAsegurado.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 4,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH4);
                });

                //Insert Cells into Table
                cellP.AddElement(tableH);
                tValores3.AddCell(cellP);

                //Insert Table to Document
                document.Add(tValores3);
            }
        }
        public async Task AseguradosDiariosTable(Document document, List<ValoresAseguradosAmparoSlip> valores, bool esTasaMensual, int opciones)
        {
            // document.Add(new Paragraph("VALORES ASEGURADOS DIARIOS", arialBoldTable));

            var nameTipoSuma =
                valores[0].CodigoTipoSumaAsegurada == 1
                ? "SUMA FIJA"
                : valores[0].CodigoTipoSumaAsegurada == 2
                ? "MULTIPLO DE SUELDOS"
                : valores[0].CodigoTipoSumaAsegurada == 3
                ? "SUMA VARIABLE POR ASEGURADO"
                : valores[0].CodigoTipoSumaAsegurada == 5
                ? "SUMA UNIFORME MAS MULTIPLO DE SUELDOS"
                : valores[0].CodigoTipoSumaAsegurada == 6
                ? "SALDO DEUDORES"
                : "S.M.M.L.V.";
            List<ValoresAseguradosAmparoSlip> lstValoresDiarios = new List<ValoresAseguradosAmparoSlip>();

            valores.ForEach(v =>
            {
                v.Opciones.ForEach(o =>
                {
                    if (o.TablaValoresDiarios)
                    {
                        if (!lstValoresDiarios.Any(n => n.NombreAmparo == v.NombreAmparo))
                            lstValoresDiarios.Add(v);

                    }
                });
            });
            if (opciones == 1)
            {
                var tValores1 = new PdfPTable(4)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    SpacingBefore = 5
                };

                //Columns Definition
                var cellP = new PdfPCell()
                {
                    Colspan = 4,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    PaddingTop = 0,
                    PaddingBottom = 0
                };

                var tableH = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                };

                var tHeaderC1 = new Paragraph("VALORES ASEGURADOS DIARIOS", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
                var cellC1 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 10,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC1.AddElement(tHeaderC1);
                tableH.AddCell(cellC1);

                var tHeaderC2 = new Paragraph("Número Días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC2 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = (esTasaMensual) ? 10 : 5,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC2.AddElement(tHeaderC2);
                tableH.AddCell(cellC2);


                var tHeaderC3 = new Paragraph("Valor Diario", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC3 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 5,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC3.AddElement(tHeaderC3);
                tableH.AddCell(cellC3);

                

                //Insert data
                lstValoresDiarios.ForEach(v =>
                {
                    var cellH1 = new PdfPCell(new Paragraph(v.NombreAmparo == "PRIMA ANUAL TOTAL" && esTasaMensual ? "TASA MENSUAL POR MIL A APLICAR" : v.NombreAmparo, arialTable) { Alignment = Element.ALIGN_LEFT })
                    {
                        Colspan = 10,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    tableH.AddCell(cellH1);
                    v.Opciones.ForEach(o =>
                    {
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 0)
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "PRIMA ANUAL TOTAL" ? $"{Decimal.ToInt32(o.NumeroDias)}" : esTasaMensual ? $"{o.tasaMensual:#0.##}%" : "")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 1)
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{Decimal.ToInt32(o.NumeroDias)}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 2)
                        {
                            var cellH2 = new PdfPCell(new Paragraph($"{Decimal.ToInt32(o.NumeroDias)}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo != "ASISTENCIA" && o.TipoValor == 3)
                        {
                            var cellH2 = new PdfPCell(new Paragraph("", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (v.NombreAmparo == "ASISTENCIA")
                        {
                            var cellH2 = new PdfPCell(new Paragraph("SI", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = (esTasaMensual) ? 10 : 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH2);
                        }
                        if (!esTasaMensual)
                        {
                            var cellH3 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? o.ValorDiario.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : "")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                            {
                                Colspan = 5,
                                BorderWidth = 0.5f,
                                BorderColor = BaseColor.LightGray,
                                HorizontalAlignment = Element.ALIGN_RIGHT
                            };
                            tableH.AddCell(cellH3);
                        }
                    });
                });

                //Insert Cells into Table
                cellP.AddElement(tableH);
                tValores1.AddCell(cellP);

                //Insert Table to Document
                if(lstValoresDiarios.Count() != 0)
                {
                    document.Add(tValores1);
                }
                
            }
            else if (opciones == 3)
            {
                var tValores3 = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    SpacingBefore = 5
                };
                tValores3.DefaultCell.Border = PdfPCell.NO_BORDER;

                //Columns Definition
                var cellP = new PdfPCell()
                {
                    Colspan = 20,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    PaddingTop = 0,
                    PaddingBottom = 0
                };

                var tableH = new PdfPTable(20)
                {
                    TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                    LockedWidth = true,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                };


                var tHeaderC1 = new Paragraph("VALORES ASEGURADOS DIARIOS", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
                var cellC1 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 8,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC1.AddElement(tHeaderC1);
                tableH.AddCell(cellC1);

                var tHeaderC2 = new Paragraph("Opción 1", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC2 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC2.AddElement(tHeaderC2);
                tableH.AddCell(cellC2);

                var tHeaderC3 = new Paragraph("Opción 2", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC3 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC3.AddElement(tHeaderC3);
                tableH.AddCell(cellC3);

                var tHeaderC4 = new Paragraph("Opción 3", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC4 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC4.AddElement(tHeaderC4);
                tableH.AddCell(cellC4);

                var tHeaderC5 = new Paragraph("", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
                var cellC5 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Colspan = 8,
                    PaddingLeft = 10f,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC5.AddElement(tHeaderC5);
                tableH.AddCell(cellC5);

                var tHeaderC6 = new Paragraph("Número Días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC6 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC6.AddElement(tHeaderC6);
                tableH.AddCell(cellC6);

                var tHeaderC7 = new Paragraph("Valor Diario", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC7 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC7.AddElement(tHeaderC7);
                tableH.AddCell(cellC7);

                var tHeaderC8 = new Paragraph("Número Días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC8 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC8.AddElement(tHeaderC8);
                tableH.AddCell(cellC8);

                var tHeaderC9 = new Paragraph("Valor Diario", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC9 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC9.AddElement(tHeaderC9);
                tableH.AddCell(cellC9);

                var tHeaderC10 = new Paragraph("Número Días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC10 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC10.AddElement(tHeaderC10);
                tableH.AddCell(cellC10);

                var tHeaderC11 = new Paragraph("Valor Diario", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
                var cellC11 = new PdfPCell()
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Colspan = 2,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                };
                cellC11.AddElement(tHeaderC11);
                tableH.AddCell(cellC11);

                //Insert data
                lstValoresDiarios.ForEach(v =>
                {
                    var cellH1 = new PdfPCell(new Paragraph(v.NombreAmparo, arialTable) { Alignment = Element.ALIGN_LEFT })
                    {
                        Colspan = 8,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    tableH.AddCell(cellH1);

                    var i = 0;
                    var cellH2 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? $"{Decimal.ToInt32(v.Opciones[i].NumeroDias)}" : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH2);
                    
                    var cellH5 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorDiario.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH5);
                    i++;
                    var cellH3 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? $"{Decimal.ToInt32(v.Opciones[i].NumeroDias)}" : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH3);
                    
                    var cellH4 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorDiario.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH4);
                    i++;
                    var cellH6 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? $"{Decimal.ToInt32(v.Opciones[i].NumeroDias)}" : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH6);
                    
                    var cellH7 = new PdfPCell(new Paragraph($"{(v.NombreAmparo != "ASISTENCIA" ? v.Opciones[i].ValorDiario.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO")) : (v.NombreAmparo == "ASISTENCIA" && v.Opciones[i].ValorAsegurado == 0) ? "NO" : "SI")}", arialTable) { Alignment = Element.ALIGN_RIGHT })
                    {
                        Colspan = 2,
                        BorderWidth = 0.5f,
                        BorderColor = BaseColor.LightGray,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };
                    tableH.AddCell(cellH7);
                });

                //Insert Cells into Table
                cellP.AddElement(tableH);
                tValores3.AddCell(cellP);

                //Insert Table to Document
                if (lstValoresDiarios.Count() != 0)
                {
                    document.Add(tValores3);
                }
            }
        }

        public async Task EdadesAmparosTable(Document document, List<EdadAmparoSlip> edades)
        {
            document.Add(new Paragraph("EDADES DE INGRESO Y PERMANENCIA", arialBoldTable));

            var tEdades = new PdfPTable(20)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
                SpacingBefore = 5
            };

            tEdades.DefaultCell.Border = PdfPCell.NO_BORDER;

            //Columns Definition
            var cellP = new PdfPCell()
            {
                Colspan = 20,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            var tableH = new PdfPTable(20)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };

            var tHeaderC1 = new Paragraph("EDADES POR AMPARO", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
            var cellC1 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 8,
                PaddingLeft = 10f,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC1.AddElement(tHeaderC1);
            tableH.AddCell(cellC1);

            var tHeaderC2 = new Paragraph("EDAD MÍNIMA DE INGRESO \n años + 364 días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC2.AddElement(tHeaderC2);
            tableH.AddCell(cellC2);

            var tHeaderC3 = new Paragraph("EDAD MÁXIMA DE INGRESO \n años + 364 días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC3 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC3.AddElement(tHeaderC3);
            tableH.AddCell(cellC3);

            var tHeaderC4 = new Paragraph("EDAD MÁXIMA DE PERMANENCIA \n años + 364 días", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC4 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC4.AddElement(tHeaderC4);
            tableH.AddCell(cellC4);

            //Insert data
            edades.ForEach(e =>
            {
                var cellH1 = new PdfPCell(new Paragraph(e.NombreAmparo, arialTable) { Alignment = Element.ALIGN_LEFT })
                {
                    Colspan = 8,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                tableH.AddCell(cellH1);
                var cellH2 = new PdfPCell(new Paragraph($"{e.EdadMinimaIngreso}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH2);
                var cellH3 = new PdfPCell(new Paragraph($"{e.EdadMaximaIngreso}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH3);
                var cellH4 = new PdfPCell(new Paragraph($"{e.EdadMaximaPermanencia}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH4);
            });

            //Insert Comment if hasEdadMaximaPermanencia
            var hasEMP = edades.Any(e => e.EdadMaximaPermanencia == 111);
            if (hasEMP)
            {
                document.Add(new Paragraph("* INDEFINIDA O HASTA LA FECHA EN QUE DEJE DE PERTENECER AL GRUPO ASEGURADO", arialTable) { Alignment = Element.ALIGN_RIGHT });
            }

            //Insert Cells into Table
            cellP.AddElement(tableH);
            tEdades.AddCell(cellP);

            //Insert Table to Document
            document.Add(tEdades);
        }

        public async Task MaximoValoAseguradoIndividualSection(Document document, ValorMaximoSlip valorMaximo)
        {
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(valorMaximo.Seccion, arialBold));
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(valorMaximo.Texto, arial)
            {
                Alignment = Element.ALIGN_JUSTIFIED,
                Leading = 10f
            });
            document.Add(new Phrase("\n"));
        }

        public async Task ClausulasSection(Document document)
        {
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(slip.Clausulas.Seccion, arialBold));
            document.Add(new Phrase("\n"));
            document.Add(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, 1));

            //Clausulas
            foreach (var cl in slip.Clausulas.Clausulas)
            {
                document.Add(new Phrase("\n"));
                document.Add(new Paragraph(cl.Seccion, arialBold));
                document.Add(new Paragraph(cl.Texto, arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });

                //Requisitos de Asegurabilidad
                if (cl.CodigoSeccion == "35")
                {
                    await this.RequisitosAsegurabilidadTable(document, slip.Clausulas.Asegurabilidad);
                }

                //Documentos en Caso de Reclamación
                if (cl.CodigoSeccion == "61")
                {
                    await this.DocumentosReclamacionTable(document);
                }
            }
        }

        public async Task RequisitosAsegurabilidadTable(Document document, IEnumerable<Asegurabilidad> asegurabilidad)
        {
            document.Add(new Phrase("\n"));
            //Table definition
            var tReqsAsegurabilidad = new PdfPTable(20)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            tReqsAsegurabilidad.DefaultCell.Border = PdfPCell.NO_BORDER;

            //Columns Definition
            var cellP = new PdfPCell()
            {
                Colspan = 20,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                PaddingTop = 0,
                PaddingBottom = 0
            };

            var tableH = new PdfPTable(20)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };

            var tHeaderC1 = new Paragraph("Edad Desde", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC1 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
            };
            cellC1.AddElement(tHeaderC1);
            tableH.AddCell(cellC1);


            var tHeaderC2 = new Paragraph("Edad Hasta", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
            };
            cellC2.AddElement(tHeaderC2);
            tableH.AddCell(cellC2);

            var tHeaderC3 = new Paragraph("Valor Individual Desde", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC3 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
            };
            cellC3.AddElement(tHeaderC3);
            tableH.AddCell(cellC3);

            var tHeaderC4 = new Paragraph("Valor Individual Hasta", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC4 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
            };
            cellC4.AddElement(tHeaderC4);
            tableH.AddCell(cellC4);

            var tHeaderC5 = new Paragraph("Requisitos", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var cellC5 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
            };
            cellC5.AddElement(tHeaderC5);
            tableH.AddCell(cellC5);

            //Insert Data
            foreach (var ase in asegurabilidad)
            {
                var cellH1 = new PdfPCell(new Paragraph($"{ase.EdadDesde}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH1);
                var cellH2 = new PdfPCell(new Paragraph($"{ase.EdadHasta}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH2);
                var cellH3 = new PdfPCell(new Paragraph($"{ase.ValorIndividualDesde.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH3);
                var cellH4 = new PdfPCell(new Paragraph($"{ase.ValorIndividualHasta.ToString("C0", CultureInfo.CreateSpecificCulture("es-CO"))}", arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH4);
                var cellH5 = new PdfPCell(new Paragraph(ase.Requisitos, arialTable) { Alignment = Element.ALIGN_CENTER })
                {
                    Colspan = 4,
                    BorderWidth = 0.5f,
                    BorderColor = BaseColor.LightGray,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                tableH.AddCell(cellH5);
            }

            //Insert Cells into Table
            cellP.AddElement(tableH);
            tReqsAsegurabilidad.AddCell(cellP);

            //Insert Table to Document
            document.Add(tReqsAsegurabilidad);

            //DefinicionReqAseqTable
            this.DefinicionReqAsegTable(document);
        }

        public void DefinicionReqAsegTable(Document document)
        {
            var tReqsAsegDefinition = new PdfPTable(3)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                SpacingBefore = 10
            };

            var cell = new PdfPCell(new Phrase("A", arialBoldTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("B", arialBoldTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("C", arialBoldTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Solicitud Individual (declaración de asegurabilidad)", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Rowspan = 4
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Solicitud Individual", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Solicitud Individual", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Examen Médico General (Forma SV-03)", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Examen Médico General (Forma SV-03)", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Análisis de Orina Completo", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Análisis de Orina Completo", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Electrocardiograma en reposo", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("Electrocardiograma en reposo", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);

            cell = new PdfPCell(new Phrase("", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);
            cell = new PdfPCell(new Phrase("", arialTable))
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            tReqsAsegDefinition.AddCell(cell);
            //Cell with text and list
            var phrase = new Phrase("Análisis de Sangre (completo)", arialTable);
            var listC1 = new List()
            {
                IndentationLeft = 10f
            };
            listC1.Add(new ListItem("Hemograma", arialTable));
            listC1.Add(new ListItem("Glicemia", arialTable));
            listC1.Add(new ListItem("Creatinina", arialTable));
            listC1.Add(new ListItem("Colesterol", arialTable));
            listC1.Add(new ListItem("Triglicéridos", arialTable));
            listC1.Add(new ListItem("Transaminasas", arialTable));
            listC1.Add(new ListItem("Prueba H.I.V. (Elisa)", arialTable));

            cell = new PdfPCell()
            {
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                HorizontalAlignment = Element.ALIGN_LEFT
            };
            cell.AddElement(phrase);
            cell.AddElement(listC1);
            tReqsAsegDefinition.AddCell(cell);

            

            //Insert table into document
            document.Add(tReqsAsegDefinition);
        }

        public async Task DocumentosReclamacionTable(Document document)
        {
            document.Add(new Phrase("\n"));
            //Table definition
            var tDocumentos = new PdfPTable(4)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };

            //Columns Definition
            var tName = new Paragraph("DOCUMENTOS PARA RECLAMACION POR SINIESTRO", arialBoldTable) { Alignment = Element.ALIGN_CENTER };
            var nameCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray,
                BackgroundColor = BaseColor.LightGray
            };
            nameCell.AddElement(tName);
            tDocumentos.AddCell(nameCell);

            var tHeaderC1 = new Paragraph("Accidente:", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
            var listC1 = new List()
            {
                IndentationLeft = 10f
            };
            listC1.Add(new ListItem("Documento de identidad del asegurado", arialTable));
            listC1.Add(new ListItem("Historia Clínica", arialTable));
            listC1.Add(new ListItem("Incapacidad Médica", arialTable));
            var cellC1 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC1.AddElement(tHeaderC1);
            cellC1.AddElement(listC1);

            var tHeaderC2 = new Paragraph("Enfermedad:", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
            var listC2 = new List()
            {
                IndentationLeft = 10f
            };
            listC2.Add(new ListItem("Documento de identidad del asegurado", arialTable));
            listC2.Add(new ListItem("Historia Clínica", arialTable));
            listC2.Add(new ListItem("Incapacidad Médica", arialTable));
            var cellC2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC2.AddElement(tHeaderC2);
            cellC2.AddElement(listC2);

            var tHeaderC3 = new Paragraph("Incapacidad permanente:", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
            var listC3 = new List()
            {
                IndentationLeft = 10f
            };
            listC3.Add(new ListItem("Documento de identidad del asegurado", arialTable));
            listC3.Add(new ListItem("Informe para Calificación de pérdida de capacidad laboral (PCL)", arialTable));
            listC3.Add(new ListItem("Fotocopia Historia Clínica", arialTable));
            var cellC3 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC3.AddElement(tHeaderC3);
            cellC3.AddElement(listC3);

            var tHeaderC4 = new Paragraph("Muerte:", arialBoldTable) { Alignment = Element.ALIGN_LEFT };
            var listC4 = new List()
            {
                IndentationLeft = 10f
            };
            listC4.Add(new ListItem("Documento de identidad del asegurado", arialTable));
            listC4.Add(new ListItem("Registro civil de defunción", arialTable));
            listC4.Add(new ListItem("Inspección Técnica del Cadáver o Certificado de Necropsia", arialTable));
            listC4.Add(new ListItem("Historia Clínica", arialTable));
            listC4.Add(new ListItem("Documento de identificación de los beneficiarios", arialTable));
            var cellC4 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 4,
                BorderWidth = 0.5f,
                BorderColor = BaseColor.LightGray
            };
            cellC4.AddElement(tHeaderC4);
            cellC4.AddElement(listC4);

            //Insert Cells into Table
            tDocumentos.AddCell(cellC1);
            tDocumentos.AddCell(cellC2);
            tDocumentos.AddCell(cellC3);
            tDocumentos.AddCell(cellC4);

            //Insert Table to Document
            document.Add(tDocumentos);
        }

        public async Task CondicionesSection(Document document)
        {
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(slip.Condiciones.Seccion, arialBold));
            document.Add(new Phrase("\n"));
            document.Add(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, 1));
            document.Add(new Paragraph(slip.Condiciones.Condiciones, arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });
        }

        public async Task DisposicionesFinalesSection(Document document)
        {
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph(slip.Disposiciones.Seccion, arialBold));
            document.Add(new Phrase("\n"));
            document.Add(new LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_LEFT, 1));

            //Disposiciones
            foreach (var df in slip.Disposiciones.Disposiciones)
            {
                document.Add(new Phrase("\n"));
                document.Add(new Paragraph(df.Texto, arial) { Alignment = Element.ALIGN_JUSTIFIED, Leading = 10f });
            }

            //Final
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph("Cordial Saludo,", arial));
            document.Add(new Phrase("\n"));
            document.Add(new Paragraph("FIRMA AUTORIZADA", arialBold));
            document.Add(new Paragraph($"Agencia {slip.NombreSucursal}", arial));
        }

        private static byte[] AddWatermark(byte[] bytes, BaseFont baseFont, string watermarkText)
        {
            using (var ms = new MemoryStream(10 * 1024))
            {
                var reader = new PdfReader(bytes);
                var stamper = new PdfStamper(reader, ms);
                {
                    var pages = reader.NumberOfPages;
                    for (var i = 1; i <= pages; i++)
                    {
                        var dc = stamper.GetOverContent(i);
                        AddWaterMarkText(dc, watermarkText, baseFont, 50, 45, BaseColor.Gray, reader.GetPageSizeWithRotation(i));
                    }
                    stamper.Close();
                }
                return ms.ToArray();
            }
        }

        private static void AddWaterMarkText(PdfContentByte pdfData, string watermarkText, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize)
        {
            var gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            pdfData.SaveState();
            pdfData.SetGState(gstate);
            pdfData.SetColorFill(color);
            pdfData.BeginText();
            pdfData.SetFontAndSize(font, fontSize);
            var x = (realPageSize.Right + realPageSize.Left) / 2;
            var y = (realPageSize.Bottom + realPageSize.Top) / 2;
            pdfData.ShowTextAligned(Element.ALIGN_CENTER, watermarkText, x, y, angle);
            pdfData.EndText();
            pdfData.RestoreState();
        }
    }
}
