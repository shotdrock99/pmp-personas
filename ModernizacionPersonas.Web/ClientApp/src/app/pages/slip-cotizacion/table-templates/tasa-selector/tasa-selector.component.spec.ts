import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TasaSelectorComponent } from './tasa-selector.component';

describe('TasaSelectorComponent', () => {
  let component: TasaSelectorComponent;
  let fixture: ComponentFixture<TasaSelectorComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TasaSelectorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TasaSelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
