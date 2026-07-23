import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-consult-services',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-services.html',
    styleUrl: './consult-services.scss'
})
export class ConsultServicesComponent { }
