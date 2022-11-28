import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipGruposAseguradosComponent } from './slip-grupos-asegurados.component';

describe('SlipGruposAseguradosComponent', () => {
  let component: SlipGruposAseguradosComponent;
  let fixture: ComponentFixture<SlipGruposAseguradosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipGruposAseguradosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipGruposAseguradosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
