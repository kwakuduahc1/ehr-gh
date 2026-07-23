import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-outcomes',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-outcomes.html',
    styleUrl: './consult-outcomes.scss'
})
export class ConsultOutcomesComponent { }
