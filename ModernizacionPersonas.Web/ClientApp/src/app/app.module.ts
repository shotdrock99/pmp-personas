import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, ErrorHandler, NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatStepperModule } from '@angular/material/stepper';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MdePopoverModule } from '@material-extended/mde';
import { NgSelectModule } from '@ng-select/ng-select';
import { TextMaskModule } from 'angular2-text-mask';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { IConfig, NgxMaskModule } from 'ngx-mask';
import { NgxUpperCaseDirectiveModule } from 'ngx-upper-case-directive';
import { AppRoutingModule } from './app.routing.module';
import { AppComponent } from './app.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { AceptacionCotizacionDialogComponent } from './pages/components/aceptacion-cotizacion/aceptacion-cotizacion.component';
import { CerrarCotizacionDialogComponent } from './pages/components/cerrar-cotizacion/cerrar-cotizacion.component';
import { CotizacionViewerComponent } from './pages/components/cotizacion-viewer/cotizacion-viewer.component';
import { ListaCotizacionesContextMenuComponent } from './pages/components/lista-cotizaciones-context-menu/lista-cotizaciones-context-menu.component';
import { InformacionAseguramientoComponent } from './pages/configuracion-cotizacion/informacion-aseguramiento/informacion-aseguramiento.component';
import { InformacionGeneralComponent } from './pages/configuracion-cotizacion/informacion-general/informacion-general.component';
import { InformacionIntermediariosComponent } from './pages/configuracion-cotizacion/informacion-intermediarios/informacion-intermediarios.component';
import { RegistroSiniestralidadComponent } from './pages/configuracion-cotizacion/informacion-siniestralidad/components/registro-siniestralidad/registro-siniestralidad.component';
import { InformacionSiniestralidadComponent } from './pages/configuracion-cotizacion/informacion-siniestralidad/informacion-siniestralidad.component';
import { InformacionTomadorComponent } from './pages/configuracion-cotizacion/informacion-tomador/informacion-tomador.component';
import { ConsultaCotizacionComponent } from './pages/consulta-cotizacion/consulta-cotizacion.component';
import { CreacionCotizacionComponent } from './pages/creacion-cotizacion/creacion-cotizacion.component';
import { FichaTecnicaInformacionTomadorComponent } from './pages/ficha-tecnica-cotizacion/components/ficha-tecnica-informacion-tomador/ficha-tecnica-informacion-tomador.component';
import { ListaCotizacionesComponent } from './pages/lista-cotizaciones/lista-cotizaciones.component';
import { ResumenCotizacionComponent } from './pages/resumen-cotizacion/resumen-cotizacion.component';
import { AddTokenInterceptor } from './services/add-token-interceptor';
import { AppErrorHandler } from './shared/app.errorhandler';
import { AlertDialogComponent } from './shared/components/alert-dialog';
import { ConfirmDialogComponent } from './shared/components/confirm-dialog';
import { CotizacionTabsComponent } from './shared/components/cotizacion-tabs/cotizacion-tabs.component';
import { InputLoadingComponent } from './shared/components/input-loading.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { InputRestrictionDirective } from './shared/directives/input-restriction.directive';
import { AmparosbasicosPipe } from './shared/pipes/amparosbasicos.pipe';
import { AsistenciasPipe } from './shared/pipes/asistencias.pipe';
import { NoSiniestralidadPipe } from './shared/pipes/nosiniestralidad.pipe';
import { BasiconoadicionalPipe } from './shared/pipes/basiconoadicional.pipe';
import { CharLimitPipe } from './shared/pipes/char-limit.pipe';
import { SoliCurrencyMaskPipe } from './shared/pipes/custom-currency-mask';
import { SoliPercentageMaskPipe } from './shared/pipes/custom-percentage-mask';
import { MaxValueLimitPipe } from './shared/pipes/max-value-limit.pipe';
import { UppercasePipe } from './shared/pipes/uppercase.pipe';
import { ConfiguracionSlipAmparosComponent } from './pages/configuracion-slip-cotizacion/components/configuracion-slip-amparos/configuracion-slip-amparos.component';
import { ConfiguracionSlipClausulasComponent } from './pages/configuracion-slip-cotizacion/components/configuracion-slip-clausulas/configuracion-slip-clausulas.component';
import { SlipAmparosDescriptionComponent } from './pages/slip-cotizacion/components/slip-amparos-description.component';
import { SlipEdadesAsegurabilidadComponent } from './pages/slip-cotizacion/components/slip-edades-asegurabilidad/slip-edades-asegurabilidad.component';
import { SlipGruposAseguradosComponent } from './pages/slip-cotizacion/components/slip-grupos-asegurados/slip-grupos-asegurados.component';
import { SlipSeccionAmparosComponent } from './pages/slip-cotizacion/components/slip-seccion-amparos/slip-seccion-amparos.component';
import { SlipSeccionClausulasComponent } from './pages/slip-cotizacion/components/slip-seccion-clausulas/slip-seccion-clausulas.component';
import { SlipSeccionCondicionesComponent } from './pages/slip-cotizacion/components/slip-seccion-condiciones/slip-seccion-condiciones.component';
import { TablaDocsRequeridosComponent } from './pages/slip-cotizacion/table-templates/tabla-docs-requeridos/tabla-docs-requeridos.component';
import { TablaEdadesAmparosComponent } from './pages/slip-cotizacion/table-templates/tabla-edades-amparos/tabla-edades-amparos.component';
import { TablaIndemnizacionesComponent } from './pages/slip-cotizacion/table-templates/tabla-indemnizaciones/tabla-indemnizaciones.component';
import { TablaInutilizacionComponent } from './pages/slip-cotizacion/table-templates/tabla-inutilizacion/tabla-inutilizacion.component';
import { TablaReqsAsegurabilidadComponent } from './pages/slip-cotizacion/table-templates/tabla-reqs-asegurabilidad/tabla-reqs-asegurabilidad.component';
import { TablaValoresAmparosComponent } from './pages/slip-cotizacion/table-templates/tabla-valores-amparos/tabla-valores-amparos.component';
import { TablaValoresAseguradosDiariosComponent } from './pages/slip-cotizacion/table-templates/tabla-valores-asegurados-diarios/tabla-valores-asegurados-diarios.component';
import { TasaSelectorComponent } from './pages/slip-cotizacion/table-templates/tasa-selector/tasa-selector.component';
import { ValoresAseguradosComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/valores-asegurados/valores-asegurados.component';
import { EdadesingresosamparosComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/edadesingresosamparos/edadesingresosamparos.component';
import { AseguradosComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/asegurados/asegurados.component';
import { OptionPercentComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-percent.component';
import { OptionValueComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-value.component';
import { OptionSalariosComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-salarios.component';
import { OptionPrimaComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-prima.component';
import { OptionDailyValueComponent } from "./pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-daily-value.component";
import { OptionDiasComponent } from "./pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-dias.component";
import { ProfilesComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/asegurados/components/profiles/profiles.component';
import { FichaTecnicaCotizacionComponent } from './pages/ficha-tecnica-cotizacion/ficha-tecnica-cotizacion.component';
import { PreloadedProfilesComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/asegurados/components/preloaded-profiles/preloaded-profiles.component';
import { FichaTecnicaGruposAseguradosComponent } from './pages/ficha-tecnica-cotizacion/components/ficha-tecnica-grupos-asegurados/ficha-tecnica-grupos-asegurados.component';
import { FichaTecnicaPerfilEdadesComponent } from './pages/ficha-tecnica-cotizacion/components/ficha-tecnica-perfil-edades/ficha-tecnica-perfil-edades.component';
import { FichaTecnicaPerfilValoresComponent } from './pages/ficha-tecnica-cotizacion/components/ficha-tecnica-perfil-valores/ficha-tecnica-perfil-valores.component';
import { FichaTecnicaSiniestralidadComponent } from './pages/ficha-tecnica-cotizacion/components/ficha-tecnica-siniestralidad/ficha-tecnica-siniestralidad.component';
import { ConfigurarGrupoAseguradoComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/modal-components/configurar-grupo-asegurado/configurar-grupo-asegurado.component';
import { AgregarGrupoAseguradoComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/modal-components/agregar-grupo-asegurado/agregar-grupo-asegurado.component';
import { InformacionGruposAseguradosComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/informacion-grupos-asegurados.component';
import { SlipCotizacionComponent } from './pages/slip-cotizacion/slip-cotizacion.component';
import { ConfiguracionSlipCotizacionComponent } from './pages/configuracion-slip-cotizacion/configuracion-slip-cotizacion.component';
import { PreventInputDirective } from './shared/directives/prevent-input.directive';
import { UppercaseDirective } from './shared/directives/uppercase.directive';
import { InitialsPipe } from './shared/pipes/initials.pipe';
import { FilterListPipe } from './shared/pipes/filter-list.pipe';
import { TrimZeroPipe } from './shared/pipes/trimzero.pipe';
import { RechazoAutorizacionCotizacionComponent } from './pages/components/rechazo-autorizacion-cotizacion/rechazo-autorizacion-cotizacion.component';
import { InformacionAutorizacionesCotizacionComponent } from './pages/components/informacion-autorizaciones-cotizacion/informacion-autorizaciones-cotizacion.component';
import { ListaCotizacionesAutorizacionComponent } from './pages/lista-cotizaciones-autorizacion/lista-cotizaciones-autorizacion.component';
import { ConfigurarIntermediarioComponent } from './pages/configuracion-cotizacion/informacion-intermediarios/components/configurar-intermediario/configurar-intermediario.component';
import { ContinuarCotizacionComponent } from './pages/components/continuar-cotizacion/continuar-cotizacion.component';
import { CotizacionesFilterPanelComponent } from './pages/components/cotizaciones-filter-panel/cotizaciones-filter-panel.component';
import { PageToolbarComponent } from './pages/components/page-toolbar/page-toolbar.component';
import { AlertSnackComponent } from './shared/components/alert-snack/alert-snack.component';
import { CotizacionHistoryTimelineComponent } from './pages/components/cotizacion-history-timeline/cotizacion-history-timeline.component';
import { SendSlipCotizacionComponent } from './pages/components/send-slip-cotizacion/send-slip-cotizacion.component';
import { EditarNombreGrupoAseguradoComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/modal-components/editar-nombre-grupo-asegurado/editar-nombre-grupo-asegurado.component';
import { AuthorizationControlListComponent } from './pages/components/authorization-control-list/authorization-control-list.component';
import { ConfiguracionSlipAsegurabilidadComponent } from './pages/configuracion-slip-cotizacion/components/configuracion-slip-asegurabilidad/configuracion-slip-asegurabilidad.component';
import { AuthorizeCotizacionComponent } from './pages/authorize-cotizacion/authorize-cotizacion.component';
import { AuthorizationTransactionsModalComponent } from './pages/components/authorization-transactions-modal/authorization-transactions-modal.component';
import { CausalParametricasComponent } from './pages/parametrizacion/causal-parametricas/causal-parametricas.component';
import { CausalEditComponent } from './pages/parametrizacion/causal-parametricas/components/causal-edit/causal-edit.component';
import { CausalCrearComponent } from './pages/parametrizacion/causal-parametricas/components/causal-crear/causal-crear.component';
import { UsuariosParametricasComponent } from './pages/parametrizacion/usuarios-parametricas/usuarios-parametricas.component';
import { UsuarioEditarComponent } from './pages/parametrizacion/usuarios-parametricas/components/usuario-editar/usuario-editar.component';
import { ReadonlyCotizacionToastComponent } from './shared/components/readonly-cotizacion-toast/readonly-cotizacion-toast.component';
import { UsuarioCrearComponent } from './pages/parametrizacion/usuarios-parametricas/components/usuario-crear/usuario-crear.component';
import { NavComponent } from './components/nav/nav.component';
import { SeccionesSlipParametricasComponent } from './pages/parametrizacion/secciones-slip-parametricas/secciones-slip-parametricas.component';
import { RolesParametricasComponent } from './pages/parametrizacion/roles-parametricas/roles-parametricas.component';
import { RolEditarComponent } from './pages/parametrizacion/roles-parametricas/components/rol-editar/rol-editar.component';
import { RolCrearComponent } from './pages/parametrizacion/roles-parametricas/components/rol-crear/rol-crear.component';
import { SeccionSlipEditarComponent } from './pages/parametrizacion/secciones-slip-parametricas/components/seccion-slip-editar/seccion-slip-editar.component';
import { SeccionSlipCrearComponent } from './pages/parametrizacion/secciones-slip-parametricas/components/seccion-slip-crear/seccion-slip-crear.component';
import { ParametrizacionComponent } from './pages/parametrizacion/parametrizacion.component';
import { VariablesSlipParametricasComponent } from './pages/parametrizacion/variables-slip-parametricas/variables-slip-parametricas.component';
import { VariableSlipEditarComponent } from './pages/parametrizacion/variables-slip-parametricas/components/variable-slip-editar/variable-slip-editar.component';
import { VariableSlipCrearComponent } from './pages/parametrizacion/variables-slip-parametricas/components/variable-slip-crear/variable-slip-crear.component';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { TextosSlipParametricasComponent } from './pages/parametrizacion/textos-slip-parametricas/textos-slip-parametricas.component';
import { TextoSlipEditorComponent } from './pages/parametrizacion/textos-slip-parametricas/components/texto-slip-editor/texto-slip-editor.component';
import { CreateTextoSlipComponent } from './pages/parametrizacion/textos-slip-parametricas/components/create-texto-slip/create-texto-slip.component';
import { EmailParametricasComponent } from './pages/parametrizacion/email-parametricas/email-parametricas.component';
import { EmailEditarComponent } from './pages/parametrizacion/email-parametricas/components/email-editar/email-editar.component';
import { VariablesGlobalesParametricasComponent } from './pages/parametrizacion/variables-globales-parametricas/variables-globales-parametricas.component';
import { IntermediarioCrearComponent } from './pages/parametrizacion/usuarios-parametricas/components/intermediario-crear/intermediario-crear.component';
import { NonEmptyRangosEdadesPipe } from './shared/pipes/nonemptyrangosedades.pipe';
import { AlertDialogPreventCloseComponent } from './shared/components/alert-dialog/alert-dialog.preventClose.component';
import { GenerateExpedicionWebComponent } from './pages/components/generate-expedicion-web/generate-expedicion-web.component';
import { ValoresAseguradosModalidadesComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/valores-asegurados/valores-asegurados-modalidades/valores-asegurados-modalidades.component';
import { OptionValueDDComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/form-control-components/option-value-dd.component';
import { ValoresAseguradosNormalesComponent } from './pages/configuracion-cotizacion/informacion-grupos-asegurados/components/partial-components/valores-asegurados/valores-asegurados-normales/valores-asegurados-normales.component';
import { SlipMaximoValorAseguradoIndividualComponent } from './pages/slip-cotizacion/components/slip-maximo-valor-asegurado-individual/slip-maximo-valor-asegurado-individual.component';

export const options: Partial<IConfig> | (() => Partial<IConfig>) = {};

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    CotizacionViewerComponent,
    InformacionAseguramientoComponent,
    InformacionIntermediariosComponent,
    InformacionGruposAseguradosComponent,
    ConfirmDialogComponent,
    AlertDialogComponent,
    ValoresAseguradosComponent,
    EdadesingresosamparosComponent,
    AseguradosComponent,
    CreacionCotizacionComponent,
    ConsultaCotizacionComponent,
    PreventInputDirective,
    UppercaseDirective,
    UppercasePipe,
    CharLimitPipe,
    SoliPercentageMaskPipe,
    SoliCurrencyMaskPipe,
    OptionPercentComponent,
    OptionValueComponent,
    OptionSalariosComponent,
    OptionPrimaComponent,
    OptionDailyValueComponent,
    OptionDiasComponent,
    OptionValueDDComponent,
    ProfilesComponent,
    PageNotFoundComponent,
    MaxValueLimitPipe,
    InputRestrictionDirective,
    InputLoadingComponent,
    ResumenCotizacionComponent,
    BasiconoadicionalPipe,
    RegistroSiniestralidadComponent,
    FichaTecnicaCotizacionComponent,
    InformacionTomadorComponent,
    FichaTecnicaGruposAseguradosComponent,
    FichaTecnicaPerfilEdadesComponent,
    FichaTecnicaPerfilValoresComponent,
    PreloadedProfilesComponent,
    FichaTecnicaSiniestralidadComponent,
    AmparosbasicosPipe,
    NonEmptyRangosEdadesPipe,
    AsistenciasPipe,
    NoSiniestralidadPipe,
    TablaIndemnizacionesComponent,
    TablaInutilizacionComponent,
    TablaReqsAsegurabilidadComponent,
    TablaDocsRequeridosComponent,
    ConfiguracionSlipClausulasComponent,
    ConfiguracionSlipAmparosComponent,
    SlipEdadesAsegurabilidadComponent,
    SlipAmparosDescriptionComponent,
    TablaEdadesAmparosComponent,
    TablaValoresAmparosComponent,
    SlipSeccionAmparosComponent,
    SlipSeccionClausulasComponent,
    SlipSeccionCondicionesComponent,
    TasaSelectorComponent,
    SlipGruposAseguradosComponent,
    AceptacionCotizacionDialogComponent,
    CerrarCotizacionDialogComponent,
    InformacionGeneralComponent,
    CotizacionTabsComponent,
    FichaTecnicaInformacionTomadorComponent,
    ListaCotizacionesComponent,
    ListaCotizacionesContextMenuComponent,
    InformacionIntermediariosComponent,
    InformacionSiniestralidadComponent,
    ConfigurarGrupoAseguradoComponent,
    AgregarGrupoAseguradoComponent,
    SlipCotizacionComponent,
    ConfiguracionSlipCotizacionComponent,
    InitialsPipe,
    FilterListPipe,
    TrimZeroPipe,
    RechazoAutorizacionCotizacionComponent,
    InformacionAutorizacionesCotizacionComponent,
    ListaCotizacionesAutorizacionComponent,
    ConfigurarIntermediarioComponent,
    ContinuarCotizacionComponent,
    CotizacionesFilterPanelComponent,
    PageToolbarComponent,
    AlertSnackComponent,
    CotizacionHistoryTimelineComponent,
    SendSlipCotizacionComponent,
    EditarNombreGrupoAseguradoComponent,
    AuthorizationControlListComponent,
    ConfiguracionSlipAsegurabilidadComponent,
    AuthorizeCotizacionComponent,
    AuthorizationTransactionsModalComponent,
    CausalParametricasComponent,
    CausalCrearComponent,
    CausalEditComponent,
    UsuariosParametricasComponent,
    UsuarioEditarComponent,
    ReadonlyCotizacionToastComponent,
    UsuarioCrearComponent,
    NavComponent,
    RolesParametricasComponent,
    RolEditarComponent,
    RolCrearComponent,
    SeccionesSlipParametricasComponent,
    SeccionSlipEditarComponent,
    SeccionSlipCrearComponent,
    ParametrizacionComponent,
    VariablesSlipParametricasComponent,
    VariableSlipEditarComponent,
    VariableSlipCrearComponent,
    UserMenuComponent,
    EmailParametricasComponent,
    EmailEditarComponent,
    TextosSlipParametricasComponent,
    TextoSlipEditorComponent,
    CreateTextoSlipComponent,
    IntermediarioCrearComponent,
    VariablesGlobalesParametricasComponent,
    AlertDialogPreventCloseComponent,
    GenerateExpedicionWebComponent,
    ValoresAseguradosModalidadesComponent,
    ValoresAseguradosNormalesComponent,
    SlipMaximoValorAseguradoIndividualComponent,
    TablaValoresAseguradosDiariosComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    MatInputModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCheckboxModule,
    MatAutocompleteModule,
    MatSelectModule,
    MatStepperModule,
    MatListModule,
    MatCardModule,
    MatExpansionModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressBarModule,
    MatDialogModule,
    MatTooltipModule,
    MatRadioModule,
    MatTabsModule,
    MatSnackBarModule,
    MatDividerModule,
    MatTableModule,
    MatPaginatorModule,
    MatToolbarModule,
    MatMenuModule,
    MatSidenavModule,
    TextMaskModule,
    NgSelectModule,
    CurrencyMaskModule,
    FlexLayoutModule,
    NgxUpperCaseDirectiveModule,
    MdePopoverModule,
    NgxMaskModule.forRoot(options),
    // NgxMaskModule.forChild(),
    AppRoutingModule
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  entryComponents: [
    ConfigurarIntermediarioComponent,
    AgregarGrupoAseguradoComponent,
    ConfigurarGrupoAseguradoComponent,
    EditarNombreGrupoAseguradoComponent,
    ConfirmDialogComponent,
    AlertDialogComponent,
    TasaSelectorComponent,
    RechazoAutorizacionCotizacionComponent,
    AceptacionCotizacionDialogComponent,
    CerrarCotizacionDialogComponent,
    AuthorizationTransactionsModalComponent,
    ContinuarCotizacionComponent,
    AlertSnackComponent,
    SendSlipCotizacionComponent,
    CausalEditComponent,
    CausalCrearComponent,
    UsuarioEditarComponent,
    UsuarioCrearComponent,
    IntermediarioCrearComponent,
    RolEditarComponent,
    RolCrearComponent,
    SeccionSlipEditarComponent,
    SeccionSlipCrearComponent,
    VariableSlipEditarComponent,
    VariableSlipCrearComponent,
    TextoSlipEditorComponent,
    CreateTextoSlipComponent,
    EmailEditarComponent,
    AlertDialogPreventCloseComponent,
    GenerateExpedicionWebComponent
  ],
  providers: [
    { provide: ErrorHandler, useClass: AppErrorHandler },
    { provide: HTTP_INTERCEPTORS, useClass: AddTokenInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }
