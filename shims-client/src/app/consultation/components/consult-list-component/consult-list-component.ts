import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ConsultationHttpService } from '../../http/consultation-http.service';
import { PatientDetailsDto } from '../../../models/registrations/IRegistrations';

@Component({
    selector: 'app-consult-list',
    imports: [RouterLink],
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-list-component.html',
    styleUrl: './consult-list-component.scss'
})
export class ConsultListComponent {
    private readonly api = inject(ConsultationHttpService);
    private readonly destroyRef = inject(DestroyRef);

    readonly patients = signal<PatientDetailsDto[]>([]);
    readonly loading = signal(true);
    readonly error = signal('');

    constructor() {
        this.api.pending().pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: patients => {
                this.patients.set(patients);
                this.loading.set(false);
            },
            error: () => {
                this.error.set('Unable to load patients awaiting consultation.');
                this.loading.set(false);
            }
        });
    }
}
