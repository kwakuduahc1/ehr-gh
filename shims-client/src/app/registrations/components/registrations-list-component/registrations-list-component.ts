import { Component, inject, model, ChangeDetectionStrategy, input } from '@angular/core';
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
import { SchemesDTO } from '../../../models/ISchemes';

@Component({
    selector: 'app-registrations-list',
    templateUrl: './registrations-list-component.html',
    styleUrl: './registrations-list-component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [MatButton, MatIcon, DatePipe]
})
export class RegistrationsListComponent {
    list = model.required<ListPatientsDto[]>();
    schemes = input.required<SchemesDTO[]>();
    private snack = inject(MatSnackBar);
    private diag = inject(MatDialog);
    private http = inject(RegistrationsHttpService);

    addRegistration(patient?: EditPatientDto | ListPatientsDto) {
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
                    // this.patients.update(list =>
                    //     result.edit
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
                    console.log(result);
                    this.addRegistration(result.patient);
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
                    this.list.update(x => x.filter(p => p.patientsID !== id));
                    this.snack.open('Registration deleted');
                }
            });
    }
}
