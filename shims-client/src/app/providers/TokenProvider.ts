// import { Injectable, Signal, computed, inject } from '@angular/core';
// import { toSignal } from '@angular/core/rxjs-interop';
// import { StorageMap } from '@ngx-pwa/local-storage';

// @Injectable({ providedIn: 'root' })
// export class TokenProvider {
//   private store = inject(StorageMap);
//   token = toSignal(this.store.get<string>('jwt', { type: 'string' }));
//   setToken(value: string) {
//     return this.store.set('jwt', value);
//   }

//   getHeader = computed(() => `Bearer ${this.token()}`);
// }
