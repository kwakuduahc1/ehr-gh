import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-investigations',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-investigations-component.html',
    styleUrl: './consult-investigations-component.scss'
})
export class ConsultInvestigationsComponent { }
