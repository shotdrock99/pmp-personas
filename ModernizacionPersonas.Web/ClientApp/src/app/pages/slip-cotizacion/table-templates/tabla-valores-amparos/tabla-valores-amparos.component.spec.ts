import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TablaValoresAmparosComponent } from './tabla-valores-amparos.component';

describe('TablaValoresAmparosComponent', () => {
  let component: TablaValoresAmparosComponent;
  let fixture: ComponentFixture<TablaValoresAmparosComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TablaValoresAmparosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TablaValoresAmparosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
