import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultServicesComponent } from './consult-services';

describe('ConsultServicesComponent', () => {
    let component: ConsultServicesComponent;
    let fixture: ComponentFixture<ConsultServicesComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultServicesComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultServicesComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
