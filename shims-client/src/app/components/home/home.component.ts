import { Component, inject, signal, computed, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  host: {
    style: 'display: flex; width: 100%; height: 100%; margin: 0; padding: 0;',
  },
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [],
})
export class HomeComponent {}
