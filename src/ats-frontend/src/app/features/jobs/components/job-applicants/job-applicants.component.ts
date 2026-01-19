import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { 
  PoPageModule, 
  PoTableModule, 
  PoTableColumn, 
  PoBreadcrumb,
  PoNotificationService,
  PoButtonModule
} from '@po-ui/ng-components';
import { JobApplicationService, ApplicationResponse } from '../../../../core/services/job-application.service';

@Component({
  selector: 'app-job-applicants',
  standalone: true,
  imports: [CommonModule, PoPageModule, PoTableModule, PoButtonModule],
  templateUrl: './job-applicants.component.html'
})
export class JobApplicantsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private applicationService = inject(JobApplicationService);
  private poNotification = inject(PoNotificationService);

  applicants: ApplicationResponse[] = [];
  isLoading = false;
  jobId: string | null = null;

  public readonly breadcrumb: PoBreadcrumb = {
    items: [
      { label: 'Vagas', link: '/jobs' },
      { label: 'Candidatos Inscritos' }
    ]
  };

  readonly columns: Array<PoTableColumn> = [
    { property: 'candidateName', label: 'Nome' },
    { property: 'candidateEmail', label: 'E-mail' },
    { property: 'appliedAt', label: 'Data Aplicação', type: 'date' },
    { 
      property: 'status', 
      label: 'Status', 
      type: 'label',
      labels: [
        { value: 0, color: 'color-07', label: 'Inscrito' },
        { value: 1, color: 'color-01', label: 'Em Análise' },
        { value: 2, color: 'color-08', label: 'Entrevista' },
        { value: 3, color: 'color-10', label: 'Contratado' },
        { value: 4, color: 'color-07', label: 'Reprovado' }
      ]
    }
  ];

  ngOnInit() {
    this.jobId = this.route.snapshot.paramMap.get('id');
    if (this.jobId) {
      this.loadData(this.jobId);
    }
  }

  loadData(id: string) {
    this.isLoading = true;
    this.applicationService.getCandidatesByJob(id).subscribe({
      next: (data) => {
        this.applicants = data;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Erro ao carregar candidatos.');
      }
    });
  }

  back() {
    this.router.navigate(['/jobs']);
  }
}