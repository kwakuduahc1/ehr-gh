import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-diagnoses',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-diagnoses.html',
    styleUrl: './consult-diagnoses.scss'
})
export class ConsultDiagnosesComponent { }
