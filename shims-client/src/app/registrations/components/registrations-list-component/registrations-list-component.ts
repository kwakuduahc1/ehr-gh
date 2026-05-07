import { Component, inject, model, ChangeDetectionStrategy, input } from '@angular/core';
import { PatientDetailsDto, EditPatientDto, InsuranceDetails } from '../../../models/registrations/IRegistrations';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { filter, iif, switchMap, tap } from 'rxjs';
import { RegistrationsHttpService } from '../../registrations-http.service';
import { ConfirmationComponent } from '../../../components/confirmation/confirmation.component';
import { AddRegistrationComponent } from '../add-registration-component/add-registration.component';
import { SchemesDTO } from '../../../models/ISchemes';
import { InsuranceDetailComponent } from '../insurance-detail-component/insurance-detail-component';
import { PatientSchemesHttpService } from '../../patient-schemes-http.service';
import { RouterLink } from "@angular/router";

@Component({
    selector: 'app-registrations-list',
    templateUrl: './registrations-list-component.html',
    styleUrl: './registrations-list-component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [MatButton, MatIcon, RouterLink]
})
export class RegistrationsListComponent {
    list = model.required<PatientDetailsDto[]>();
    schemes = input.required<SchemesDTO[]>();
    private snack = inject(MatSnackBar);
    private diag = inject(MatDialog);
    private http = inject(RegistrationsHttpService);
    private psHttp = inject(PatientSchemesHttpService);

    addRegistration(patient?: EditPatientDto | PatientDetailsDto) {
        let result: { patient: EditPatientDto, edit: boolean };
        this.diag.open<AddRegistrationComponent, {}, { patient: EditPatientDto, edit: boolean }>(
            AddRegistrationComponent,
            {
                data: { patient },
                width: '800px',
                disableClose: true
            }
        )
            .afterClosed()
            .pipe(
                filter(x => !!x),
                tap(x => result = x!),
                switchMap(x =>
                    iif(() => x?.edit,
                        this.http.update(x!.patient),
                        this.http.register(x!.patient)
                    ))
            )
            .subscribe({
                next: d => {
                    this.snack.open(result.edit ? 'Patient updated' : 'Patient registered');
                    this.list.update(x => {
                        if (result.edit) {
                            return x.map(p => p.patientsID === result.patient.patientsID
                                ? {
                                    ...p,
                                    ghanaCard: result.patient.ghanaCard,
                                    phoneNumber: result.patient.phoneNumber,
                                    dateOfBirth: result.patient.dateOfBirth,
                                    otherNames: result.patient.otherNames,
                                    surname: result.patient.surname
                                } as PatientDetailsDto
                                : p
                            )
                        }
                        return [{
                            patientsID: d!.pid,
                            fullName: `${result.patient.surname} ${result.patient.otherNames}`,
                            hospitalID: d!.hid,
                            phoneNumber: result.patient.phoneNumber,
                            ghanaCard: result.patient.ghanaCard,
                            visitType: 'Acute',
                            patientAttendancesID: '',
                            dateSeen: new Date().toISOString(),
                            sex: result.patient.sex,
                            dateOfBirth: result.patient.dateOfBirth!,
                            age: Math.floor((new Date().getTime() - new Date(result.patient.dateOfBirth!).getTime()) / (1000 * 3600 * 24 * 365)),
                            schemes: []
                        } as PatientDetailsDto, ...x]
                    })

                },
                error: () => this.addRegistration(result.patient)
            });
    }

    viewInsurance(patient: PatientDetailsDto) {
        this.diag.open<InsuranceDetailComponent, {}, InsuranceDetails[]>(InsuranceDetailComponent, {
            data: { patient },
            width: '800px'
        })
            .afterClosed()
            .subscribe(res => {
                if (res) {
                    this.list.update(x =>
                        x.map(p => p.patientsID === patient.patientsID
                            ? { ...p, schemes: res }
                            : p
                        )
                    )
                }
            })
    }

    deleteRegistration(id: string) {
        this.diag.open<ConfirmationComponent, {}, boolean>(ConfirmationComponent, {
            data: 'Are you sure you want to delete this registration?'
        })
            .afterClosed()
            .pipe(
                filter(x => !!x),
                switchMap(() => this.http.delete(id))
            )
            .subscribe({
                next: () => {
                    this.list.update(x => x.filter(p => p.patientsID !== id));
                    this.snack.open('Registration deleted');
                }
            });
    }
}
