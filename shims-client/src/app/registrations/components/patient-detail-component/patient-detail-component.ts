import { Component, input } from '@angular/core';
import { ListPatientsDto } from '../../../models/registrations/IRegistrations';

@Component({
  selector: 'app-patient-detail-component',
  imports: [],
  templateUrl: './patient-detail-component.html',
  styleUrl: './patient-detail-component.scss',
})
export class PatientDetailComponent {
  patient = input<ListPatientsDto>();
}
