import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipSeccionCondicionesComponent } from './slip-seccion-condiciones.component';

describe('SlipSeccionCondicionesComponent', () => {
  let component: SlipSeccionCondicionesComponent;
  let fixture: ComponentFixture<SlipSeccionCondicionesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipSeccionCondicionesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipSeccionCondicionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
