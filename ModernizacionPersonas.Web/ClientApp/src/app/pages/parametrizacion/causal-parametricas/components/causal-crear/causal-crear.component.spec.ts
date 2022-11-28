import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CausalCrearComponent } from './causal-crear.component';

describe('CausalCrearComponent', () => {
  let component: CausalCrearComponent;
  let fixture: ComponentFixture<CausalCrearComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CausalCrearComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CausalCrearComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
