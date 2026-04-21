import { Component, inject, signal, computed } from '@angular/core';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    host: {
        'style': 'display: flex; width: 100%; height: 100%; margin: 0; padding: 0;'
    },
    imports: []
})
export class HomeComponent {

}
