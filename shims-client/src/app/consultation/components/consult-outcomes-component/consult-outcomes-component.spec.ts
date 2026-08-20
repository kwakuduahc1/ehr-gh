import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultOutcomesComponent } from './consult-outcomes';

describe('ConsultOutcomesComponent', () => {
    let component: ConsultOutcomesComponent;
    let fixture: ComponentFixture<ConsultOutcomesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultOutcomesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultOutcomesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
