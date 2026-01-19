import { Routes } from '@angular/router';
import { CandidateListComponent } from './features/candidates/components/candidate-list/candidate-list.component';
import { CandidateFormComponent } from './features/candidates/components/candidate-form/candidate-form.component';
import { HomeComponent } from './features/home/home.component'; // Importe a Home

export const routes: Routes = [
  { path: 'home', component: HomeComponent },

  { path: 'candidates', component: CandidateListComponent },
  { path: 'candidates/new', component: CandidateFormComponent },
  { path: 'candidates/edit/:id', component: CandidateFormComponent },

  { path: '', redirectTo: '/home', pathMatch: 'full' } 
];