import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApplicationStatus } from '../enums/application-status.enum';

export interface ApplicationResponse {
    applicationId: string;
    candidateId: string;
    candidateName: string;
    candidateEmail: string;
    status: ApplicationStatus;
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

    changeStatus(applicationId: string, action: 'interview' | 'reject'): Observable<void> {
        return this._http.patch<void>(`${this._apiUrl}/${applicationId}/status`, `"${action}"`, {
            headers: { 'Content-Type': 'application/json' }
        });
    }
}