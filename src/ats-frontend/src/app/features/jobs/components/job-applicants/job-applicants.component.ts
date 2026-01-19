import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import {
  PoPageModule,
  PoTableModule,
  PoTableColumn,
  PoBreadcrumb,
  PoNotificationService,
  PoButtonModule,
  PoTableAction
} from '@po-ui/ng-components';
import { JobApplicationService, ApplicationResponse } from '../../../../core/services/job-application.service';
import { ApplicationStatus } from '../../../../core/enums/application-status.enum';

@Component({
  selector: 'app-job-applicants',
  standalone: true,
  imports: [
    CommonModule,
    PoPageModule,
    PoTableModule,
    PoButtonModule],
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
        {
          value: ApplicationStatus.Applied,
          color: 'color-01',
          label: 'Inscrito'
        },
        {
          value: ApplicationStatus.Reviewing,
          color: 'color-05',
          label: 'Em Análise'
        },
        {
          value: ApplicationStatus.Interviewing,
          color: 'color-08',
          label: 'Entrevista'
        },
        {
          value: ApplicationStatus.Rejected,
          color: 'color-07',
          label: 'Reprovado'
        },
        {
          value: ApplicationStatus.Hired,
          color: 'color-10',
          label: 'Contratado'
        }
      ]
    }
  ];

  readonly tableActions: Array<PoTableAction> = [
    {
      label: 'Marcar Entrevista',
      icon: 'po-icon-calendar',
      action: (row: ApplicationResponse) => this.updateStatus(row.applicationId, 'interview'),

      visible: (row: ApplicationResponse) =>
        row.status === ApplicationStatus.Applied ||
        row.status === ApplicationStatus.Reviewing
    },
    {
      label: 'Reprovar Candidato',
      icon: 'po-icon-close',
      type: 'danger',
      action: (row: ApplicationResponse) => this.updateStatus(row.applicationId, 'reject'),

      visible: (row: ApplicationResponse) =>
        row.status !== ApplicationStatus.Hired &&
        row.status !== ApplicationStatus.Rejected
    }
  ];

  updateStatus(id: string, action: 'interview' | 'reject') {
    this.isLoading = true;
    this.applicationService.changeStatus(id, action).subscribe({
      next: () => {
        this.poNotification.success('Status atualizado com sucesso!');
        this.loadData(this.jobId!);
      },
      error: (err) => {
        this.isLoading = false;
        this.poNotification.error(err.error?.detail || 'Erro ao mudar status');
      }
    });
  }

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