import { Component, inject, model, ChangeDetectionStrategy } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ListPatientsDto, EditPatientDto, AddPatientDto } from '../../../models/registrations/IRegistrations';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { filter, iif, switchMap, tap } from 'rxjs';
import { RegistrationsHttpService } from '../../registrations-http.service';
import { ConfirmationComponent } from '../../../components/confirmation/confirmation.component';
import { AddRegistrationComponent } from '../add-registration-component/add-registration.component';

@Component({
    selector: 'app-registrations-list',
    templateUrl: './registrations-list-component.html',
    styleUrl: './registrations-list-component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [MatButton, MatIcon, DatePipe]
})
export class RegistrationsListComponent {
    patients = model.required<ListPatientsDto[]>();
    private snack = inject(MatSnackBar);
    private diag = inject(MatDialog);
    private http = inject(RegistrationsHttpService);

    addRegistration(patient?: ListPatientsDto) {
        let isEdit: boolean;
        let result: { patient: EditPatientDto, edit: boolean };
        this.diag.open<AddRegistrationComponent, {}, { patient: EditPatientDto, edit: boolean }>(
            AddRegistrationComponent,
            {
                data: { patient },
                width: '800px'
            }
        )
            .afterClosed()
            .pipe(
                filter(x => !!x),
                tap(x => {
                    isEdit = x!.edit;
                    result = x!;
                }),
                switchMap(x =>
                    iif(() => x?.edit,
                        this.http.update(x!.patient),
                        this.http.register(x!.patient)
                    ))
            )
            .subscribe({
                next: d => {
                    this.snack.open(isEdit ? 'Patient updated' : 'Patient registered');
                    // this.patients.update(list =>
                    //     isEdit
                    //         ? list.map(p => p.patientID === result.patient.patientID
                    //             ? { ...p, gender: result.patient.sex }
                    //             : p)
                    //         : [
                    //             {
                    //                 age: 0,
                    //                 attendanceDate: new Date(),
                    //                 fullName: `${result.patient.surname} ${result.patient.otherNames}`,
                    //                 gender: result.patient.sex,
                    //                 hospitalID: d!.hosid,
                    //                 patientID: d!.pid,
                    //                 patientSchemesID: result.patient.patientID,
                    //                 visitType: result.patient.visitType,
                    //             }, ...list]
                    // );
                },
                error: () => {
                    this.addRegistration(patient);
                }
            });
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
                    this.patients.update(x => x.filter(p => p.patientID !== id));
                    this.snack.open('Registration deleted');
                }
            });
    }
}
