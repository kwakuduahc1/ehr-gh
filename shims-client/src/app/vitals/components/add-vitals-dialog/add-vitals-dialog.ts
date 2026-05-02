// import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
// import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
// import { MatButtonModule } from '@angular/material/button';
// import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
// import { MatFormFieldModule } from '@angular/material/form-field';
// import { MatIconModule } from '@angular/material/icon';
// import { MatInputModule } from '@angular/material/input';
// import { AddVitalsDto } from '../../../models/vitals/IVitals';

// @Component({
//     selector: 'app-add-vitals-dialog',
//     template: `
//         <h2 mat-dialog-title>Add Vitals</h2>
//         <mat-dialog-content>
//             <form [formGroup]="form">
//                 <mat-form-field appearance="outline">
//                     <mat-label>Temperature (°C)</mat-label>
//                     <input matInput type="number" step="0.1" formControlName="temperature" />
//                 </mat-form-field>

//                 <mat-form-field appearance="outline">
//                     <mat-label>Weight (kg)</mat-label>
//                     <input matInput type="number" step="0.1" formControlName="weight" />
//                 </mat-form-field>

//                 <mat-form-field appearance="outline">
//                     <mat-label>Pulse (bpm)</mat-label>
//                     <input matInput type="number" formControlName="pulse" />
//                 </mat-form-field>

//                 <div class="bp-row">
//                     <mat-form-field appearance="outline">
//                         <mat-label>Systolic (mmHg)</mat-label>
//                         <input matInput type="number" formControlName="systol" />
//                     </mat-form-field>

//                     <mat-form-field appearance="outline">
//                         <mat-label>Diastolic (mmHg)</mat-label>
//                         <input matInput type="number" formControlName="diastol" />
//                     </mat-form-field>
//                 </div>

//                 <mat-form-field appearance="outline">
//                     <mat-label>Respiration Rate (rpm)</mat-label>
//                     <input matInput type="number" formControlName="respiration" />
//                 </mat-form-field>

//                 <mat-form-field appearance="outline">
//                     <mat-label>SpO₂ (%)</mat-label>
//                     <input matInput type="number" step="0.1" formControlName="sPO2" />
//                 </mat-form-field>

//                 <mat-form-field appearance="outline">
//                     <mat-label>Complaints</mat-label>
//                     <textarea matInput formControlName="complaints"></textarea>
//                 </mat-form-field>

//                 <mat-form-field appearance="outline">
//                     <mat-label>Notes</mat-label>
//                     <textarea matInput formControlName="notes"></textarea>
//                 </mat-form-field>
//             </form>
//         </mat-dialog-content>
//         <mat-dialog-actions align="end">
//             <button type="button" mat-button mat-dialog-close>
//                 <mat-icon>close</mat-icon> Cancel
//             </button>
//             <button type="button" mat-flat-button color="primary" [disabled]="form.invalid" [mat-dialog-close]="getFormValue()">
//                 <mat-icon>check</mat-icon> Add Vitals
//             </button>
//         </mat-dialog-actions>
//     `,
//     styles: `
//         :host {
//             display: block;
//         }

//         form {
//             display: flex;
//             flex-direction: column;
//             gap: 1rem;
//         }

//         .bp-row {
//             display: flex;
//             gap: 1rem;

//             mat-form-field {
//                 flex: 1;
//             }
//         }

//         mat-dialog-actions {
//             margin-top: 1.5rem;
//         }
//     `,
//     changeDetection: ChangeDetectionStrategy.OnPush,
//     imports: [ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule],
//     standalone: true,
// })
// export class AddVitalsDialogComponent {
//     private fb = inject(FormBuilder);
//     private data: { patientAttendancesID: string } = inject(MAT_DIALOG_DATA);

//     form = this.fb.group({
//         temperature: [0, [Validators.required, Validators.min(30), Validators.max(45)]],
//         weight: [0, [Validators.required, Validators.min(1)]],
//         pulse: [null as number | null, []],
//         systol: [null as number | null, []],
//         diastol: [null as number | null, []],
//         respiration: [null as number | null, []],
//         sPO2: [null as number | null, []],
//         complaints: ['', [Validators.required]],
//         notes: [''],
//     });

//     getFormValue(): AddVitalsDto | null {
//         if (this.form.invalid) return null;

//         return {
//             PatientsAttendancesID: this.data.patientAttendancesID,
//             ...this.form.value,
//         } as AddVitalsDto;
//     }
// }
