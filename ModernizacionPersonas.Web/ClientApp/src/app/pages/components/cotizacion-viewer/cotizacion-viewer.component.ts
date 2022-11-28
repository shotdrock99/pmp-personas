import { Component, EventEmitter, HostListener, Input, OnInit, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Cotizacion, CotizacionState } from '../../../models';
import { CotizacionPersistenceService } from '../../../services/cotizacion-persistence.service';
import { NavegacionService } from '../../../services/navegacion.service';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-cotizacion-viewer',
  templateUrl: './cotizacion-viewer.component.html',
  styleUrls: ['./cotizacion-viewer.component.scss']
})
export class CotizacionViewerComponent implements OnInit {
  indexView = 0;
  sections = this.navigationService.sections;

  esNegocioDirecto: boolean;
  tieneTasaSiniestralidad = false;
  anyosSiniestralidadSubject = new BehaviorSubject<number>(0);

  @Input() data: Cotizacion;

  @Input() readonly: boolean;

  @Output() updateInitialValues = new EventEmitter();

  constructor(
    public dialog: MatDialog,
    private cotizacionDataService: CotizacionPersistenceService,
    private navigationService: NavegacionService) { }

  @HostListener('window:unload', ['$event'])
  unloadHandler(event) {

  }

  @HostListener('window:beforeunload', ['$event'])
  beforeUnloadHandler(event) {
    const confirmationMessage = '\o/';
    // tslint:disable-next-line: deprecation
    (event || window.event).returnValue = confirmationMessage; // Gecko + IE
    return confirmationMessage;
  }

  get numeroCotizacion() {
    return this.cotizacionDataService.cotizacion.numero;
  }

  get progress() {
    return this.navigationService.progress;
  }

  get btnInformacionBasicaText(): string {
    if (this.cotizacionDataService.isEdit) {
      return 'Siguiente';
    }

    return 'Crear cotización';
  }

  get mostrarAutorizarCotizacion(): boolean {
    // muestra el componente de autorizaciones si corresponde el estado y si el usuario tiene permisos
    return this.data.estado === CotizacionState.PendingAuthorization;
    // && this.userManager.currentUser.role.id === 5;
  }

  ngOnInit() {
    this.navigationService.reset();
    
    this.tieneTasaSiniestralidad = this.cotizacionDataService.hasSiniestralidad;
    this.registerIndexViewObservable();
  }

  async nextStep(e: Event) {
    const btn = e.currentTarget as Element;
    const isValid = await this.navigationService.validateNavigate('next', this.indexView);
    if (isValid) {
      btn.setAttribute('disabled', 'disabled');
      const key = `s${this.indexView}`;
      // this.sections[key].completed = true;

      const isLastStep = !this.tieneTasaSiniestralidad && this.indexView === 4 || this.tieneTasaSiniestralidad && this.indexView === 5;

      this.indexView++;
      if (this.esNegocioDirecto && this.indexView === 3) {
        this.indexView++;
      }

      if (isLastStep) {
        this.navigationService.completeCotizacion();
      }

      btn.removeAttribute('disabled');
    }
  }

  prevStep() {
    const isValid = this.navigationService.validateNavigate('back', this.indexView);

    if (isValid) {
      this.indexView--;
      if (this.indexView === 3 && this.esNegocioDirecto) {
        this.indexView--;
      }
    }
  }

  OnInformacionNegocioChange(args): void {
    this.esNegocioDirecto = args.value;
  }

  OnSiniestralidadChange(args): void {
    this.tieneTasaSiniestralidad = args.value1 === 5 || args.value2 === 5;
  }

  OnAnyosSiniestralidadChange(anyos: number): void {
    this.anyosSiniestralidadSubject.next(anyos);
  }

  private registerIndexViewObservable() {
    setTimeout(() => {
      this.indexView = this.navigationService.activeSectionIndex;
    });
  }

  updateValues(event: any) {
    this.updateInitialValues.emit(event.codigoTipoSuma);
  }
}
