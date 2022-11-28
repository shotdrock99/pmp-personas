import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuthorizationControlListComponent } from './authorization-control-list.component';

describe('AuthorizationControlListComponent', () => {
  let component: AuthorizationControlListComponent;
  let fixture: ComponentFixture<AuthorizationControlListComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuthorizationControlListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthorizationControlListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
