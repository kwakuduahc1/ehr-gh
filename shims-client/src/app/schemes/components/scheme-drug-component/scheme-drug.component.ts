import { Component, ChangeDetectionStrategy, input } from '@angular/core';

@Component({
    selector: 'app-scheme-drug',
    templateUrl: './scheme-drug.component.html',
    styleUrl: './scheme-drug.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: []
})
export class SchemeDrugComponent {
    schemeId = input.required<number>();
    schemeName = input.required<string>();
}
