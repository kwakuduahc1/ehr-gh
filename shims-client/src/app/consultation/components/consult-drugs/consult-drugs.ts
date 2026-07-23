import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-drugs',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-drugs.html',
    styleUrl: './consult-drugs.scss'
})
export class ConsultDrugsComponent { }
