import { Component, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { form, required, schema, minLength, maxLength, validate, FormRoot, FormField, min, max, applyWhen } from '@angular/forms/signals';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ActivityProvider } from '../../../providers/ActivityProvider';
import { MatSelect, MatOption } from "@angular/material/select";
import { AddSchemeDto, UpdateSchemeDto } from '../../../models/ISchemes';
import { ErrorMessagesComponent } from '../../../components/error-messages-component/error-messages-component';


@Component({
    selector: 'app-add-scheme',
    templateUrl: './add-scheme.component.html',
    styleUrls: ['./add-scheme.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        ErrorMessagesComponent,
        CommonModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        FormRoot,
        FormField,
        MatSelect,
        MatOption
    ]
})
export class AddSchemeComponent {
    act = inject(ActivityProvider);
    diag = inject(MatDialogRef<AddSchemeComponent>);
    data = inject<{ scheme: UpdateSchemeDto }>(MAT_DIALOG_DATA);
    form = form<AddSchemeDto>(signal<AddSchemeDto>({
        schemeName: this.data?.scheme?.schemeName || '',
        coverage: this.data?.scheme?.coverage || 'Full',
        maxPayable: this.data?.scheme?.maxPayable || 0,
        recovery: this.data?.scheme?.recovery || 0
    }), AddSchemeSchema);

    addScheme() {
        this.diag.close(
            {
                scheme: {
                    schemesID: this.data?.scheme ? this.data.scheme.schemesID : undefined,
                    ...this.form().value()
                },
                edit: !!this.data?.scheme
            })
    }

    close() {
        this.diag.close(null);
    }
}

const AddSchemeSchema = schema<AddSchemeDto>((path) => {
    // Scheme Name validations
    required(path.schemeName);
    minLength(path.schemeName, 4);
    maxLength(path.schemeName, 100);

    // Coverage validations
    required(path.coverage);
    minLength(path.coverage, 1);
    maxLength(path.coverage, 30);

    // Max Payable validations
    required(path.maxPayable, {
        when: ({ valueOf }) => valueOf(path.coverage) === 'Relative',
    });

    applyWhen(
        path.maxPayable,
        ({ valueOf }) => valueOf(path.coverage) === 'Relative',
        (maxPay) => {
            required(maxPay, { message: 'Max pay is required' });
            min(maxPay, 0)
        }
    );

    applyWhen(
        path.recovery,
        ({ valueOf }) => valueOf(path.coverage) === 'Relative',
        (recovery) => {
            required(recovery);
            min(recovery, 0.01),
                max(recovery, 100)
        }
    );

});
