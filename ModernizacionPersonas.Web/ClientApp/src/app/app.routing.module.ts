import { TextosSlipParametricasComponent } from './pages/parametrizacion/textos-slip-parametricas/textos-slip-parametricas.component';
import { UsuariosParametricasComponent } from './pages/parametrizacion/usuarios-parametricas/usuarios-parametricas.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizeCotizacionComponent } from './pages/authorize-cotizacion/authorize-cotizacion.component';
import { CotizacionHistoryTimelineComponent } from './pages/components/cotizacion-history-timeline/cotizacion-history-timeline.component';
import { ConfiguracionSlipCotizacionComponent } from './pages/configuracion-slip-cotizacion/configuracion-slip-cotizacion.component';
import { ConsultaCotizacionComponent } from './pages/consulta-cotizacion/consulta-cotizacion.component';
import { CreacionCotizacionComponent } from './pages/creacion-cotizacion/creacion-cotizacion.component';
import { FichaTecnicaCotizacionComponent } from './pages/ficha-tecnica-cotizacion/ficha-tecnica-cotizacion.component';
import { ListaCotizacionesAutorizacionComponent } from './pages/lista-cotizaciones-autorizacion/lista-cotizaciones-autorizacion.component';
import { ListaCotizacionesComponent } from './pages/lista-cotizaciones/lista-cotizaciones.component';
import { PageNotFoundComponent } from './pages/page-not-found/page-not-found.component';
import { CausalParametricasComponent } from './pages/parametrizacion/causal-parametricas/causal-parametricas.component';
import { ResumenCotizacionComponent } from './pages/resumen-cotizacion/resumen-cotizacion.component';
import { SlipCotizacionComponent } from './pages/slip-cotizacion/slip-cotizacion.component';
import { CotizacionDataResolver } from './resolvers/cotizacion-data-resolver.service';
import { AuthGuard } from './services/guards/auth-guard.service';
import { CanDeactivateGuard } from './services/guards/can-deactivate.guard';
import { UnlockCotizacionGuard } from './services/guards/unlock-cotizacion.guard';
import { RolesParametricasComponent } from './pages/parametrizacion/roles-parametricas/roles-parametricas.component';
import { SeccionesSlipParametricasComponent } from './pages/parametrizacion/secciones-slip-parametricas/secciones-slip-parametricas.component';
import { ParametrizacionComponent } from './pages/parametrizacion/parametrizacion.component';
import { VariablesSlipParametricasComponent } from './pages/parametrizacion/variables-slip-parametricas/variables-slip-parametricas.component';
import { EmailParametricasComponent } from './pages/parametrizacion/email-parametricas/email-parametricas.component';
import { VariablesGlobalesParametricasComponent } from './pages/parametrizacion/variables-globales-parametricas/variables-globales-parametricas.component';

const routes: Routes = [
  {
    path: 'parametrizacion',
    canActivate: [AuthGuard],
    component: ParametrizacionComponent
  },
  {
    path: 'parametrizacion/variablesGlobales',
    canActivate: [AuthGuard],
    component: VariablesGlobalesParametricasComponent
  },
  {
    path: 'parametrizacion/emails',
    canActivate: [AuthGuard],
    component: EmailParametricasComponent
  },
  {
    path: 'parametrizacion/causales',
    canActivate: [AuthGuard],
    component: CausalParametricasComponent
  },
  {
    path: 'parametrizacion/usuarios',
    canActivate: [AuthGuard],
    component: UsuariosParametricasComponent
  },
  {
    path: 'parametrizacion/roles',
    canActivate: [AuthGuard],
    component: RolesParametricasComponent
  },
  {
    path: 'parametrizacion/slip/secciones',
    canActivate: [AuthGuard],
    component: SeccionesSlipParametricasComponent
  },
  {
    path: 'parametrizacion/slip/variables',
    canActivate: [AuthGuard],
    component: VariablesSlipParametricasComponent
  },
  {
    path: 'parametrizacion/slip/textos',
    canActivate: [AuthGuard],
    component: TextosSlipParametricasComponent
  },
  {
    path: 'cotizaciones',
    canActivate: [AuthGuard],
    component: ListaCotizacionesComponent
  },
  {
    path: 'cotizaciones/nueva',
    pathMatch: 'full',
    component: CreacionCotizacionComponent,
    canActivate: [AuthGuard],
    canDeactivate: [CanDeactivateGuard],
    data: {
      navigationTabIndex: 1,
      resolveDependencies: true
    },
    resolve: {
      data: CotizacionDataResolver
    },
    runGuardsAndResolvers: 'always',
  },
  {
    path: 'cotizaciones/:cod_cotiza',
    canActivate: [AuthGuard],
    canDeactivate: [UnlockCotizacionGuard],
    component: ConsultaCotizacionComponent,
    data: {
      navigationTabIndex: 1,
      resolveDependencies: true
    },
    resolve: {
      data: CotizacionDataResolver
    }
  },
  {
    path: 'cotizaciones/:cod_cotiza/authorize',
    component: AuthorizeCotizacionComponent
  },
  {
    path: '',
    redirectTo: 'cotizaciones',
    pathMatch: 'full'
  },
  {
    path: 'cotizaciones/:cod_cotiza/resumen',
    component: ResumenCotizacionComponent,
    canDeactivate: [UnlockCotizacionGuard],
    data: {
      navigationTabIndex: 2,
      resolveDependencies: false
    },
    resolve: {
      data: CotizacionDataResolver
    }
  },
  {
    path: 'cotizaciones/:cod_cotiza/fichatecnica',
    component: FichaTecnicaCotizacionComponent,
    canDeactivate: [UnlockCotizacionGuard],
    data: {
      navigationTabIndex: 3,
      resolveDependencies: false
    },
    resolve: {
      data: CotizacionDataResolver
    }
  },
  {
    path: 'cotizaciones/:cod_cotiza/slip/config',
    component: ConfiguracionSlipCotizacionComponent,
    canDeactivate: [UnlockCotizacionGuard],
    data: {
      navigationTabIndex: 4,
      resolveDependencies: false
    },
    resolve: {
      data: CotizacionDataResolver
    }
  },
  {
    path: 'cotizaciones/:cod_cotiza/slip/preview',
    component: SlipCotizacionComponent,
    canDeactivate: [UnlockCotizacionGuard],
    data: {
      navigationTabIndex: 5,
      resolveDependencies: false
    },
    resolve: {
      data: CotizacionDataResolver
    }
  },
  {
    path: 'cotizaciones/:cod_cotiza/timeline',
    component: CotizacionHistoryTimelineComponent,
    data: {
      resolveDependencies: false
    }
  },
  {
    path: 'autorizaciones',
    component: ListaCotizacionesAutorizacionComponent
  },
  {
    path: '**',
    component: PageNotFoundComponent
  }];
@NgModule({
  imports: [RouterModule.forRoot(routes, {
    onSameUrlNavigation: 'reload',
    relativeLinkResolution: 'legacy'
})],
  exports: [RouterModule],
  providers: [CotizacionDataResolver]
})
export class AppRoutingModule { }
