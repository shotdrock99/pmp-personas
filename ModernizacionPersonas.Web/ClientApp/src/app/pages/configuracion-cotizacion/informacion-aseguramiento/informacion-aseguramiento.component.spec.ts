import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CharLimitPipe } from '../../../shared/pipes/char-limit.pipe';
import { InformacionAseguramientoComponent } from './informacion-aseguramiento.component';

describe('InformacionAseguramientoComponent', () => {
  let component: InformacionAseguramientoComponent;
  let fixture: ComponentFixture<InformacionAseguramientoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [
        InformacionAseguramientoComponent,
        CharLimitPipe
      ],
      imports: [
        HttpClientModule,
        FormsModule,
        ReactiveFormsModule,
        MatInputModule,
        MatFormFieldModule,
        MatTooltipModule,
        MatTableModule,
        MatPaginatorModule,
        MatAutocompleteModule,
        BrowserAnimationsModule,
        MatDatepickerModule,
        MatSelectModule,
        MatCheckboxModule
      ],
      schemas: [
        CUSTOM_ELEMENTS_SCHEMA
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionAseguramientoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
