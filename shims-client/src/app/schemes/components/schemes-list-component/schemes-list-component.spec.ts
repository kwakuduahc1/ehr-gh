import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchemesComponent } from './schemes-component';

describe('SchemesComponent', () => {
  let component: SchemesComponent;
  let fixture: ComponentFixture<SchemesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SchemesComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SchemesComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
