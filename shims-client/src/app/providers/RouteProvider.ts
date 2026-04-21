import { Injectable, effect, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class RouteProvider {
  path = signal<string | null>(null)

  // constructor() {
  //   effect(() => console.log(this.path()))
  // }
}
