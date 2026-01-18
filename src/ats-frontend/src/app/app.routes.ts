import { Routes } from '@angular/router';
import { CandidateListComponent } from './features/candidates/components/candidate-list/candidate-list.component';
import { CandidateFormComponent } from './features/candidates/components/candidate-form/candidate-form.component';

export const routes: Routes = [
  { path: 'candidates', component: CandidateListComponent },
  { path: 'candidates/new', component: CandidateFormComponent },
  { path: 'candidates/edit/:id', component: CandidateFormComponent },
  { path: '', redirectTo: '/candidates', pathMatch: 'full' }
];