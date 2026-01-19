import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { CandidateListComponent } from './features/candidates/components/candidate-list/candidate-list.component';
import { CandidateFormComponent } from './features/candidates/components/candidate-form/candidate-form.component';
import { JobListComponent } from './features/jobs/components/job-list/job-list.component';
import { JobFormComponent } from './features/jobs/components/job-form/job-form.component';
import { JobApplicantsComponent } from './features/jobs/components/job-applicants/job-applicants.component';

export const routes: Routes = [
    { path: 'home', component: HomeComponent },
    { path: '', redirectTo: '/home', pathMatch: 'full' },


    { path: 'candidates', component: CandidateListComponent },
    { path: 'candidates/new', component: CandidateFormComponent },
    { path: 'candidates/edit/:id', component: CandidateFormComponent },


    { path: 'jobs', component: JobListComponent },
    { path: 'jobs/new', component: JobFormComponent },
    { path: 'jobs/edit/:id', component: JobFormComponent },
    
    { path: 'jobs/:id/applicants', component: JobApplicantsComponent }
];