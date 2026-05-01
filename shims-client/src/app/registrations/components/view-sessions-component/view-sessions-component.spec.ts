import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewSessionsComponent } from './view-sessions-component';

describe('ViewSessionsComponent', () => {
  let component: ViewSessionsComponent;
  let fixture: ComponentFixture<ViewSessionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewSessionsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ViewSessionsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
