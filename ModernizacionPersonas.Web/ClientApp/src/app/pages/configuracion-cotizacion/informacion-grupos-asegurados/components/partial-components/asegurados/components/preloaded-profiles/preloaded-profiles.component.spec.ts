import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PreloadedProfilesComponent } from './preloaded-profiles.component';

describe('PreloadedProfilesComponent', () => {
  let component: PreloadedProfilesComponent;
  let fixture: ComponentFixture<PreloadedProfilesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PreloadedProfilesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PreloadedProfilesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
