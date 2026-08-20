import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ConsultHistoryComponent } from './consult-history-component';

describe('ConsultHistoryComponent', () => {
    let component: ConsultHistoryComponent;
    let fixture: ComponentFixture<ConsultHistoryComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultHistoryComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultHistoryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
