import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTab, MatTabsModule } from '@angular/material/tabs';
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-patient-consultation',
    imports: [
        MatTab,
        MatIconModule,
        RouterOutlet,
        MatTabsModule
    ],
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './patient-consultation-component.html',
    styleUrl: './patient-consultation-component.scss'
})
export class PatientConsultationComponent {
    private router = inject(Router);
    protected route = inject(ActivatedRoute);
    selectedTabIndex = signal(0);

    readonly tabs = [
        { path: 'vitals', icon: 'monitor_heart', label: 'Vitals', color: 'primary' },
        { path: 'history', icon: 'history', label: 'History', color: 'secondary' },
        { path: 'investigations', icon: 'science', label: 'Investigations', color: 'accent' },
        { path: 'labs', icon: 'biotech', label: 'Labs', color: 'info' },
        { path: 'drugs', icon: 'local_pharmacy', label: 'Drugs', color: 'success' },
        { path: 'summary', icon: 'summarize', label: 'Summary', color: 'warning' }
    ];

    onTabChange(evt: number): void {
        this.selectedTabIndex.set(evt);

        this.router.navigate([this.tabs[evt].path], { relativeTo: this.route });
    }

}
