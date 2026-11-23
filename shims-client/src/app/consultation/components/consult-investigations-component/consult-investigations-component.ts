import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ConsultationHttpService } from '../../http/consultation-http.service';
import { PatientAttendanceInvestigations, SchemeInvestigationDTO } from '../../consult-models';
import { PatientDetailsDto } from '../../../models/registrations/IRegistrations';

interface AvailableInvestigation extends SchemeInvestigationDTO {
    schemeName: string;
}

@Component({
    selector: 'app-consult-investigations',
    imports: [CurrencyPipe, DatePipe],
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-investigations-component.html',
    styleUrl: './consult-investigations-component.scss'
})
export class ConsultInvestigationsComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly api = inject(ConsultationHttpService);
    private readonly destroyRef = inject(DestroyRef);

    readonly patient = signal<PatientDetailsDto | null>(null);
    readonly available = signal<AvailableInvestigation[]>([]);
    readonly requested = signal<PatientAttendanceInvestigations[]>([]);
    readonly selectedId = signal('');
    readonly loading = signal(true);
    readonly submittingId = signal<string | null>(null);
    readonly error = signal('');
    readonly notice = signal('');

    constructor() {
        const patient = this.route.parent?.snapshot.data['patient'] as PatientDetailsDto | undefined;
        if (!patient) {
            this.error.set('Patient consultation details could not be loaded.');
            this.loading.set(false);
            return;
        }

        this.patient.set(patient);
        const schemes = patient.schemes ?? [];
        const available$ = schemes.length
            ? forkJoin(schemes.map(scheme => this.api.getInvestigationsByScheme(scheme.schemesID).pipe(
                catchError(() => of([] as SchemeInvestigationDTO[]))
            )))
            : of([] as SchemeInvestigationDTO[][]);

        forkJoin({
            available: available$,
            requested: this.api.getInvestigationsByAttendance(patient.patientAttendancesID)
        }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: result => {
                const schemeNames = new Map(schemes.map(scheme => [scheme.schemesID, scheme.schemeName ?? 'Scheme']));
                const uniqueInvestigations = new Map(
                    result.available.flat()
                        .filter(item => item.isActive)
                        .map(item => [item.schemeInvestigationsID, {
                            ...item,
                            schemeName: schemeNames.get(item.schemesID) ?? 'Scheme'
                        }])
                );
                this.available.set([...uniqueInvestigations.values()]);
                this.requested.set(result.requested);
                this.loading.set(false);
            },
            error: () => {
                this.error.set('Unable to load investigations. Please try again.');
                this.loading.set(false);
            }
        });
    }

    isRequested(investigationId: string): boolean {
        return this.requested().some(item => item.investigationsID === investigationId);
    }

    selectInvestigation(event: Event): void {
        this.selectedId.set((event.target as HTMLSelectElement).value);
    }

    requestSelectedInvestigation(): void {
        const item = this.available().find(candidate => candidate.schemeInvestigationsID === this.selectedId());
        if (item && !this.isRequested(item.investigationsID)) {
            this.requestInvestigation(item);
        }
    }

    requestInvestigation(item: AvailableInvestigation): void {
        const patient = this.patient();
        if (!patient || this.submittingId()) return;

        this.error.set('');
        this.notice.set('');
        this.submittingId.set(item.schemeInvestigationsID);
        this.api.addInvestigationRequest({
            patientsAttendancesID: patient.patientAttendancesID,
            schemeInvestigationsID: item.schemeInvestigationsID
        }).pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
            next: () => {
                this.requested.update(items => [...items, {
                    investigationSRequestsID: '',
                    investigationsID: item.investigationsID,
                    dateRequested: new Date().toISOString(),
                    investigation: item.investigation ?? ''
                }]);
                this.notice.set(`${item.investigation ?? 'Investigation'} requested.`);
                this.submittingId.set(null);
            },
            error: () => {
                this.error.set('Could not request this investigation. Please try again.');
                this.submittingId.set(null);
            }
        });
    }
}
