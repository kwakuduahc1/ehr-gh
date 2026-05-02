import { ChangeDetectionStrategy, Component, computed, inject, input, linkedSignal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { VitalsummaryDto } from '../../../models/vitals/IVitals';

@Component({
    selector: 'app-vitals-list',
    templateUrl: './vitals-list-component.html',
    styleUrl: './vitals-list-component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        CommonModule,
        MatButtonModule,
        MatIconModule,
        MatTableModule
    ]
})
export class VitalsListComponent {
    private dialog = inject(MatDialog);
    readonly details = input.required<VitalsummaryDto>();
    vitals = linkedSignal(() => this.details().vitals);
    patient = computed(() => this.details().patient);

}
