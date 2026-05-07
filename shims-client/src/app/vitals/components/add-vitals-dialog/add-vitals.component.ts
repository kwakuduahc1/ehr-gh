import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { applyWhen, form, FormField, FormRoot, max, maxLength, min, minLength, required } from '@angular/forms/signals';
import { MAT_DIALOG_DATA, MatDialogActions, MatDialogContent, MatDialogRef } from '@angular/material/dialog';
import { AddVitalsDto } from '../../../models/vitals/IVitals';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { ValidatorMessages } from '../../../components/auth-validators';
import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatInput } from '@angular/material/input';

@Component({
    selector: 'app-add-vitals',
    templateUrl: './add-vitals.component.html',
    styleUrl: './add-vitals.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        FormRoot,
        FormField,
        MatFormField,
        MatInput,
        MatError,
        MatLabel,
        MatDialogContent,
        MatDialogActions,
        MatIcon,
        MatButton
    ],
})
export class AddVitalsDialogComponent {
    diagRef = inject(MatDialogRef<AddVitalsDialogComponent>);
    private data = inject<{ vitals: AddVitalsDto }>(MAT_DIALOG_DATA);
    private snack = inject(MatSnackBar);
    protected val = new ValidatorMessages();
    fmMld = signal<Omit<AddVitalsDto, 'patientsAttendancesID'>>({
        temperature: this.data.vitals?.temperature || NaN,
        pulse: this.data.vitals?.pulse || NaN,
        respiration: this.data.vitals?.respiration || NaN,
        systol: this.data.vitals?.systol || NaN,
        diastol: this.data.vitals?.diastol || NaN,
        complaints: this.data.vitals?.complaints || '',
        notes: this.data.vitals?.notes || '',
        sPO2: this.data.vitals?.sPO2 || NaN,
        weight: this.data.vitals?.weight || NaN,
    });

    form = form(this.fmMld, path => {
        required(path.temperature);
        min(path.temperature, 35.5);
        max(path.temperature, 42);

        required(path.weight);
        min(path.weight, 0.5);
        max(path.weight, 200);

        min(path.pulse, 30);
        max(path.pulse, 220);

        min(path.systol, 50);
        max(path.systol, 250);

        applyWhen(path, ({ valueOf, stateOf }) =>
            valueOf(path.systol) > 50 &&
            stateOf(path.systol).valid(),
            (sp) => {
                required(sp.diastol)
            }
        );

        min(path.diastol, 30);
        max(path.diastol, 150,);

        min(path.respiration, 5);
        max(path.respiration, 100);

        min(path.sPO2, 50);
        max(path.sPO2, 100);

        minLength(path.complaints, 5);
        maxLength(path.complaints, 500);

        maxLength(path.notes, 500);
        minLength(path.notes, 5);
    }, {
        submission: {
            action: async _ => this.addVitals(),
            onInvalid: _ => this.snack.open('Please fill out all required fields correctly.', 'Close')
        }
    });

    constructor() {
        console.log(this.data);
    }

    addVitals() {
        this.diagRef.close(this.form().value());
    }

    close() {
        console.log('closing dialog');
        this.diagRef.close();
    }

}
