import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CotizacionHistoryTimelineComponent } from './cotizacion-history-timeline.component';

describe('CotizacionHistoryTimelineComponent', () => {
  let component: CotizacionHistoryTimelineComponent;
  let fixture: ComponentFixture<CotizacionHistoryTimelineComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CotizacionHistoryTimelineComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CotizacionHistoryTimelineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
