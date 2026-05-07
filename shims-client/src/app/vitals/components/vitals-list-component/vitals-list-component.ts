import { ChangeDetectionStrategy, Component, computed, inject, input, linkedSignal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { AddVitalsDto, VitalsDTO, VitalsummaryDto } from '../../../models/vitals/IVitals';
import { AddVitalsDialogComponent } from '../add-vitals-dialog/add-vitals.component';
import { filter, map, switchMap, tap } from 'rxjs';
import { VitalsHttpService } from '../../vitals-http.service';

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
    readonly patientAttendanceID = input.required<string>();
    vitals = linkedSignal(() => this.details().vitals);
    patient = computed(() => this.details().patient);
    private http = inject(VitalsHttpService);


    addVitals(vitals?: AddVitalsDto) {
        let vs = vitals;
        this.dialog.open<AddVitalsDialogComponent, {}, AddVitalsDto>(
            AddVitalsDialogComponent,
            {
                width: '1200px',
                disableClose: true,
                data: { vitals: vs }
            }
        )
            .afterClosed()
            .pipe(
                filter(x => !!x),
                map(x => ({ ...x!, patientAttendancesID: this.patientAttendanceID() })),
                tap(result => vs = result),
                switchMap(dto => this.http.add(dto))
            )
            .subscribe({
                next: () => {
                },
                error: () => this.addVitals(vs)
            })
    }

    viewVitals(vital: VitalsDTO) {

    }
}
