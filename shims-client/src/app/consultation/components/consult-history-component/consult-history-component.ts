import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-history',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-history-component.html',
    styleUrl: './consult-history-component.scss'
})
export class ConsultHistoryComponent { }
