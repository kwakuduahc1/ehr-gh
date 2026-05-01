import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { SessionsHttpService } from '../../sessions-http.service';
import { PatientDetailsDto, VwSessions } from '../../../models/registrations/IRegistrations';

@Component({
  selector: 'app-view-sessions-component',
  imports: [DatePipe, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './view-sessions-component.html',
  styleUrl: './view-sessions-component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ViewSessionsComponent {
  private http = inject(SessionsHttpService);
  data = inject<{ patient: PatientDetailsDto }>(MAT_DIALOG_DATA);
  sessions = signal<VwSessions[]>([]);

  constructor() {
    this.http.patientSessions(this.data.patient.patientsID)
      .subscribe((data) => {
        this.sessions.set(data);
      });
  }

  endSession(session: VwSessions): void {
    this.http.endSession(session.patientAttendancesID).subscribe(() => {
      this.sessions.update(list =>
        list.map(s => s.patientAttendancesID === session.patientAttendancesID ? { ...s, isActive: false } : s)
      );
    });
  }
}
