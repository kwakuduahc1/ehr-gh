import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-outcomes',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-outcomes-component.html',
    styleUrl: './consult-outcomes-component.scss'
})
export class ConsultOutcomesComponent { }
