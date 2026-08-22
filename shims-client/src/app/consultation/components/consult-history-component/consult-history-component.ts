import { ChangeDetectionStrategy, Component, inject, input, model, signal } from '@angular/core';
import { AddPatientConsultationDto, PatientConsultationDto } from '../../consult-models';
import { schema, required, minLength, maxLength, applyWhen, min, max, form, disabled, FormRoot, FormField } from '@angular/forms/signals';
import { PatientDetailsDto } from '../../../models/registrations/IRegistrations';
import { MatError, MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { ValidatorMessages } from '../../../components/auth-validators';
import { ActivityProvider } from '../../../providers/ActivityProvider';
import { MatIcon } from '@angular/material/icon';
import { MatCheckbox } from '@angular/material/checkbox';
import { DatePipe } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConsultHistoryHttpService } from '../../http/consult-history.service';

@Component({
    selector: 'app-consult-history',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './consult-history-component.html',
    styleUrl: './consult-history-component.scss',
    imports: [
        MatFormField,
        MatInput,
        MatLabel,
        MatError,
        FormRoot,
        FormField,
        MatIcon,
        DatePipe,
        MatCheckbox
    ]
})
export class ConsultHistoryComponent {

    patient = input<PatientDetailsDto>();
    fmMdl = signal<AddPatientConsultationDto>({
        patientAttendancesID: this.patient()?.patientAttendancesID ?? '',
        complaints: '',
        odq: '',
        hasAvpu: false,
        avpu: {
            alert: NaN,
            verbal: NaN,
            pain: NaN,
            responsive: NaN
        },
        hasGcs: false,
        gcs: {
            eyeOpening: NaN,
            verbalResponse: NaN,
            motorResponse: NaN
        }
    });
    val = new ValidatorMessages();
    protected act = inject(ActivityProvider);
    private snack = inject(MatSnackBar);
    private http = inject(ConsultHistoryHttpService);
    history = model.required<PatientConsultationDto[]>();
    form = form<AddPatientConsultationDto>(this.fmMdl, AddPatientSchema, {
        submission: {
            action: async (f) => this.saveHist(f().value()),
            onInvalid: async () => this.snack.open('Form is invalid. Please check the fields and try again.', 'Close')
        }
    });

    saveHist(hist: AddPatientConsultationDto) {
        this.http.addHistory({ ...hist, patientAttendancesID: this.patient()!.patientAttendancesID }).subscribe({
            next: (res) => {
                this.snack.open('Consultation history saved successfully.', 'Close');
                this.form().reset();
                this.fmMdl.set({
                    patientAttendancesID: this.patient()?.patientAttendancesID ?? '',
                    complaints: '',
                    odq: '',
                    hasAvpu: false,
                    avpu: {
                        alert: NaN,
                        verbal: NaN,
                        pain: NaN,
                        responsive: NaN
                    },
                    hasGcs: false,
                    gcs: {
                        eyeOpening: NaN,
                        verbalResponse: NaN,
                        motorResponse: NaN
                    }
                });
            }
        });
    }
}

const AddPatientSchema = schema<AddPatientConsultationDto>((path) => {
    required(path.patientAttendancesID);
    minLength(path.patientAttendancesID, 2);
    maxLength(path.patientAttendancesID, 50);
    disabled(path.patientAttendancesID);

    required(path.complaints);
    minLength(path.complaints, 4);
    maxLength(path.complaints, 500);

    required(path.odq);
    minLength(path.odq, 4);
    maxLength(path.odq, 250);

    applyWhen(path, ({ valueOf }) => !!valueOf(path.hasAvpu),
        (sp) => {
            // Alert
            required(sp.avpu.alert);
            min(sp.avpu.alert, 0);
            max(sp.avpu.alert, 4);

            // Verbal
            required(sp.avpu.verbal);
            min(sp.avpu.verbal, 0);
            max(sp.avpu.verbal, 3);

            // Pain
            required(sp.avpu.pain);
            min(sp.avpu.pain, 0);
            max(sp.avpu.pain, 2);

            // Response
            required(sp.avpu.responsive);
            min(sp.avpu.responsive, 0);
            max(sp.avpu.responsive, 1);
        });

    applyWhen(path, ({ valueOf }) => !!valueOf(path.hasGcs),
        (sp) => {
            // Eye opening
            required(sp.gcs.eyeOpening);
            min(sp.gcs.eyeOpening, 0);
            max(sp.gcs.eyeOpening, 3);

            // Verbal response
            required(sp.gcs.verbalResponse);
            min(sp.gcs.verbalResponse, 0);
            max(sp.gcs.verbalResponse, 5);

            // Motor response
            required(sp.gcs.motorResponse);
            min(sp.gcs.motorResponse, 0);
            max(sp.gcs.motorResponse, 6);
        }
    )
});
