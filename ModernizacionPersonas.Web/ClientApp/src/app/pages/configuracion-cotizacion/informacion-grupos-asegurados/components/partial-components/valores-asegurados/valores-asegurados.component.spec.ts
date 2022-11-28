import { HttpClientModule } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TipoSumaAsegurada } from 'src/app/models';
import { ValoresAseguradosComponent } from './valores-asegurados.component';

describe('ValoresAseguradosComponent', () => {
  let component: ValoresAseguradosComponent;
  let fixture: ComponentFixture<ValoresAseguradosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ValoresAseguradosComponent],
      imports: [
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        MatInputModule,
        MatFormFieldModule,
        MatTooltipModule,
        MatTableModule,
        MatPaginatorModule,
        BrowserAnimationsModule
      ],
      schemas: [
        CUSTOM_ELEMENTS_SCHEMA
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ValoresAseguradosComponent);
    component = fixture.componentInstance;

    let tipoSumaAsegurada: TipoSumaAsegurada = {
      codigoTipoSumaAsegurada: 1,
      nombreTipoSumaAsegurada: '',
      disabled: false
    };

    component.tipoSumaAsegurada = tipoSumaAsegurada;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
