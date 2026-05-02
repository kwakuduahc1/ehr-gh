import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VitalsDTO } from '../../../models/vitals/IVitals';

@Component({
    selector: 'app-vitals-list',
    templateUrl: './vitals-list-component.html',
    styleUrl: './vitals-list-component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: []
})
export class VitalsListComponent {
    list = signal<VitalsDTO[]>([]);
}
