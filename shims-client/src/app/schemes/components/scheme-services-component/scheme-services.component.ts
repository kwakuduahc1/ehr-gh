import { Component, ChangeDetectionStrategy, input } from '@angular/core';

@Component({
    selector: 'app-scheme-services',
    templateUrl: './scheme-services.component.html',
    styleUrl: './scheme-services.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: []
})
export class SchemeServicesComponent {
    schemeId = input.required<number>();
    schemeName = input.required<string>();
}
