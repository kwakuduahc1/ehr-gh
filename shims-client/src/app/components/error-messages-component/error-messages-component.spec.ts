import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ErrorMessagesComponent } from './error-messages-component';

describe('ErrorMessagesComponent', () => {
  let component: ErrorMessagesComponent;
  let fixture: ComponentFixture<ErrorMessagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ErrorMessagesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ErrorMessagesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
