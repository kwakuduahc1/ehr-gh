import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-vitals',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-vitals-component.html',
    styleUrl: './consult-vitals-component.scss'
})
export class ConsultVitalsComponent { }
