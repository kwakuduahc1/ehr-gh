import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelect, MatOption } from '@angular/material/select';
import { form, required, schema, minLength, maxLength, FormRoot, FormField, applyWhen } from '@angular/forms/signals';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent } from '@angular/material/dialog';
import { ActivityProvider } from '../../../providers/ActivityProvider';
import { AddPatientDto, EditPatientDto } from '../../../models/registrations/IRegistrations';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ValidatorMessages } from '../../../components/auth-validators';

@Component({
    selector: 'app-add-registration',
    templateUrl: './add-registration.component.html',
    styleUrl: './add-registration.component.scss',
    imports: [
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatSelect,
        MatOption,
        FormRoot,
        FormField,
        MatDialogContent
    ]
})
export class AddRegistrationComponent {
    act = inject(ActivityProvider);
    diag = inject(MatDialogRef<AddRegistrationComponent>);
    data = inject<{ patient?: EditPatientDto }>(MAT_DIALOG_DATA);
    private snack = inject(MatSnackBar);
    protected val = new ValidatorMessages();

    fmMdl = signal<AddPatientDto>({
        surname: this.data?.patient?.surname ?? '',
        otherNames: this.data?.patient?.otherNames ?? '',
        dateOfBirth: this.data?.patient?.dateOfBirth ?? null,
        ghanaCard: this.data?.patient?.ghanaCard ?? '',
        sex: this.data?.patient?.sex ?? '',
        phoneNumber: this.data?.patient?.phoneNumber ?? ''
    });

    form = form<AddPatientDto>(this.fmMdl, AddPatientSchema);

    submit() {
        // console.log(!this.form.schemes().value().some(s => s.schemesID === this.schemes.feePaying().schemesID));
        // if (!this.form.schemes().value().some(s => s.schemesID === this.schemes.feePaying().schemesID)) {
        //     console.log('fee paying');
        //     this.form.schemes().value.set([{
        //         schemesID: this.schemes.feePaying().schemesID
        //     }]);
        // }
        this.diag.close(
            { patient: this.form().value(), edit: !!this.data?.patient?.patientsID });
    }

    close() {
        this.diag.close(null);
    }
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
    applyWhen(path.phoneNumber,
        ({ valueOf }) => !valueOf(path.phoneNumber || '')?.length,
        (phone) => {
            minLength((phone), 10);
            maxLength(phone, 10);
        });
    applyWhen(path.ghanaCard,
        ({ valueOf }) => valueOf(path?.ghanaCard || '')?.length > 0,
        (ghanaCard) => {
            minLength(ghanaCard, 17);
            maxLength(ghanaCard, 17);
        });
});
