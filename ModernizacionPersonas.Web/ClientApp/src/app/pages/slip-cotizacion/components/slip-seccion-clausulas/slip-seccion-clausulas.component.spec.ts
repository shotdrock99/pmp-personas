import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipSeccionClausulasComponent } from './slip-seccion-clausulas.component';

describe('SlipSeccionClausulasComponent', () => {
  let component: SlipSeccionClausulasComponent;
  let fixture: ComponentFixture<SlipSeccionClausulasComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipSeccionClausulasComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipSeccionClausulasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
