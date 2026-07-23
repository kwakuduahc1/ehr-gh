import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-list',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-list-component.html',
    styleUrl: './consult-list-component.scss'
})
export class ConsultListComponent { }
