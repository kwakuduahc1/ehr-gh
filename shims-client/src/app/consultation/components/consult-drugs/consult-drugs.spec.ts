import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultDrugsComponent } from './consult-drugs';

describe('ConsultDrugsComponent', () => {
    let component: ConsultDrugsComponent;
    let fixture: ComponentFixture<ConsultDrugsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultDrugsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultDrugsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
