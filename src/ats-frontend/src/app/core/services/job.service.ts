import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/paged-result.model';
import { Job } from '../models/job.model';

@Injectable({
  providedIn: 'root'
})
export class JobService {
  private _http = inject(HttpClient);
  private _apiUrl = `${environment.apiUrl}/jobs`;

  getAll(page: number = 1, pageSize: number = 10, onlyActive: boolean = false): Observable<PagedResult<Job>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('onlyActive', onlyActive.toString());

    return this._http.get<PagedResult<Job>>(this._apiUrl, { params });
  }

  delete(id: string): Observable<void> {
    return this._http.delete<void>(`${this._apiUrl}/${id}`);
  }
  
  create(job: any): Observable<void> {
    return this._http.post<void>(this._apiUrl, job);
  }

  getById(id: string): Observable<Job> {
    return this._http.get<Job>(`${this._apiUrl}/${id}`);
  }

  update(id: string, job: any): Observable<void> {
    return this._http.put<void>(`${this._apiUrl}/${id}`, job);
  }
}