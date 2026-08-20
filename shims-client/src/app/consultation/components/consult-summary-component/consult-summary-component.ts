import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-summary',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-summary-component.html',
    styleUrl: './consult-summary-component.scss'
})
export class ConsultSummaryComponent { }
