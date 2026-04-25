import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AddSchemeComponent } from './add-scheme.component';

describe('AddSchemeComponent', () => {
    let component: AddSchemeComponent;
    let fixture: ComponentFixture<AddSchemeComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [AddSchemeComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(AddSchemeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
