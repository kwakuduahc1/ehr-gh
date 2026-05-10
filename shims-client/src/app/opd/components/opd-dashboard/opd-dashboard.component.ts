import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'app-opd-dashboard',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './opd-dashboard.component.html',
    styleUrl: './opd-dashboard.component.scss'
})
export class OpdComponent { }
