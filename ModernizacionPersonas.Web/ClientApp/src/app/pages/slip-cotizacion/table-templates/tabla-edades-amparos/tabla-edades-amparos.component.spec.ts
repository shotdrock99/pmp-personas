import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TablaEdadesAmparosComponent } from './tabla-edades-amparos.component';

describe('TablaEdadesAmparosComponent', () => {
  let component: TablaEdadesAmparosComponent;
  let fixture: ComponentFixture<TablaEdadesAmparosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TablaEdadesAmparosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TablaEdadesAmparosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
