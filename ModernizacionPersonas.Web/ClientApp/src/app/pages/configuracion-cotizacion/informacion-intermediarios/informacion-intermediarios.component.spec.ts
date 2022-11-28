import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { InformacionIntermediariosComponent } from './informacion-intermediarios.component';

describe('InformacionIntermediariosComponent', () => {
  let component: InformacionIntermediariosComponent;
  let fixture: ComponentFixture<InformacionIntermediariosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacionIntermediariosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacionIntermediariosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
