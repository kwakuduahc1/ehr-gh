import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { VitalsDTO } from '../../models/vitals/IVitals';
import { VitalsHttpService } from '../../vitals/vitals-http.service';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ConsultingVitalsHttpService {
    private http = inject(HttpClient);
    url = environment.AppUrl + 'Vitals';

    list(id: string, n: number = 20): Observable<VitalsDTO> {
        return this.http.get<VitalsDTO>(`${this.url}/${id}/${n}`);
    }
}
