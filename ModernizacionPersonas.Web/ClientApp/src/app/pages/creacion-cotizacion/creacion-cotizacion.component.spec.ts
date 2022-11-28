import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CharLimitPipe } from 'src/app/shared/pipes/char-limit.pipe';
import { CotizacionViewerComponent } from '../components/cotizacion-viewer/cotizacion-viewer.component';
import { InformacionAseguramientoComponent } from '../configuracion-cotizacion/informacion-aseguramiento/informacion-aseguramiento.component';
import { InformacionGruposAseguradosComponent } from '../configuracion-cotizacion/informacion-grupos-asegurados/informacion-grupos-asegurados.component';
import { InformacionIntermediariosComponent } from '../configuracion-cotizacion/informacion-intermediarios/informacion-intermediarios.component';
import { CreacionCotizacionComponent } from './creacion-cotizacion.component';

describe('CreacionCotizacionComponent', () => {
  let component: CreacionCotizacionComponent;
  let fixture: ComponentFixture<CreacionCotizacionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [
        CreacionCotizacionComponent,
        CotizacionViewerComponent,
        InformacionAseguramientoComponent,
        InformacionIntermediariosComponent,
        InformacionGruposAseguradosComponent,
        CharLimitPipe
      ],
      imports: [
        HttpClientModule,
        MatExpansionModule,
        MatProgressBarModule,
        MatIconModule,
        MatAutocompleteModule,
        FormsModule,
        ReactiveFormsModule,
        MatInputModule,
        MatFormFieldModule,
        MatTooltipModule,
        MatDatepickerModule,
        MatSelectModule,
        MatCheckboxModule,
        MatTableModule,
        MatPaginatorModule,
        BrowserAnimationsModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreacionCotizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
