import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-services',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-services-component.html',
    styleUrl: './consult-services-component.scss'
})
export class ConsultServicesComponent { }
