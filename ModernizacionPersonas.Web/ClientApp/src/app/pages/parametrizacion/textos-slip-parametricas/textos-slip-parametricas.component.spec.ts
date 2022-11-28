import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TextosSlipParametricasComponent } from './textos-slip-parametricas.component';

describe('TextosSlipParametricasComponent', () => {
  let component: TextosSlipParametricasComponent;
  let fixture: ComponentFixture<TextosSlipParametricasComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TextosSlipParametricasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TextosSlipParametricasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
