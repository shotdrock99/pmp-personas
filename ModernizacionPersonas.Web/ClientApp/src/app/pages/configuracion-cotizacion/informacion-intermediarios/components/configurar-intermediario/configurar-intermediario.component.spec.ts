import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ConfigurarIntermediarioComponent } from './configurar-intermediario.component';

describe('ConfigurarIntermediarioComponent', () => {
  let component: ConfigurarIntermediarioComponent;
  let fixture: ComponentFixture<ConfigurarIntermediarioComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigurarIntermediarioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigurarIntermediarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
