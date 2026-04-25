import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelect, MatOption } from '@angular/material/select';
import { form, required, schema, minLength, maxLength, FormRoot, FormField } from '@angular/forms/signals';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ValidatorMessages } from '../../../components/auth-validators';
import { ActivityProvider } from '../../../providers/ActivityProvider';
import { AddPatientDto } from '../../../models/registrations/IRegistrations';
import { ListPatientsDto } from '../../../models/registrations/IRegistrations';
import { SchemesHttpService } from '../../../schemes/schemes-http.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
    selector: 'app-add-registration',
    templateUrl: './add-registration.component.html',
    styleUrl: './add-registration.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatSelect,
        MatOption,
        FormRoot,
        FormField
    ]
})
export class AddRegistrationComponent {
    // act = inject(ActivityProvider);
    // diag = inject(MatDialogRef<AddRegistrationComponent>);
    // data = inject<{ patient?: ListPatientsDto }>(MAT_DIALOG_DATA);
    // schemes = toSignal(inject(SchemesHttpService).getSchemes(), { initialValue: [] });
    // val = new ValidatorMessages();

    // form = form<AddPatientDto>(signal<AddPatientDto>({
    //     surname: this.data?.patient?.fullName?.split(' ')[0] ?? '',
    //     otherNames: this.data?.patient?.fullName?.split(' ').slice(1).join(' ') ?? '',
    //     dateOfBirth: '',
    //     ghanaCard: this.data?.patient?.hospitalID ?? '',
    //     sex: this.data?.patient?.gender ?? '',
    //     schemesID: this.data?.patient?.schemesID ?? '',
    //     cardID: this.data?.patient?.cardID ?? '',
    //     expiryDate: this.data?.patient?.expiryDate ?? '',
    //     phoneNumber: ''
    // }), AddPatientSchema);

    // submit() {
    //     this.diag.close({
    //         patient: {
    //             ...this.form().value(),
    //             patientID: this.data?.patient?.patientID
    //         },
    //         edit: !!this.data?.patient
    //     });
    // }

    // close() {
    //     this.diag.close(null);
    // }
}

const AddPatientSchema = schema<AddPatientDto>((path) => {
    required(path.surname);
    minLength(path.surname, 2);
    maxLength(path.surname, 50);

    required(path.otherNames);
    minLength(path.otherNames, 2);
    maxLength(path.otherNames, 100);

    required(path.dateOfBirth);
    required(path.sex);
    required(path.schemesID);
    required(path.phoneNumber);
    minLength(path.phoneNumber, 10);
    maxLength(path.phoneNumber, 10);
});
