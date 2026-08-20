import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultInvestigationsComponent } from './consult-investigations';

describe('ConsultInvestigationsComponent', () => {
    let component: ConsultInvestigationsComponent;
    let fixture: ComponentFixture<ConsultInvestigationsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultInvestigationsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultInvestigationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
