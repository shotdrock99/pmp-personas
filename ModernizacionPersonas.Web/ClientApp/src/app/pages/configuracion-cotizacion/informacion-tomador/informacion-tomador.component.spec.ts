import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionTomadorComponent } from './informacion-tomador.component';

describe('InformacionTomadorComponent', () => {
  let component: InformacionTomadorComponent;
  let fixture: ComponentFixture<InformacionTomadorComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacionTomadorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionTomadorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
