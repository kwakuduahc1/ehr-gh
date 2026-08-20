import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultDiagnosesComponent } from './consult-diagnoses';

describe('ConsultDiagnosesComponent', () => {
    let component: ConsultDiagnosesComponent;
    let fixture: ComponentFixture<ConsultDiagnosesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultDiagnosesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultDiagnosesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
