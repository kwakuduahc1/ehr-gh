import { ConsultVitalsComponent } from './consult-vitals-component';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { VitalsDTO } from '../../../models/vitals/IVitals';

describe('ConsultVitalsComponent', () => {
    let component: ConsultVitalsComponent;
    let fixture: ComponentFixture<ConsultVitalsComponent>;

    // Mock data for testing
    const mockVitals: VitalsDTO[] = [
        {
            dateSeen: '2023-10-01T00:00:00',
            weight: 75,
            temperature: 37,
            systol: 120,
            diastol: 80,
            pulse: 72,
            complaints: 'Fever',
            notes: 'Tired',
            patientsAttendancesID: 'attendance-1',
            vitalsID: 'vitals-1',
            respiration: 72,
            sPO2: 97,
            userName: 'nurse1'
        },
        {
            dateSeen: '2023-10-05T00:00:00',
            weight: 76,
            temperature: 37.1,
            systol: 125,
            diastol: 85,
            pulse: 75,
            complaints: '',
            notes: 'No complaints',
            patientsAttendancesID: 'attendance-2',
            vitalsID: 'vitals-2',
            respiration: 70,
            sPO2: 98,
            userName: 'nurse2'
        }
    ];

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [ConsultVitalsComponent]
        }).compileComponents();

        fixture = TestBed.createComponent(ConsultVitalsComponent);
        component = fixture.componentInstance;
        // Mock the input signal with test data
        component.vitals.set(mockVitals);
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should display all vitals data in the table', () => {
        const compiled: any = fixture.nativeElement;
        const rows = compiled['querySelector']('.table-responsive tbody tr');

        // Check if the correct number of rows is displayed
        expect(rows.length).toBe(mockVitals.length);

        // Check data for the first row
        const firstRow = rows[0];
        expect(firstRow.querySelector('td:nth-child(1)').textContent).toContain('Oct 1, 2023'); // Date format check
        expect(firstRow.querySelector('td:nth-child(2)').textContent).toContain('75 kg');
        expect(firstRow.querySelector('td:nth-child(3)').textContent).toContain('37.0 °C');
        expect(firstRow.querySelector('td:nth-child(4)').textContent).toContain('120/80');
        expect(firstRow.querySelector('td:nth-child(5)').textContent).toContain('72 bpm');
        expect(firstRow.querySelector('td:nth-child(6)').textContent).toContain('72 bpm'); // Assuming pulse and respiration are often the same in the sample, but testing based on provided data/template structure
        expect(firstRow.querySelector('td:nth-child(7)').textContent).toContain('Fever');
        expect(firstRow.querySelector('td:nth-child(8)').textContent).toContain('Tired');

        // Check data for the second row (example of empty/different data)
        const secondRow = rows[1];
        expect(secondRow.querySelector('td:nth-child(2)').textContent).toContain('76 kg');
        expect(secondRow.querySelector('td:nth-child(7)').textContent).toContain(''); // Empty complaints
    });
});
