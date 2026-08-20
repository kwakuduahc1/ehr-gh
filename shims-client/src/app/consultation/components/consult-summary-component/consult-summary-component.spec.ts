import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultSummaryComponent } from './consult-summary-component';

describe('ConsultSummaryComponent', () => {
    let component: ConsultSummaryComponent;
    let fixture: ComponentFixture<ConsultSummaryComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultSummaryComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultSummaryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
