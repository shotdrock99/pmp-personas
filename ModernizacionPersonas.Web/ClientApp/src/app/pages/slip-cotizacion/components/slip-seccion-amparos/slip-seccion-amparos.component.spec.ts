import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SlipSeccionAmparosComponent } from './slip-seccion-amparos.component';

describe('SlipSecionAmparosComponent', () => {
  let component: SlipSeccionAmparosComponent;
  let fixture: ComponentFixture<SlipSeccionAmparosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SlipSeccionAmparosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SlipSeccionAmparosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
