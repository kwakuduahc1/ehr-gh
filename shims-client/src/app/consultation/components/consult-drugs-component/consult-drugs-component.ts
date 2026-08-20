import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-drugs',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-drugs-component.html',
    styleUrl: './consult-drugs-component.scss'
})
export class ConsultDrugsComponent { }
