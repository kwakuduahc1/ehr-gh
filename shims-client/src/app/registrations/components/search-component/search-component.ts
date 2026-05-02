import { CommonModule } from '@angular/common';
import { httpResource } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { environment } from '../../../../environments/environment';
import { PatientDetailsDto } from '../../../models/registrations/IRegistrations';
import { form, FormField, FormRoot } from '@angular/forms/signals';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { ValidatorMessages } from '../../../components/auth-validators';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { ViewSessionsComponent } from '../view-sessions-component/view-sessions-component';
@Component({
  selector: 'app-search-component',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatFormField,
    MatLabel,
    MatInput,
    MatError,
    FormRoot,
    FormField,
    MatProgressBar,
    MatButton,
    MatIcon
  ],
  templateUrl: './search-component.html',
  styleUrl: './search-component.scss',
})
export class SearchComponent {

  searchMdl = signal<{ search: string }>({ search: '' });
  val = new ValidatorMessages();
  form = form(this.searchMdl);
  private diag = inject(MatDialog);

  patients = httpResource<PatientDetailsDto[]>(() => `${environment.AppUrl}Registrations/${this.searchMdl().search}`, {
    defaultValue: []
  });

  addAttendance(p: PatientDetailsDto) {
    this.diag.open<ViewSessionsComponent, {}, { patient: PatientDetailsDto }>(ViewSessionsComponent, {
      data: { patient: p },
      width: '850px',
      disableClose: true
    })
      .afterClosed()
      .subscribe();
  }
}
