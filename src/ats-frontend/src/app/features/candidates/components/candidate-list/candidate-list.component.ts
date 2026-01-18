import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import {
  PoPageModule,
  PoTableModule,
  PoTableAction,
  PoTableColumn,
  PoPageAction,
  PoNotificationService,
  PoDialogService
} from '@po-ui/ng-components';
import { CandidateService } from '../../../../core/services/candidate.service';
import { Candidate } from '../../../../core/models/candidate.model';

@Component({
  selector: 'app-candidate-list',
  standalone: true,
  imports: [CommonModule, PoPageModule, PoTableModule],
  templateUrl: './candidate-list.component.html'
})
export class CandidateListComponent implements OnInit {
  private candidateService = inject(CandidateService);
  private router = inject(Router);
  private poDialog = inject(PoDialogService);
  private notification = inject(PoNotificationService);

  candidates: Array<Candidate> = [];
  isLoading = false;

  readonly pageActions: Array<PoPageAction> = [
    { label: 'Novo Candidato', action: () => this.onCreate(), icon: 'po-icon-user-add' }
  ];

  readonly tableActions: Array<PoTableAction> = [
    { label: 'Editar', action: (row: Candidate) => this.onEdit(row), icon: 'po-icon-edit' },
    { label: 'Excluir', action: (row: Candidate) => this.onDisable(row), type: 'danger', icon: 'po-icon-delete' }
  ];

  readonly columns: Array<PoTableColumn> = [
    { property: 'name', label: 'Nome' },
    { property: 'email', label: 'E-mail' },
    { property: 'age', label: 'Idade', type: 'number' },
    { property: 'createdAt', label: 'Data de Cadastro', type: 'date' }
  ];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    this.candidateService.getAll(1, 10).subscribe({
      next: (res) => {
        this.candidates = res.items;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
    });
  }

  private onCreate() {
    this.router.navigate(['/candidates/new']);
  }

  private onEdit(item: Candidate) {
    this.router.navigate(['/candidates/edit', item.id]);
  }

  private onDisable(item: Candidate) {
    this.poDialog.confirm({
      title: 'Excluir Candidato',
      message: `Tem certeza que deseja excluir o candidato ${item.name}?`,
      confirm: () => this.confirmDelete(item.id),
      cancel: () => { }
    });
  }

  private confirmDelete(id: string) {
    this.candidateService.disable(id).subscribe({
      next: () => {
        this.notification.success('Candidato excluído com sucesso');
        this.loadData();
      },
      error: () => this.notification.error('Erro ao excluir candidato')
    });
  }

}