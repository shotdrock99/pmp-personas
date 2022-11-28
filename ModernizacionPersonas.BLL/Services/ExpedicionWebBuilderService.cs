using ModernizacionPersonas.BLL.AppConfig;
using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Common;
using ModernizacionPersonas.DAL.Services;
using ModernizacionPersonas.DAL.Soligespro;
using ModernizacionPersonas.Entities;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Services
{
    public class ExpedicionWebBuilderService
    {
        private readonly ExpedicionWeb expedicionWeb;
        private readonly DatosParametrizacionReader servicioParametrizacion;
        private readonly IDatosPersonasReader informacionPersonasReader;
        private readonly IDatosGruposAseguradoReader grupoAseguradosReader;
        private readonly IDatosOpcionValorAseguradoReader opcionValorReader;
        private readonly IAuthorizationsDataReader authorizationsReader;
        private readonly IDatosEdadesReader edadesReader;
        private readonly ITransactionsDataReader tranAuthReader;
        private readonly ITransactionCommentsDataReader tranCommentReader;
        private readonly IDatosIntermediarioReader intermediarioReader;
        private readonly IDatosTomadorReader tomadorReader;
        private readonly SoligesproDataUsuariosReader soligespro;
        private readonly VariablesParametrizacionDataProvider variablesSlip;
        private readonly ConfiguracionSlipDataProvider slipDataProvider;
        private readonly DatosGruposAseguradosMapper dataGrupoMap;
        FichaTecnicaDataProvider fichaTecnicaData;
        private readonly AppConfigurationFromJsonFile AppConfig;
        private string basePath = "";
        private readonly int codigoCotizacion;
        private readonly string numeroCotizacion;
        private readonly int version;

        public ExpedicionWebBuilderService(ExpedicionWeb expedicionWeb)
        {
            this.expedicionWeb = expedicionWeb;
            this.codigoCotizacion = this.expedicionWeb.InformacionNegocio.CodigoCotizacion;
            this.numeroCotizacion = this.expedicionWeb.InformacionNegocio.NumeroCotizacion;
            this.version = this.expedicionWeb.InformacionNegocio.Version;
            this.servicioParametrizacion = new DatosParametrizacionReader();
            this.informacionPersonasReader = new InformacionPersonasReader();
            this.grupoAseguradosReader = new DatosGruposAseguradosTableReader();
            this.opcionValorReader = new DatosOpcionValorAseguradoTableReader();
            this.authorizationsReader = new AuthorizationsDataTableReader();
            this.edadesReader = new DatosEdadesTableReader();
            this.tranAuthReader = new TransactionsDataTableReader();
            this.tranCommentReader = new TransactionsCommentsDataTableReader();
            this.soligespro = new SoligesproDataUsuariosReader();
            this.intermediarioReader = new DatosIntermediarioTableReader();
            this.tomadorReader = new DatosTomadorTableReader();
            this.variablesSlip = new VariablesParametrizacionDataProvider();
            this.slipDataProvider = new ConfiguracionSlipDataProvider();
            this.dataGrupoMap = new DatosGruposAseguradosMapper();
            fichaTecnicaData = new FichaTecnicaDataProvider();
            this.AppConfig = new AppConfigurationFromJsonFile();
            var rootPMP = @"\\FILESERVERIBM\PMP_Repositorio";
            this.basePath = $@"{rootPMP}\{this.AppConfig.NameEnvironemnt}\";
        }

        /// <summary>
        /// Create ExcelPackage / Worksheet / Workbook
        /// Initiate and Decouple methods to create each document section properly
        /// </summary>
        public async Task GenerateExpedicion()
        {
            var excelPackage = new ExcelPackage();
            //Add new worksheet to the empty workbook and set primary properties
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Resumen Expedición");
            worksheet.Cells.Style.Font.Name = "Calibri";
            //worksheet.View.PageLayoutView = true;
            worksheet.PrinterSettings.PaperSize = ePaperSize.Tabloid;
            worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
            //worksheet.PrinterSettings.FitToPage = true;

            //Create header
            await this.CreateHeader(worksheet);

            //Información General Section
            await this.CreateInfoGeneralSection(worksheet);

            //Información Tomador
            await this.CreateInfoTomadorSection(worksheet);

            //Información Grupos Asegurados
            await this.CreateGruposAseguradosSection(worksheet);

            //Información Intermediarios
            await this.CreateIntermediariosSection(worksheet);

            //Información Autorizaciones
            await this.GenerateAutorizacionesSection(worksheet);

            //Save Workbook
            await this.SaveWorkbook(excelPackage, worksheet);
        }


        /// <summary>
        /// Create and decorate worksheet header section
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public async Task CreateHeader(ExcelWorksheet ws)
        {
            //Header 
            var range = ws.Cells[1, 1, 1, 9];
            range.Merge = true;
            range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.Font.Size = 12;
            range.Style.Font.Bold = true;
            range.Value = "INFORMACIÓN PARA EXPEDICIÓN SEGUROS DE PERSONAS - COTIZACIÓN Nro. " + this.numeroCotizacion + " Versión" + this.version;
            var dataUser = this.soligespro.GetUserAsync(this.expedicionWeb.InformacionNegocio.LastAuthorName);
            //Subheader
            ws.Cells["A2"].Value = "Cotizante";
            ws.Cells["A3"].Value = "Cargo / Rol";
            ws.Cells["B2"].Value = this.expedicionWeb.InformacionNegocio.LastAuthorName + " - " + dataUser.Result.NombreUsuario;
            ws.Cells["B3"].Value = dataUser.Result.Cargo;
            ws.Cells["B2"].Style.Font.Size = 14;
            ws.Cells["B2"].Style.Font.Bold = true;

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }


        /// <summary>
        /// Create and decorate worksheet Información General section
        /// </summary>
        /// <param name="ws"></param>
        /// <returns></returns>
        public async Task CreateInfoGeneralSection(ExcelWorksheet ws)
        {
            var range = ws.Cells[4, 1, 4, 9];
            var headerValue = "Información General";
            var periodos = this.servicioParametrizacion.TraerPeriodoFacturacionAsync();
            var periodoSel = periodos.Result.Where(x => x.CodigoPeriodo == this.expedicionWeb.InformacionNegocio.CodigoPeriodoFacturacion).FirstOrDefault();

            var tipoNegocioSel = this.informacionPersonasReader.TraerTipoNegocioxCodigoAsync(this.expedicionWeb.InformacionNegocio.CodigoTipoNegocio).Result;

            var tipoContratacion = this.informacionPersonasReader.TraerTipoContratacionxCodigoAsync(this.expedicionWeb.InformacionNegocio.CodigoTipoContratacion).Result;

            DateTime DefaultDate = DateTime.Parse("0001-01-01T00:00:00");
            //Generate its header
            await this.GenerateSectionHeaders(range, headerValue);

            //Info Names
            ws.Cells["A5"].Value = "Zona";
            ws.Cells["A6"].Value = "Agencia";
            ws.Cells["A7"].Value = "Ramo";
            ws.Cells["A8"].Value = "Sub ramo";
            ws.Cells["A9"].Value = "Tipo de Operación";
            ws.Cells["A10"].Value = "Sector";
            ws.Cells["A11"].Value = "Tipo de Tasa";
            ws.Cells["A12"].Value = "Vigencia Desde / Hasta";
            ws.Cells["A13"].Value = "Período de Facturación";
            ws.Cells["A14"].Value = "Tipo de Negocio";
            ws.Cells["A15"].Value = "Tipo de Contratación";
            ws.Cells["A16"].Value = "Director Comercial";

            //Info Values
            ws.Cells["B5"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoZona} - {this.expedicionWeb.FichaTecnica.Zona}";
            ws.Cells["B6"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoSucursal} - {this.expedicionWeb.FichaTecnica.Sucursal}";
            ws.Cells["B7"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoRamo} - {this.expedicionWeb.FichaTecnica.Ramo}";
            ws.Cells["B8"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoSubramo} - {this.expedicionWeb.FichaTecnica.Subramo}";
            ws.Cells["B9"].Value = "100% Compañía";
            ws.Cells["B10"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoSector} - {this.expedicionWeb.FichaTecnica.Sector}";
            ws.Cells["B11"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoTipoTasa1} - {this.expedicionWeb.FichaTecnica.TipoTasa.NombreTasa} ";
            if (this.expedicionWeb.InformacionNegocio.FechaInicio == DefaultDate)
            {
                ws.Cells["B12"].Value = "Por Definir";
            }
            else
            {
                ws.Cells["B12"].Value = $"{this.expedicionWeb.InformacionNegocio.FechaInicio?.ToString("dd/MM/yyyy")} - {this.expedicionWeb.InformacionNegocio.FechaFin?.ToString("dd/MM/yyyy")}";
            }
            ws.Cells["B13"].Value = $"{periodoSel.CodigoPeriodo} - {periodoSel.NombrePeriodo}";
            ws.Cells["B14"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoTipoNegocio} - {tipoNegocioSel.NombreTipoNegocio}";
            ws.Cells["B15"].Value = $"{this.expedicionWeb.InformacionNegocio.CodigoTipoContratacion} - {tipoContratacion.NombreTipoContratacion}";
            ws.Cells["B16"].Value = $"{this.expedicionWeb.InformacionNegocio.NombreDirectorComercial}";

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task CreateInfoTomadorSection(ExcelWorksheet ws)
        {
            var range = ws.Cells[18, 1, 18, 9];
            var headerValue = "Información Tomador";

            //Generate its header
            await this.GenerateSectionHeaders(range, headerValue);

            //Info Names
            ws.Cells["A19"].Value = "Tipo Documento";
            ws.Cells["A20"].Value = "No. Documento";
            ws.Cells["A21"].Value = "Nombre";
            ws.Cells["A22"].Value = "% Gastos de Administración";
            ws.Cells["A23"].Value = "Clasificación Actividad";
            ws.Cells["A24"].Value = "Actividad Económica";
            var riesgoResponse = await informacionPersonasReader.TraerRiesgoActividadAsync(this.expedicionWeb.InformacionNegocio.CodigoTipoRiesgo);
            //Info Values
            ws.Cells["B19"].Value = this.expedicionWeb.Slip.Tomador.CodigoTipoDocumento == 1 ? "CC" : "NIT";
            ws.Cells["B20"].Value = this.expedicionWeb.Slip.Tomador.NumeroDocumento;
            ws.Cells["B21"].Value = this.expedicionWeb.Slip.Tomador.Nombre;
            ws.Cells["B22"].Value = this.expedicionWeb.InformacionNegocio.PorcentajeRetorno / 100;
            ws.Cells["B23"].Value = this.expedicionWeb.InformacionNegocio.CodigoTipoRiesgo + " - " + riesgoResponse.NombreRiesgoActividad;
            ws.Cells["B24"].Value = this.expedicionWeb.Slip.Tomador.Actividad;

            //Formating Cells
            ws.Cells["B22"].Style.Numberformat.Format = "#0.00%";
            ws.Cells["B22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task CreateGruposAseguradosSection(ExcelWorksheet ws)
        {

            try
            {
                var range = ws.Cells[26, 1, 26, 9];
                var headerValue = "Grupos de Asegurados / Categorías";

                //Generate its header
                await this.GenerateSectionHeaders(range, headerValue);

                //Calculate edades
                var edades = await this.CalculateEdadesGrupos(this.expedicionWeb.Slip.GruposAsegurados);

                //Info Names
                ws.Cells["A27"].Value = "Edad Mínima Ingreso";
                ws.Cells["A28"].Value = "Edad Máxima Ingreso";
                ws.Cells["A29"].Value = "Edad Máxima Permanencia";
                ws.Cells["A30"].Value = "Amparo Automático";
                ws.Cells["A31"].Value = "Número de días";
                ws.Cells["A32"].Value = "Edad tope";
                ws.Cells["A33"].Value = "Valor Asegurado";

                //BUSCA AMPARO AUTOMÁTICO
                var clausulas = this.expedicionWeb.Slip.Clausulas;
                var amparoAut = clausulas.Clausulas.Where(x => x.CodigoSeccion.Equals("44"));
                var subramo = this.expedicionWeb.InformacionNegocio.CodigoSubramo;
                var sector = this.expedicionWeb.InformacionNegocio.CodigoSector;
                var amparoData = await this.slipDataProvider.ConstruirClausulasSlipAsync(this.codigoCotizacion, this.expedicionWeb.InformacionNegocio.CodigoRamo, subramo, sector);


                var variablesAmparoAut = amparoData.Where(x => x.CodigoSeccion == 44);
                var numDiasAmparoAut = "";
                var sumaAseguradatopeAmparoAut = "";
                var edadAmparoAut = "";
                foreach (var i in variablesAmparoAut)
                {
                    numDiasAmparoAut = i.Variables.Where(o => o.CodigoVariable == 10).FirstOrDefault().Valor;
                    edadAmparoAut = i.Variables.Where(o => o.CodigoVariable == 11).FirstOrDefault().Valor;
                    sumaAseguradatopeAmparoAut = i.Variables.Where(o => o.CodigoVariable == 12).FirstOrDefault().Valor;
                }

                
                string txtAmpAut = "";
                string txtSiAmoAut = "NO";
                if (amparoAut.Count() > 0)
                {
                    txtSiAmoAut = "SI";
                    txtAmpAut = amparoAut.FirstOrDefault().Texto;
                }

                //Info Values
                ws.Cells["B27"].Value = edades[0];
                ws.Cells["B28"].Value = edades[1];
                ws.Cells["B29"].Value = edades[2];
                ws.Cells["B30"].Value = txtSiAmoAut;
                ws.Cells["B31"].Value = numDiasAmparoAut;
                ws.Cells["B32"].Value = edadAmparoAut;
                if (sumaAseguradatopeAmparoAut == "") {
                    ws.Cells["B33"].Value = "";
                }
                else {
                    ws.Cells["B33"].Value = Decimal.Parse(sumaAseguradatopeAmparoAut);

                }
                //Generate each group description
                foreach (var grupo in this.expedicionWeb.Slip.GruposAsegurados.GruposAsegurados)
                {
                    await this.GenerateEachGroupDescription(grupo, ws);
                }

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                ws.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B33"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B33"].Style.Numberformat.Format = "$#,##0";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GenerateEachGroupDescription(GrupoAseguradoSlip grupoAsegurado, ExcelWorksheet ws)
        {
            try
            {
                var smmlv = await fichaTecnicaData.getSalarioMinimoAsync();
                int lastRow = ws.Dimension.End.Row;
                int lastColumn = ws.Dimension.End.Column;

                var infoGrupo = this.grupoAseguradosReader.GetGrupoAseguradoAsync(grupoAsegurado.CodigoGrupoAsegurado).Result;
                var tipoSuma = this.informacionPersonasReader.TraerTiposSumaAsegurada(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo).Result;
                var tipoSumaSel = tipoSuma.Where(x => x.CodigoTipoSumaAsegurada == infoGrupo.CodigoTipoSuma).FirstOrDefault();

                //Info Names
                ws.Cells[lastRow + 2, 1].Value = "Descripción Grupo";
                ws.Cells[lastRow + 3, 1].Value = "Tipo de Suma Asegurada";
                ws.Cells[lastRow + 4, 1].Value = "No. Asegurados";
                ws.Cells[lastRow + 5, 1].Value = "Listado de Asegurados";
                ws.Cells[lastRow + 6, 1].Value = "Valor Asegurado Mínimo";
                ws.Cells[lastRow + 7, 1].Value = "Valor Asegurado Máximo";
                ws.Cells[lastRow + 8, 1].Value = "Edad Promedio";
                ws.Cells[lastRow + 9, 1].Value = "Valor Total Asegurado";
                if(infoGrupo.CodigoTipoSuma == 10)
                {
                    ws.Cells[lastRow + 10, 1].Value = "Valor S.M.M.L.V";
                }

                //Info Values
                ws.Cells[lastRow + 2, 2].Value = grupoAsegurado.Nombre;
                ws.Cells[lastRow + 3, 2].Value = tipoSumaSel.NombreTipoSumaAsegurada;
                ws.Cells[lastRow + 4, 2].Value = infoGrupo.NumeroAsegurados;
                ws.Cells[lastRow + 5, 2].Value = this.expedicionWeb.InformacionNegocio.ConListaAsegurados == true ? "SI" : "NO";
                ws.Cells[lastRow + 6, 2].Value = infoGrupo.ValorMinAsegurado;
                ws.Cells[lastRow + 7, 2].Value = infoGrupo.ValorMaxAsegurado;
                ws.Cells[lastRow + 8, 2].Value = infoGrupo.EdadPromedioAsegurados;
                ws.Cells[lastRow + 9, 2].Value = infoGrupo.ValorAsegurado;
                if (infoGrupo.CodigoTipoSuma == 10)
                {
                    ws.Cells[lastRow + 10, 2].Value = smmlv;
                }

                //Formating Cells;
                ws.Cells[lastRow + 2, 1].Style.Font.Bold = true;
                ws.Cells[lastRow + 9, 1].Style.Font.Bold = true;
                ws.Cells[lastRow + 4, 2].Style.Numberformat.Format = "#,##0";
                ws.Cells[lastRow + 6, 2, lastRow + 7, 2].Style.Numberformat.Format = "$#,##0";
                ws.Cells[lastRow + 9, 2].Style.Numberformat.Format = "$#,##0";
                ws.Cells[lastRow + 10, 2].Style.Numberformat.Format = "$#,##0";
                ws.Cells[lastRow + 4, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[lastRow + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[lastRow + 7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[lastRow + 8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[lastRow + 9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells[lastRow + 10, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;



                //Generate its amparos table / Send TSA
                if (infoGrupo.CodigoTipoSuma == 1)
                {
                    var opciones = new List<int> { 1, 2, 3 };
                    var ng = 1;
                    foreach (var op in opciones)
                    {
                        await this.GenerateSumaFijaTable(ws, grupoAsegurado, op);
                        ng++;
                    }
                }
                else if (infoGrupo.CodigoTipoSuma == 2)
                {
                    //Generate its amparos table / Send TSA
                    await this.GenerateSumaUnificadaSmmlvTable(ws, grupoAsegurado, infoGrupo);
                }
                else if (infoGrupo.CodigoTipoSuma == 3)
                {
                    await this.GenerateSumaInformadaTable(ws, grupoAsegurado, infoGrupo);

                }
                else if (infoGrupo.CodigoTipoSuma == 4)
                {
                    //Generate its amparos table / Send TSA
                    await this.GenerateSumaVariableMultiploSueldosTable(ws, grupoAsegurado, infoGrupo);
                }
                else if (infoGrupo.CodigoTipoSuma == 5)
                {
                    //Generate its amparos table / Send TSA
                    await this.GenerateSumaUniformeMasMultiploSueldosTable(ws, grupoAsegurado, infoGrupo);
                }
                else if (infoGrupo.CodigoTipoSuma == 6)
                {
                    //Generate its amparos table / Send TSA
                    await this.GenerateSumaSaldoDeudoresTable(ws, grupoAsegurado, infoGrupo);
                }
                else if (infoGrupo.CodigoTipoSuma == 10)
                {
                    //Generate its amparos table / Send TSA
                    await this.GenerateSumaSMMLVTable(ws, grupoAsegurado, infoGrupo);
                }

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateIntermediariosSection(ExcelWorksheet ws)
        {
            var lastRow = ws.Dimension.End.Row + 1;
            var range = ws.Cells[lastRow + 1, 1, lastRow + 1, 9];
            var headerValue = "Intermediarios";

            //Generate header section
            await this.GenerateSectionHeaders(range, headerValue);



            //Generate each group description
            var intermediarios = this.intermediarioReader.GetIntermediariosAsync(this.codigoCotizacion).Result;//this.expedicionWeb.FichaTecnica.InformacionTomador.Intermediarios.ToList();
            foreach (var it in intermediarios)
            {
                await this.GenerateEachIntermediarioDescription(it, ws);
            }
            lastRow = ws.Dimension.End.Row + 1;
            var comision = this.expedicionWeb.InformacionNegocio.PorcentajeComision / 100;

            ws.Cells[lastRow + 1, 1].Value = "% de Comisión";
            ws.Cells[lastRow + 1, 2].Value = comision;
            ws.Cells[lastRow + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[lastRow + 1, 2].Style.Numberformat.Format = "#0%";
            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task GenerateEachIntermediarioDescription(Intermediario it, ExcelWorksheet ws)
        {
            var intermediario = await this.informacionPersonasReader.TraerIntermediarioAsync(this.expedicionWeb.InformacionNegocio.CodigoSucursal, (int)it.Clave);
            var lastRow = ws.Dimension.End.Row + 1;


            //Info Names
            ws.Cells[lastRow + 1, 1].Value = "Nombre";
            ws.Cells[lastRow + 2, 1].Value = "Tipo de Agente";
            ws.Cells[lastRow + 3, 1].Value = "Código";
            ws.Cells[lastRow + 4, 1].Value = "Tipo Documento";
            ws.Cells[lastRow + 5, 1].Value = "# Documento";
            ws.Cells[lastRow + 6, 1].Value = "% de Participación";

            //Info Values
            ws.Cells[lastRow + 1, 2].Value = $"{it.PrimerNombre} {it.SegundoNombre} {it.PrimerApellido} {it.SegundoApellido}";
            ws.Cells[lastRow + 2, 2].Value = string.IsNullOrEmpty(intermediario.NombreTipoAgente) ? "N/A" : intermediario.NombreTipoAgente;
            ws.Cells[lastRow + 3, 2].Value = it.Clave;
            ws.Cells[lastRow + 4, 2].Value = it.CodigoTipoDocumento == 1 ? "CC" : "NIT";
            ws.Cells[lastRow + 5, 2].Value = it.NumeroDocumento;
            ws.Cells[lastRow + 6, 2].Value = it.Participacion / 100;

            //Formatin Cells
            ws.Cells[lastRow + 1, 1].Style.Font.Bold = true;
            ws.Cells[lastRow + 3, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[lastRow + 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[lastRow + 6, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            ws.Cells[lastRow + 6, 2].Style.Numberformat.Format = "#0%";

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task GenerateAutorizacionesSection(ExcelWorksheet ws)
        {
            var lastRow = ws.Dimension.End.Row + 1;
            var range = ws.Cells[lastRow + 3, 1, lastRow + 3, 9];
            var headerValue = "Autorizaciones";

            //Generate header section
            await this.GenerateSectionHeaders(range, headerValue);

            //Generate Controles table
            await this.GenerateControlesTable(ws);

            //Generate Autorizadores table
            await this.GenerateAutorizadoresTable(ws);

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task GenerateControlesTable(ExcelWorksheet ws)
        {
            var numCot = int.Parse(this.numeroCotizacion);
            
            var authorzaciones = this.authorizationsReader.GetAuthorizationsByCotizacionAsync(numCot, this.version);
            var controles = authorzaciones.Result.Authorizations.Select(x => x.MensajeValidacion).ToList();

            var lastRow = ws.Dimension.End.Row + 1;
            //This range depends on #controls
            var range = controles.Count() > 0 ? ws.Cells[lastRow + 1, 1, lastRow + controles.Count() + 1, 1] : ws.Cells[lastRow + 1, 1, lastRow + 2, 1];

            // Create table with apropiate range
            var table = ws.Tables.Add(range, "Controles");

            //Set columns & names
            table.Columns[0].Name = "Detalle Control Autorizado";

            //Set values
            //var controles = new List<string> { "Hola", "Hola1", "Hola2", "Hola3", "Hola4" };
            var lr = ws.Dimension.End.Row;
            var j = 1;

            for (int i = 0; i < controles.Count(); i++)
            {
                ws.Cells[lr + j, 1].Value = controles[i];
                j++;
            }

            //Disable filter
            table.ShowFilter = false;

            //Styling table
            table.TableStyle = TableStyles.Light11;

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();

        }

        public async Task GenerateAutorizadoresTable(ExcelWorksheet ws)
        {
            var autorizadores = this.tranAuthReader.GetAuthorizationTransactionsAsync(this.codigoCotizacion, this.version).Result;
            autorizadores = autorizadores.Where(x => x.CodigoEstadoCotizacion != 1110);

            var lastRow = ws.Dimension.End.Row + 1;
            //This range depends on #autorizadores
            var range = autorizadores.Count() > 0 ? ws.Cells[lastRow + 3, 1, lastRow + autorizadores.Count() + 3, 3] : ws.Cells[lastRow + 7, 1, lastRow + 8, 3];


            // Create table with apropiate range
            var table = ws.Tables.Add(range, "Autorizadores");

            //Set columns & names
            table.Columns[0].Name = "Notificado / Autorizador";
            table.Columns[1].Name = "Tipo";
            table.Columns[2].Name = "Descripción";


            var autor = new List<string>();

            var tipo = new List<string>();

            var descripcion = new List<string>();

            foreach (var tran in autorizadores)
            {
                var username = this.soligespro.GetUserAsync(tran.CodigoUsuario).Result;
                var comments = this.tranCommentReader.GetTransactionsCommentsAsync(tran.CodigoTransaccion).Result;

                autor.Add(username.NombreUsuario);
                tipo.Add(tran.Description);
                descripcion.Add(comments.FirstOrDefault()?.Message);
            }

            var lr = ws.Dimension.End.Row;
            var j = 1;

            for (int i = 0; i < autorizadores.Count(); i++)
            {
                ws.Cells[lr + j, 1].Value = autor[i];
                ws.Cells[lr + j, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                ws.Cells[lr + j, 2].Value = tipo[i];
                ws.Cells[lr + j, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                ws.Cells[lr + j, 3].Value = descripcion[i];
                ws.Cells[lr + j, 3].Style.WrapText = true;
                j++;
            }

            //Disable filter
            table.ShowFilter = false;

            //Styling table
            table.TableStyle = TableStyles.Light11;

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task GenerateSumaFijaTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, int opcion)
        {
            try
            {

                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 7];
                //range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, $"{gp}{opcion}");

                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = $"Suma Asegurada {opcion}";
                table.Columns[3].Name = "Tasa por Cobertura";
                table.Columns[4].Name = "Edad Min. Ingreso";
                table.Columns[5].Name = "Edad Max. Ingreso";
                table.Columns[6].Name = "Edad Max. Permanencia";

                // Get Edades


                //Set values 
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == gas.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();


                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();

                var ng = 0;
                var stringdata = JsonConvert.SerializeObject(opcionvaloresAmparos);
                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == opcion).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == opcion).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == opcion).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == opcion).Select(x => x.ValorAsegurado).FirstOrDefault());
                            var tasa = opcionValorReader.Result.Where(x => x.IndiceOpcion == opcion).Select(x => x.TasaComercial).FirstOrDefault();
                            tasas.Add(tasa);
                            if(gas.Edades.ElementAtOrDefault(ng) != null) { 
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                           
                                edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);

                            
                                permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);

                            }
                            else
                            {
                                edadMin.Add(null);
                                edadMax.Add(null);
                                permanenciaMax.Add(null);

                            }

                        }
                        ng++;
                    }
                }

                //var nombresValores = new List<string> { "Hola", "Hola1", "Hola2", "Hola3", "Hola4" };
                //var porcentajes = new List<int> { 1, 2, 4 };
                var lr = ws.Dimension.End.Row;
                var j = 1;

                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    //ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    ws.Cells[lr + j, 4].Value = tasas[i]; /// 100;
                    ws.Cells[lr + j, 5].Value = edadMin[i];
                    ws.Cells[lr + j, 6].Value = edadMax[i];
                    ws.Cells[lr + j, 7].Value = permanenciaMax[i];

                    //Formating values inside table
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 4].Style.Numberformat.Format = "#0.000000‰";

                    j++;
                }

                //Disable filter
                table.ShowFilter = false;

                //Styling table
                table.TableStyle = TableStyles.Light11;

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GenerateSumaVariableMultiploSueldosTableOld(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 7];

                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);

                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "Tasa por Cobertura";
                table.Columns[4].Name = "Edad Min. Ingreso";
                table.Columns[5].Name = "Edad Max. Ingreso";
                table.Columns[6].Name = "Edad Max. Permanencia";

                //Set values 
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.TipoSumaAsegurada.CodigoTipoSumaAsegurada == infoGrupo.CodigoTipoSuma).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();


                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();

                var ng = 0;

                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }

                var lr = ws.Dimension.End.Row;
                var j = 1;

                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    ws.Cells[lr + j, 4].Value = tasas[i];
                    ws.Cells[lr + j, 5].Value = edadMin[i];
                    ws.Cells[lr + j, 6].Value = edadMax[i];
                    ws.Cells[lr + j, 7].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }
                    //Formating values inside table
                    ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 4].Style.Numberformat.Format = "#0.000000‰";

                    j++;
                }

                //Disable filter
                table.ShowFilter = false;

                //Styling table
                table.TableStyle = TableStyles.Light11;

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GenerateSumaUniformeMasMultiploSueldosTableOld(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8];

                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);

                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia";

                //Set values 
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result;

                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();

                var ng = 0;

                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            numSueldos.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {
                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }

                var lr = ws.Dimension.End.Row;
                var j = 1;

                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    if (infoGrupo.CodigoTipoSuma == 2 && nombresAmparos[i] != "ASISTENCIA" && nombresAmparos[i] != "RENTA DIARIA POR HOSPITALIZACION" && nombresAmparos[i] != "RENTA MENSUAL POR ACCIDENTE")
                    {
                        ws.Cells[lr + j, 3].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    }
                    ws.Cells[lr + j, 4].Value = numSueldos[i];
                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }

                    //Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    j++;
                }

                //Disable filter
                table.ShowFilter = false;

                //Styling table
                table.TableStyle = TableStyles.Light11;

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task GenerateSumaUnificadaSmmlvTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8];

                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);

                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia";

                //Set values 
                /* var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                 var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                 var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();//.Where(x => x.IndiceOpcion == opcion).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result;

                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();


                var ng = 0;

                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {

                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            tasas.Add(null);
                            numSueldos.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {
                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }

                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }

                var lr = ws.Dimension.End.Row;
                var j = 1;

                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    if (infoGrupo.CodigoTipoSuma == 2 && nombresAmparos[i] != "ASISTENCIA" && nombresAmparos[i] != "RENTA DIARIA POR HOSPITALIZACION" && nombresAmparos[i] != "RENTA MENSUAL POR ACCIDENTE")
                    {
                        ws.Cells[lr + j, 3].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    }
                    ws.Cells[lr + j, 4].Value = numSueldos[i];
                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }

                    //Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    j++;
                }

                //Disable filter
                table.ShowFilter = false;

                //Styling table
                table.TableStyle = TableStyles.Light11;

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GenerateSumaInformadaTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8];

                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);

                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia";

                //Set values 
                /* var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                 var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                 var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();//.Where(x => x.IndiceOpcion == opcion).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result;

                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();


                var ng = 0;

                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {

                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            tasas.Add(null);
                            numSueldos.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {


                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }

                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }

                var lr = ws.Dimension.End.Row;
                var j = 1;

                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    if (infoGrupo.CodigoTipoSuma == 3 && nombresAmparos[i] != "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 4].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 4].Value = numSueldos[i];
                    }

                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }

                    //Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    j++;
                }

                //Disable filter
                table.ShowFilter = false;

                //Styling table
                table.TableStyle = TableStyles.Light11;

                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task GenerateMiniTable(ExcelWorksheet ws, int codigoGrupo)
        {
            var lastRow = ws.Dimension.End.Row + 1;

            //Info Names
            ws.Cells[lastRow + 1, 1].Value = "TASA COMERCIAL ANUAL";
            ws.Cells[lastRow + 2, 1].Value = "% DESCUENTO";
            ws.Cells[lastRow + 3, 1].Value = "% RECARGO";
            ws.Cells[lastRow + 4, 1].Value = "TASA COMERCIAL TOTAL";

            decimal tasaSiniestralidad = 0;
            if (this.expedicionWeb.InformacionNegocio.anyosSiniestralidad > 0)
            {
                tasaSiniestralidad = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().Siniestralidad.TasaComercial;
            }

            var tasaComercialAplicar = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().TasaComercialAplicar;
            var tasaComercial = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().TasaComercialAnual;
            var descuento = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().PorcentajeDescuento / 100;
            var recargo = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().PorcentajeRecargo / 100;

            if (tasaSiniestralidad > tasaComercial || tasaComercial == 0)
            {
                tasaComercial = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().Siniestralidad.TasaComercial;
                descuento = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().Siniestralidad.PorcentajeDescuento / 100;
                recargo = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().Siniestralidad.PorcentajeRecargo / 100;
                tasaComercialAplicar = this.expedicionWeb.Resumen.GruposAsegurados.Where(x => x.Codigo == codigoGrupo).FirstOrDefault().Opciones.FirstOrDefault().Siniestralidad.TasaComercialAplicar;
            }

            //Info Values
            ws.Cells[lastRow + 1, 2].Value = tasaComercial;
            ws.Cells[lastRow + 2, 2].Value = descuento;
            ws.Cells[lastRow + 3, 2].Value = recargo;
            ws.Cells[lastRow + 4, 2].Value = tasaComercialAplicar;

            //Formating Cells
            ws.Cells[lastRow + 1, 1, lastRow + 4, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            ws.Cells[lastRow + 1, 1, lastRow + 4, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            ws.Cells[lastRow + 1, 1, lastRow + 4, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            ws.Cells[lastRow + 1, 1, lastRow + 4, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            ws.Cells[lastRow + 1, 2].Style.Numberformat.Format = "#0.0000‰";
            ws.Cells[lastRow + 4, 2].Style.Numberformat.Format = "#0.0000‰";
            ws.Cells[lastRow + 2, 2].Style.Numberformat.Format = "#0.0000%";
            ws.Cells[lastRow + 3, 2].Style.Numberformat.Format = "#0.0000%";

            //AutoSize columns
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        public async Task<List<int>> CalculateEdadesGrupos(GruposAseguradosSlip gas)
        {
            List<EdadesGrupos> edades = new List<EdadesGrupos>();
            List<int> edadesResponse = new List<int>();

            foreach (var gp in gas.GruposAsegurados)
            {
                foreach (var edad in gp.Edades)
                {
                    edades.Add(new EdadesGrupos
                    {
                        EdadMinima = edad.EdadMinimaIngreso,
                        EdadMaxima = edad.EdadMaximaIngreso,
                        EdadMaximaPermanencia = edad.EdadMaximaPermanencia
                    });
                }
            }

            edadesResponse.Add(edades.Min(x => x.EdadMinima));
            edadesResponse.Add(edades.Max(x => x.EdadMaxima));
            edadesResponse.Add(edades.Max(x => x.EdadMaximaPermanencia));

            return edadesResponse;

        }


        /// <summary>
        /// Create each section header
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns> 
        public async Task GenerateSectionHeaders(ExcelRange range, string value)
        {
            try
            {
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightSteelBlue);
                range.Style.Font.Size = 12;
                range.Style.Font.Bold = true;
                range.Value = value;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Save workbook into server path
        /// </summary>
        /// <param name="excelPackage"></param>
        /// <returns></returns>
        public async Task SaveWorkbook(ExcelPackage excelPackage, ExcelWorksheet ws)
        {
            try
            {
                int numeroCotizacion = int.Parse(this.numeroCotizacion.TrimStart(new char[] { '0' }));

                var directoryPath = $@"{this.basePath}\{this.codigoCotizacion}";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var path = new FileInfo($@"{this.basePath}\{this.codigoCotizacion}\{numeroCotizacion}_ExpedicionSISE.xlsx");
                excelPackage.SaveAs(path);
                excelPackage.Dispose();

                var pathRead = $@"{this.basePath}\{this.codigoCotizacion}\{numeroCotizacion}_ExpedicionSISE.xlsx";

                //Protect after saved
                var workbook = new XSSFWorkbook(pathRead);
                var worksheet = workbook.GetSheet("Resumen Expedición");
                worksheet.ProtectSheet("test");
                workbook.LockRevision();
                workbook.LockStructure();
                workbook.LockWindows();

                var dp = $@"{this.basePath}\{this.codigoCotizacion}\Expedición";
                if (!Directory.Exists(dp))
                {
                    Directory.CreateDirectory(dp);
                }
                var file = File.Create($@"{this.basePath}\{this.codigoCotizacion}\Expedición\{numeroCotizacion}_ExpedicionSISE.xlsx");
                workbook.Write(file);
                file.Close();
                workbook.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<AmparoGrupoAsegurado> ObtenerAmparoBasicoNoAdicionalAsync(int codigoRamo, int codigoSubramo, int codigoSector, IEnumerable<AmparoGrupoAsegurado> amparosGrupo)
        {
            var amparos = await this.informacionPersonasReader.TraerAmparosAsync(codigoRamo, codigoSubramo, codigoSector);
            var result = (from amparoGrupo in amparosGrupo
                          join amparo in amparos.Where(x => x.SiNoBasico && !x.SiNoAdicional) on amparoGrupo.CodigoAmparo equals amparo.CodigoAmparo
                          select amparoGrupo).FirstOrDefault();

            return result;
        }
        public async Task GenerateSumaVariableMultiploSueldosTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 7]; // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp); //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "Tasa por Cobertura";
                table.Columns[4].Name = "Edad Min. Ingreso";
                table.Columns[5].Name = "Edad Max. Ingreso";
                table.Columns[6].Name = "Edad Max. Permanencia"; //Set values
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>(); var ng = 0; foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }
                var lr = ws.Dimension.End.Row;
                var j = 1; for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    ws.Cells[lr + j, 4].Value = tasas[i];
                    ws.Cells[lr + j, 5].Value = edadMin[i];
                    ws.Cells[lr + j, 6].Value = edadMax[i];
                    ws.Cells[lr + j, 7].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }
                    //Formating values inside table
                    ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 4].Style.Numberformat.Format = "#0.000000‰"; j++;
                } //Disable filter
                table.ShowFilter = false; //Styling table
                table.TableStyle = TableStyles.Light11; //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns(); //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task GenerateSumaUniformeMasMultiploSueldosTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8]; // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp); //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia"; //Set values
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result; var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>(); var ng = 0; foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            numSueldos.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {
                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }
                var lr = ws.Dimension.End.Row;
                var j = 1; for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    if (infoGrupo.CodigoTipoSuma == 2 && nombresAmparos[i] != "ASISTENCIA" && nombresAmparos[i] != "RENTA DIARIA POR HOSPITALIZACION" && nombresAmparos[i] != "RENTA MENSUAL POR ACCIDENTE")
                    {
                        ws.Cells[lr + j, 3].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    }
                    ws.Cells[lr + j, 4].Value = numSueldos[i];
                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    } //Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; j++;
                } //Disable filter
                table.ShowFilter = false; //Styling table
                table.TableStyle = TableStyles.Light11; //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns(); //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task GenerateSumaSaldoDeudoresTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8];// Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);//Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia";//Set values
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result; var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>(); var ng = 0; foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            numSueldos.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault());
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {
                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }
                var lr = ws.Dimension.End.Row;
                var j = 1; for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    if (infoGrupo.CodigoTipoSuma == 2 && nombresAmparos[i] != "ASISTENCIA" && nombresAmparos[i] != "RENTA DIARIA POR HOSPITALIZACION" && nombresAmparos[i] != "RENTA MENSUAL POR ACCIDENTE")
                    {
                        ws.Cells[lr + j, 3].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    }
                    if (infoGrupo.CodigoTipoSuma == 6 && nombresAmparos[i] != "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 4].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 4].Value = numSueldos[i];
                    }
                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }//Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right; j++;
                }//Disable filter
                table.ShowFilter = false;//Styling table
                table.TableStyle = TableStyles.Light11;//AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();//Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task GenerateSumaSMMLVTable(ExcelWorksheet ws, GrupoAseguradoSlip gas, GrupoAsegurado infoGrupo)
        {
            try
            {
                var lastRow = ws.Dimension.End.Row + 1;
                //This range depends on tsa and #amparos
                var range = ws.Cells[lastRow + 1, 1, lastRow + gas.ValoresAmparos.Count() + 1, 8];
                // Create table with apropiate range
                var gp = gas.Nombre.Replace(" ", "_");
                var table = ws.Tables.Add(range, gp);
                //Set columns & names
                table.Columns[0].Name = "Amparos";
                table.Columns[1].Name = "% Básico o Prima";
                table.Columns[2].Name = "Valor Asegurado";
                table.Columns[3].Name = "No. Sueldos";
                table.Columns[4].Name = "Tasa por Cobertura";
                table.Columns[5].Name = "Edad Min. Ingreso";
                table.Columns[6].Name = "Edad Max. Ingreso";
                table.Columns[7].Name = "Edad Max. Permanencia";
                //Set values 
                /*var amparos = this.expedicionWeb.FichaTecnica.GruposAsegurados.Select(x => x.Amparos).FirstOrDefault();
                var nombresAmparos = amparos.Select(x => x.NombreAmparo).ToList();
                var opcionvaloresAmparos = amparos.Select(x => x.OpcionesValores).ToList();*/
                var grupoAsegurado = this.expedicionWeb.FichaTecnica.GruposAsegurados.Where(o => o.Codigo == infoGrupo.CodigoGrupoAsegurado).FirstOrDefault(); //.Select(x => x.Amparos).FirstOrDefault();
                var asistencia = grupoAsegurado.Asistencias.FirstOrDefault();
                var dataAsis = await this.dataGrupoMap.ConsultarGrupoAseguradoAsync(this.codigoCotizacion, this.version, grupoAsegurado.Codigo);
                var nombresAmparos = dataAsis.AmparosGrupo.Select(o => o.AmparoInfo.NombreAmparo).ToList();
                var opcionvaloresAmparos = dataAsis.AmparosGrupo.ToList();
                var esBasico = ObtenerAmparoBasicoNoAdicionalAsync(this.expedicionWeb.InformacionNegocio.CodigoRamo, this.expedicionWeb.InformacionNegocio.CodigoSubramo, this.expedicionWeb.InformacionNegocio.CodigoSector, dataAsis.AmparosGrupo).Result;
                var porcentajes = new List<decimal?>();
                var valoresAseg = new List<decimal?>();
                var numSueldos = new List<decimal?>();
                var tasas = new List<decimal?>();
                var edadMin = new List<int?>();
                var edadMax = new List<int?>();
                var permanenciaMax = new List<int?>();
                var ng = 0;
                foreach (var amparoGrupo in opcionvaloresAmparos)
                {
                    var todasOpciones = amparoGrupo.OpcionesValores.Where(x => x.IndiceOpcion == 1).ToList();
                    var amparoGruposValores = todasOpciones.Select(x => x.CodigoAmparoGrupoAsegurado).ToList();
                    foreach (var valoresOpcion in amparoGruposValores)
                    {
                        var opcionValorReader = this.opcionValorReader.LeerOpcionValorAseguradoAsync(valoresOpcion);
                        //TODO: Valores numeros asegurados por opcion
                        if (amparoGrupo.AmparoInfo.CodigoAmparo == 95)
                        {
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.Prima).FirstOrDefault());
                            valoresAseg.Add(null);
                            numSueldos.Add(null);
                            tasas.Add(null);
                            edadMin.Add(null);
                            edadMax.Add(null);
                            permanenciaMax.Add(null);
                        }
                        else
                        {
                            foreach (var valr in opcionValorReader.Result.Where(x => x.IndiceOpcion == 1))
                            {
                                valr.ValorAsegurado = valr.PorcentajeCobertura > 0 || valr.NumeroSalarios > 0 ? 0 : valr.ValorAsegurado;
                            }
                            var valorAseg = opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.ValorAsegurado).FirstOrDefault();
                            
                            porcentajes.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault());
                            valoresAseg.Add(valorAseg);
                            tasas.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.TasaComercial).FirstOrDefault());
                            if (opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() > 0)
                            {
                                numSueldos.Add((opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.PorcentajeCobertura).FirstOrDefault() * esBasico.OpcionesValores.FirstOrDefault().NumeroSalarios) / 100);
                            }
                            else
                            {
                                numSueldos.Add(opcionValorReader.Result.Where(x => x.IndiceOpcion == 1).Select(x => x.NumeroSalarios).FirstOrDefault());
                            }
                            edadMin.Add(gas.Edades[ng].EdadMinimaIngreso);
                            edadMax.Add(gas.Edades[ng].EdadMaximaIngreso);
                            permanenciaMax.Add(gas.Edades[ng].EdadMaximaPermanencia);
                        }
                        ng++;
                    }
                }
                var lr = ws.Dimension.End.Row;
                var j = 1;
                for (int i = 0; i < nombresAmparos.Count(); i++)
                {
                    ws.Cells[lr + j, 1].Value = nombresAmparos[i];
                    if (infoGrupo.CodigoTipoSuma == 2 && nombresAmparos[i] != "ASISTENCIA" && nombresAmparos[i] != "RENTA DIARIA POR HOSPITALIZACION" && nombresAmparos[i] != "RENTA MENSUAL POR ACCIDENTE")
                    {
                        ws.Cells[lr + j, 3].Value = "n/a";
                    }
                    else
                    {
                        ws.Cells[lr + j, 3].Value = valoresAseg[i];
                    }
                    ws.Cells[lr + j, 4].Value = numSueldos[i];
                    ws.Cells[lr + j, 5].Value = tasas[i];
                    ws.Cells[lr + j, 6].Value = edadMin[i];
                    ws.Cells[lr + j, 7].Value = edadMax[i];
                    ws.Cells[lr + j, 8].Value = permanenciaMax[i];
                    if (nombresAmparos[i] == "ASISTENCIA")
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "$#,##0";
                        ws.Cells[lr + j, 2].Value = porcentajes[i];
                    }
                    else
                    {
                        ws.Cells[lr + j, 2].Style.Numberformat.Format = "#0%";
                        ws.Cells[lr + j, 2].Value = porcentajes[i] / 100;
                    }
                    //Formating values inside table
                    ws.Cells[lr + j, 3].Style.Numberformat.Format = "$#,##0";
                    ws.Cells[lr + j, 5].Style.Numberformat.Format = "#0.000000‰";
                    ws.Cells[lr + j, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    j++;
                }
                //Disable filter
                table.ShowFilter = false;
                //Styling table
                table.TableStyle = TableStyles.Light11;
                //AutoSize columns
                ws.Cells[ws.Dimension.Address].AutoFitColumns();
                //Generate mini table after each TSA table
                await this.GenerateMiniTable(ws, gas.CodigoGrupoAsegurado);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}
