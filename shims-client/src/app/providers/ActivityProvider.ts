import { computed, Injectable, signal } from '@angular/core';

interface IStates { isProcessing: boolean, message?: string }

@Injectable({ providedIn: 'root' })
export class ActivityProvider {
  private _act = signal<IStates>({ isProcessing: false, message: '' })
  act = computed<IStates>(() => this._act())
  beginProc() {
    this._act.update(() => {
      return {
        isProcessing: true
      }
    })
  }


  endProc() {
    this._act.update(() => {
      return {
        isProcessing: false
      }
    })
  }
}
