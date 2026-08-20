import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-diagnoses',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-diagnoses-component.html',
    styleUrl: './consult-diagnoses-component.scss'
})
export class ConsultDiagnosesComponent { }
