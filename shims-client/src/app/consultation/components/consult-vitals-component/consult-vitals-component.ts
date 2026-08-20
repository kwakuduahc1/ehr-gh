import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { VitalsDTO, VitalsummaryDto } from '../../../models/vitals/IVitals';
import { DatePipe, JsonPipe } from '@angular/common';

@Component({
    selector: 'app-consult-vitals',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-vitals-component.html',
    styleUrl: './consult-vitals-component.scss',
    imports: [
        DatePipe
    ]
})
export class ConsultVitalsComponent {
    vitals = input.required<VitalsDTO[]>();
}
