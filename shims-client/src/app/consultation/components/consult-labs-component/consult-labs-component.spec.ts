import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultLabsComponent } from './consult-labs-component';

describe('ConsultLabsComponent', () => {
    let component: ConsultLabsComponent;
    let fixture: ComponentFixture<ConsultLabsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultLabsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultLabsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
