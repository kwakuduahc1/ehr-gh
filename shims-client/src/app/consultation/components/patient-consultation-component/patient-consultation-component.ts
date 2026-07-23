import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-patient-consultation',
    imports: [RouterOutlet],
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './patient-consultation-component.html',
    styleUrl: './patient-consultation-component.scss'
})
export class PatientConsultationComponent { }
