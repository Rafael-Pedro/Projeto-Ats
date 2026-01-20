import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  PoPageModule,
  PoTableModule,
  PoTableAction,
  PoTableColumn,
  PoPageAction,
  PoDialogService,
  PoNotificationService,
  PoButtonModule
} from '@po-ui/ng-components';
import { CandidateService } from '../../../../core/services/candidate.service';
import { Candidate } from '../../../../core/models/candidate.model';

@Component({
  selector: 'app-candidate-list',
  standalone: true,
  imports: [CommonModule, PoPageModule, PoTableModule, PoButtonModule],
  templateUrl: './candidate-list.component.html'
})
export class CandidateListComponent implements OnInit {
  private candidateService = inject(CandidateService);
  private router = inject(Router);
  private poDialog = inject(PoDialogService);
  private poNotification = inject(PoNotificationService);

  candidates: Array<Candidate> = [];
  isLoading = false;

  page = 1;
  readonly pageSize = 10;
  hasNext = false;

  readonly pageActions: Array<PoPageAction> = [
    { label: 'Novo Candidato', action: () => this.onCreate(), icon: 'po-icon-user-add' }
  ];

  readonly tableActions: Array<PoTableAction> = [
    { 
      label: 'Baixar Currículo', 
      action: (row: Candidate) => this.onDownload(row), 
      icon: 'po-icon-download', 
      disabled: (row: Candidate) => !row.resumeFileName 
    },
    { label: 'Editar', action: (row: Candidate) => this.onEdit(row), icon: 'po-icon-edit' },
    { label: 'Excluir', action: (row: Candidate) => this.onDelete(row), type: 'danger', icon: 'po-icon-delete' }
  ];

  readonly columns: Array<PoTableColumn> = [
    { property: 'name', label: 'Nome' },
    { property: 'email', label: 'E-mail' },
    { property: 'age', label: 'Idade', type: 'number' },

    {
      property: 'linkedIn',
      label: 'LinkedIn',
      type: 'link'
    },
    {
      property: 'resumeFileName',
      label: 'Arquivo',
      type: 'string'
    },

    { property: 'createdAt', label: 'Criado em', type: 'date' },
    {
      property: 'status',
      type: 'label',
      width: '8%',
      labels: [
        { value: 'active', color: 'color-10', label: 'Ativo' },
        { value: 'disabled', color: 'color-07', label: 'Inativo' }
      ]
    }
  ];

  ngOnInit() {
    this.loadData();
  }

  loadData(isShowMore: boolean = false) {
    this.isLoading = true;

    if (!isShowMore) {
      this.page = 1;
    }

    this.candidateService.getAll(this.page, this.pageSize).subscribe({
      next: (res) => {
        if (isShowMore) {
          this.candidates = [...this.candidates, ...res.items];
        } else {
          this.candidates = res.items;
        }

        this.hasNext = this.candidates.length < res.totalCount;

        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Erro ao carregar dados.');
      }
    });
  }

  onShowMore() {
    this.page++;
    this.loadData(true);
  }

  private onCreate() {
    this.router.navigate(['/candidates/new']);
  }

  private onEdit(item: Candidate) {
    this.router.navigate(['/candidates/edit', item.id]);
  }

  private onDelete(item: Candidate) {
    this.poDialog.confirm({
      title: 'Confirmar Exclusão',
      message: `Tem certeza que deseja inativar o candidato ${item.name}?`,
      confirm: () => this.confirmDisable(item.id)
    });
  }

  private confirmDisable(id: string) {
    this.isLoading = true;
    this.candidateService.disable(id).subscribe({
      next: () => {
        this.poNotification.success('Candidato inativado com sucesso!');
        this.loadData(false);
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Não foi possível inativar o candidato.');
      }
    });
  }

  private onDownload(item: Candidate) {
    if (!item.resumeFileName) {
      this.poNotification.warning('Candidato sem currículo.');
      return;
    }

    this.isLoading = true;

    this.candidateService.downloadResume(item.id).subscribe({
      next: (data: Blob) => {
        this.downloadFile(data, item.resumeFileName!);
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.poNotification.error('Erro ao baixar o arquivo.');
        this.isLoading = false;
      }
    });
  }

  private downloadFile(blob: Blob, fileName: string) {
    const url = window.URL.createObjectURL(blob);
    
    const a = document.createElement('a');
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);
    
    a.click();
    
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }
}