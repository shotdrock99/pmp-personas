import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipMaximoValorAseguradoIndividualComponent } from './slip-maximo-valor-asegurado-individual.component';

describe('SlipMaximoValorAseguradoIndividualComponent', () => {
  let component: SlipMaximoValorAseguradoIndividualComponent;
  let fixture: ComponentFixture<SlipMaximoValorAseguradoIndividualComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipMaximoValorAseguradoIndividualComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipMaximoValorAseguradoIndividualComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
