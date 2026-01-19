import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ApplicationResponse {
    applicationId: string;
    candidateId: string;
    candidateName: string;
    candidateEmail: string;
    status: number;
    appliedAt: string;
}

@Injectable({
    providedIn: 'root'
})
export class JobApplicationService {
    private _http = inject(HttpClient);
    private _apiUrl = `${environment.apiUrl}/job-applications`;

    apply(candidateId: string, jobId: string): Observable<void> {
        return this._http.post<void>(this._apiUrl, { candidateId, jobId });
    }

    getCandidatesByJob(jobId: string): Observable<ApplicationResponse[]> {
        return this._http.get<ApplicationResponse[]>(`${this._apiUrl}/job/${jobId}`);
    }
}