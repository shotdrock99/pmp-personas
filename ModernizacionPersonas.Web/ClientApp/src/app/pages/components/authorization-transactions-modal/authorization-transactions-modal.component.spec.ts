import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuthorizationTransactionsModalComponent } from './authorization-transactions-modal.component';

describe('AuthorizationTransactionsModalComponent', () => {
  let component: AuthorizationTransactionsModalComponent;
  let fixture: ComponentFixture<AuthorizationTransactionsModalComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuthorizationTransactionsModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuthorizationTransactionsModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
