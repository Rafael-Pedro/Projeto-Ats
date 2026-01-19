import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import {
  PoPageModule,
  PoTableModule,
  PoTableAction,
  PoTableColumn,
  PoPageAction,
  PoDialogService,
  PoNotificationService,
  PoButtonModule,
  PoFieldModule
} from '@po-ui/ng-components';
import { Job } from '../../../../core/models/job.model';
import { JobService } from '../../../../core/services/job.service';
import { JobApplicationService } from '../../../../core/services/job-application.service';
import { UserContextService } from '../../../../core/services/user-context.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-job-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    PoPageModule,
    PoTableModule,
    PoButtonModule,
    PoFieldModule
  ],
  templateUrl: './job-list.component.html'
})
export class JobListComponent implements OnInit {
  private jobService = inject(JobService);
  private applicationService = inject(JobApplicationService);
  private userContext = inject(UserContextService);

  private router = inject(Router);
  private poDialog = inject(PoDialogService);
  private poNotification = inject(PoNotificationService);

  readonly candidateServiceUrl = `${environment.apiUrl}/candidates`;

  isRecruiter = false;
  selectedCandidateId: string | null = null;

  jobs: Array<Job> = [];
  isLoading = false;
  page = 1;
  pageSize = 10;
  hasNext = false;

  get pageActions(): Array<PoPageAction> {
    return [
      {
        label: 'Nova Vaga',
        action: () => this.onCreate(),
        icon: 'po-icon-plus',
        visible: this.isRecruiter
      }
    ];
  }

  readonly tableActions: Array<PoTableAction> = [
    {
      label: 'Candidatar-se',
      action: (row: Job) => this.onApply(row),
      icon: 'po-icon-handshake',
      visible: (row: Job) => !this.isRecruiter && row.isActive
    },
    {
      label: 'Ver Inscritos',
      action: (row: Job) => this.onViewApplicants(row),
      icon: 'po-icon-users',
      visible: (row: Job) => this.isRecruiter
    },
    {
      label: 'Editar',
      action: (row: Job) => this.onEdit(row),
      icon: 'po-icon-edit',
      visible: (row: Job) => this.isRecruiter
    },
    {
      label: 'Excluir',
      action: (row: Job) => this.onDelete(row),
      type: 'danger',
      icon: 'po-icon-delete',
      visible: (row: Job) => this.isRecruiter
    }
];

  readonly columns: Array<PoTableColumn> = [
    { property: 'title', label: 'Título' },
    { property: 'salary', label: 'Salário', type: 'currency', format: 'BRL' },
    { property: 'createdAt', label: 'Criada em', type: 'date' },
    {
      property: 'isActive',
      label: 'Status',
      type: 'label',
      width: '100px',
      labels: [
        { value: 'true', color: 'color-10', label: 'Aberta' },
        { value: 'false', color: 'color-07', label: 'Fechada' }
      ]
    }
  ];

  ngOnInit() {
    this.selectedCandidateId = this.userContext.currentUserId;
    this.loadData();
  }

  onChangeProfile() {
    // Ao trocar de perfil, recarrega os dados.
    // Recrutador vê tudo (Active = false), Candidato vê só ativas (Active = true)
    this.page = 1;
    this.loadData();
  }

  onCandidateChange(id: string) {
    this.selectedCandidateId = id;

    if (id) {
      this.userContext.setCurrentUser(id);
      this.poNotification.information('Usuário alterado. Agora você está "logado" como este candidato.');
    } else {
      this.userContext.clearUser();
    }
  }

  loadData(isLoadMore: boolean = false) {
    this.isLoading = true;
    if (!isLoadMore) this.page = 1;

    // Se for Recrutador -> Traz todas (onlyActive = false)
    // Se for Candidato -> Traz só ativas (onlyActive = true)
    const onlyActive = !this.isRecruiter;

    this.jobService.getAll(this.page, this.pageSize, onlyActive).subscribe({
      next: (res) => {
        if (isLoadMore) {
          this.jobs = [...this.jobs, ...res.items];
        } else {
          this.jobs = res.items;
        }
        this.hasNext = this.jobs.length < res.totalCount;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Erro ao carregar vagas');
      }
    });
  }

  private onApply(job: Job) {
    const candidateId = this.userContext.currentUserId;

    if (!candidateId) {
      this.poNotification.warning('Por favor, selecione um candidato na lista acima para simular a aplicação!');
      return;
    }

    this.poDialog.confirm({
      title: 'Confirmar Candidatura',
      message: `Deseja se candidatar para a vaga de ${job.title}?`,
      confirm: () => {
        this.isLoading = true;

        this.applicationService.apply(candidateId, job.id).subscribe({
          next: () => {
            this.isLoading = false;
            this.poNotification.success(`Sucesso! Você se candidatou para ${job.title}.`);
          },
          error: (err) => {
            this.isLoading = false;
            const errorMsg = err.error?.detail || 'Erro ao realizar candidatura.';
            this.poNotification.error(errorMsg);
          }
        });
      }
    });
  }

private onViewApplicants(row: Job) {
  if (!row || !row.id) {
    this.poNotification.error('Erro: ID da vaga não encontrado.');
    return;
  }

  this.router.navigate(['/jobs', row.id, 'applicants']);
}

  onShowMore() {
    this.page++;
    this.loadData(true);
  }

  private onCreate() {
    this.router.navigate(['/jobs/new']);
  }

  private onEdit(item: Job) {
    // this.router.navigate(['/jobs/edit', item.id]);
  }

  private onDelete(item: Job) {
    this.poDialog.confirm({
      title: 'Excluir Vaga',
      message: `Tem certeza que deseja excluir a vaga ${item.title}?`,
      confirm: () => {
        this.jobService.delete(item.id).subscribe(() => {
          this.poNotification.success('Vaga excluída');
          this.loadData();
        });
      }
    });
  }
}