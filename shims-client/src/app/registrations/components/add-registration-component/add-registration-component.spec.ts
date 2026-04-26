import { ComponentFixture, TestBed } from '@angular/core/testing';

describe('AddRegistrationComponent', () => {
  let component: AddRegistrationComponent;
  let fixture: ComponentFixture<AddRegistrationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddRegistrationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AddRegistrationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
