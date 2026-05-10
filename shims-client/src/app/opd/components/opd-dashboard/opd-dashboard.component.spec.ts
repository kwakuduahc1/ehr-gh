import { ComponentFixture, TestBed } from '@angular/core/testing';
import { OpdDashboardComponent } from './opd-dashboard.component';

describe('OpdDashboardComponent', () => {
    let component: OpdDashboardComponent;
    let fixture: ComponentFixture<OpdDashboardComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [OpdDashboardComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(OpdDashboardComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
