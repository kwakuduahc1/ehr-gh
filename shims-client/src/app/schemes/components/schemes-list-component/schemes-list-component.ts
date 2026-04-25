import { Component, inject, model } from '@angular/core';
import { AddSchemeDto, SchemesDTO, UpdateSchemeDto } from '../../../models/ISchemes';
import { MatAnchor, MatButton } from "@angular/material/button";
import { MatIcon } from "@angular/material/icon";
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { AddSchemeComponent } from '../add-scheme-component/add-scheme.component';
import { filter, iif, map, switchMap, tap } from 'rxjs';
import { SchemesHttpService } from '../../schemes-http.service';
import { ConfirmationComponent } from '../../../components/confirmation/confirmation.component';

@Component({
  selector: 'app-schemes-component',
  imports: [MatAnchor, MatIcon, MatButton, MatIcon],
  templateUrl: './schemes-list-component.html',
  styleUrl: './schemes-list-component.scss',
})
export class SchemesListComponent {
  schemes = model.required<SchemesDTO[]>();
  private snack = inject(MatSnackBar);
  private diag = inject(MatDialog);
  private http = inject(SchemesHttpService)

  addScheme(form?: UpdateSchemeDto) {
    let scheme: UpdateSchemeDto
    let isEdit: boolean
    this.diag.open<AddSchemeComponent, {}, { scheme: UpdateSchemeDto, edit: boolean }>(
      AddSchemeComponent,
      {
        data: { scheme: form }
      }
    )
      .afterClosed()
      .pipe(
        filter(x => !!x),
        tap(x => {
          scheme = x?.scheme!;
          isEdit = x?.edit!;
        }),
        switchMap(x => iif(() => !!x?.edit,
          this.http.updateScheme({
            schemesID: x?.scheme?.schemesID!,
            coverage: x?.scheme?.coverage!,
            maxPayable: x?.scheme?.maxPayable!,
            recovery: x?.scheme?.recovery!,
            schemeName: x?.scheme?.schemeName!
          }),
          this.http.createScheme(x?.scheme!)))
      )
      .subscribe({
        next: (res) => {
          this.snack.open(isEdit ? `Scheme updated` : `New scheme added`);
          this.schemes.update(x => {
            if (isEdit) {
              // Update existing scheme
              return x.map(s => s.schemesID === scheme.schemesID ? {
                coverage: scheme.coverage,
                maxPayable: scheme.maxPayable,
                recovery: scheme.recovery,
                schemeName: scheme.schemeName,
                schemesID: scheme.schemesID!
              } : s);
            } else {
              // Add new scheme
              return [{
                coverage: scheme.coverage,
                maxPayable: scheme.maxPayable,
                recovery: scheme.recovery,
                schemeName: scheme.schemeName,
                schemesID: res || scheme.schemesID!
              }, ...x];
            }
          });
        },
        error: (err) => {
          this.addScheme(scheme)
        },
      })
  }

  deleteScheme(id: string) {
    this.diag.open<ConfirmationComponent, {}, boolean>(ConfirmationComponent, {
      data: 'Are you sure you want to delete this scheme?'
    })
      .afterClosed()
      .pipe(
        filter(x => !!x),
        switchMap(() => this.http.deleteScheme(id))
      )
      .subscribe({
        next: () => {
          this.schemes.update(x => x.filter(s => s.schemesID !== id));
          this.snack.open('Scheme deleted');
        }
      })
  }
}
