import { Component, inject, ChangeDetectionStrategy, signal, computed } from '@angular/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelect, MatOption } from '@angular/material/select';
import { form, required, schema, minLength, maxLength, FormRoot, FormField, applyWhen, applyEach } from '@angular/forms/signals';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogContent } from '@angular/material/dialog';
import { ActivityProvider } from '../../../providers/ActivityProvider';
import { AddPatientDto, EditPatientDto, InsuranceInformation } from '../../../models/registrations/IRegistrations';
import { SchemesDTO } from '../../../models/ISchemes';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ValidatorMessages } from '../../../components/auth-validators';
import { MatIcon } from "@angular/material/icon";
import { SchemesService } from '../../../providers/schemes-service';

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
        FormField,
        MatIcon,
        MatDialogContent
    ]
})
export class AddRegistrationComponent {
    act = inject(ActivityProvider);
    diag = inject(MatDialogRef<AddRegistrationComponent>);
    data = inject<{ patient?: EditPatientDto }>(MAT_DIALOG_DATA);
    private snack = inject(MatSnackBar);
    protected val = new ValidatorMessages();
    schemes = inject(SchemesService);


    fmMdl = signal<AddPatientDto>({
        surname: this.data?.patient?.surname ?? '',
        otherNames: this.data?.patient?.otherNames ?? '',
        dateOfBirth: this.data?.patient?.dateOfBirth ?? null,
        ghanaCard: this.data?.patient?.ghanaCard ?? '',
        sex: this.data?.patient?.sex ?? '',
        schemes: this.data.patient?.schemes || [],
        phoneNumber: this.data?.patient?.phoneNumber ?? ''
    })
    form = form<AddPatientDto>(this.fmMdl, AddPatientSchema);

    addScheme() {
        if (this.fmMdl().schemes.length + 1 === this.schemes.schemes.length)
            this.snack.open('You cannot add any more schemes', 'Ok');
        else
            this.fmMdl.update(x => (
                {
                    ...x,
                    schemes: [{
                        cardID: '',
                        expiryDate: '',
                        schemesID: ''
                    }, ...x.schemes]
                }));
    }

    removeScheme(id: string) {
        // if (this.ptSchemes().length === 1) {
        //     this.snack.open('Patient must have at least one scheme');
        //     return;
        // }
        // else this.ptSchemes.update(x => x.filter(s => s.schemesID !== id));
    }


    submit() {
        this.diag.close(this.form().value());
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

    applyEach(path.schemes, (scheme) => {
        required(scheme.schemesID);

        // 019dc2b7-77a7-7fb2-b246-e7892984c2ff = "Fee paying"
        applyWhen(scheme.expiryDate,
            ({ valueOf }) => valueOf(scheme.schemesID) !== '019dc2b7-77a7-7fb2-b246-e7892984c2ff',
            (sid) => {
                required(sid);
            });

        applyWhen(scheme.cardID,
            ({ valueOf }) => valueOf(scheme.schemesID) !== '019dc2b7-77a7-7fb2-b246-e7892984c2ff',
            (cardID) => {
                required(cardID);
                minLength(cardID, 5);
                maxLength(cardID, 20);
            });
    });
});
