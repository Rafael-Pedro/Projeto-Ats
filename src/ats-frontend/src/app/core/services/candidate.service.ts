import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Candidate } from '../models/candidate.model';
import { PagedResult } from '../models/paged-result.model';
import { environment } from '../../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CandidateService {
    private readonly _http = inject(HttpClient);
    private _apiUrl = `${environment.apiUrl}/candidates`;

    /**
     * Busca candidatos paginados
     * @param page Número da página (padrão 1)
     * @param pageSize Tamanho da página (padrão 10)
     */
    getAll(page: number = 1, pageSize: number = 10): Observable<PagedResult<Candidate>> {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        let response =  this._http.get<PagedResult<Candidate>>(this._apiUrl, { params });
        return response;
    }

    getById(id: string): Observable<Candidate> {
        let response = this._http.get<Candidate>(`${this._apiUrl}/${id}`);
        return response;
    }

    create(candidate: any): Observable<Candidate> {
        let response = this._http.post<Candidate>(this._apiUrl, candidate);
        return response;
    }

    update(id: string, candidate: any): Observable<void> {
        const payload = { ...candidate, id };
        let response = this._http.put<void>(`${this._apiUrl}/${id}`, payload);
        return response;
    }

    disable(id: string): Observable<void> {
        let response = this._http.delete<void>(`${this._apiUrl}/${id}`);
        return response;
    }
}