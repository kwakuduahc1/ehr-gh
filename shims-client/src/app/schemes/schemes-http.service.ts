import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SchemesDTO, AddSchemeDto, UpdateSchemeDto } from '../models/ISchemes';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SchemesHttpService {
    private readonly http = inject(HttpClient);
    private readonly baseUrl = `${environment.AppUrl}Schemes`;

    /**
     * Retrieves all schemes
     * }
     */
    getSchemes(): Observable<SchemesDTO[]> {
        return this.http.get<SchemesDTO[]>(this.baseUrl);
    }

    /**
     * Retrieves a single scheme by ID
     */
    getScheme(id: string): Observable<SchemesDTO> {
        return this.http.get<SchemesDTO>(`${this.baseUrl}/${id}`);
    }

    /**
     * Creates a new scheme
     */
    createScheme(scheme: AddSchemeDto): Observable<string> {
        return this.http.post<string>(this.baseUrl, scheme);
    }

    /**
     * Updates an existing scheme
     */
    updateScheme(scheme: UpdateSchemeDto): Observable<void> {
        return this.http.put<void>(this.baseUrl, scheme);
    }

    /**
     * Deletes a scheme by ID
     */
    deleteScheme(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}/${id}`);
    }
}
