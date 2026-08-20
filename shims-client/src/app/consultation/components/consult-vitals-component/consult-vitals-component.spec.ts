import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultVitalsComponent } from './consult-vitals-component';

describe('ConsultVitalsComponent', () => {
    let component: ConsultVitalsComponent;
    let fixture: ComponentFixture<ConsultVitalsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultVitalsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultVitalsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
