import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionGeneralComponent } from './informacion-general.component';
import { CharLimitPipe } from 'src/app/shared/pipes/char-limit.pipe';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('InformacionGeneralComponent', () => {
  let component: InformacionGeneralComponent;
  let fixture: ComponentFixture<InformacionGeneralComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [
        InformacionGeneralComponent,
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
        MatDatepickerModule
      ]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
