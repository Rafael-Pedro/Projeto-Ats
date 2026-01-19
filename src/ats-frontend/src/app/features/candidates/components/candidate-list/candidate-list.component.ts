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
  
  // 1. Variáveis de Controle de Paginação
  page = 1;
  readonly pageSize = 10;
  hasNext = false; // Controla se o botão "Carregar Mais" deve aparecer

  readonly pageActions: Array<PoPageAction> = [
    { label: 'Novo Candidato', action: () => this.onCreate(), icon: 'po-icon-user-add' }
  ];

  readonly tableActions: Array<PoTableAction> = [
    { label: 'Editar', action: (row: Candidate) => this.onEdit(row), icon: 'po-icon-edit' },
    { label: 'Excluir', action: (row: Candidate) => this.onDelete(row), type: 'danger', icon: 'po-icon-delete' }
  ];

  readonly columns: Array<PoTableColumn> = [
    { property: 'name', label: 'Nome' },
    { property: 'email', label: 'E-mail' },
    { property: 'age', label: 'Idade', type: 'number' },
    { property: 'createdAt', label: 'Data de Cadastro', type: 'date' }
  ];

  ngOnInit() {
    // Carrega a primeira página
    this.loadData();
  }

  // 2. Método ajustado para suportar paginação
  loadData(isShowMore: boolean = false) {
    this.isLoading = true;
    
    // Se NÃO for "Carregar Mais", reseta para a página 1 (ex: após excluir ou iniciar)
    if (!isShowMore) {
      this.page = 1;
    }

    this.candidateService.getAll(this.page, this.pageSize).subscribe({
      next: (res) => {
        // Se for paginação, ADICIONA ao final da lista. Se for refresh, SUBSTITUI a lista.
        if (isShowMore) {
          this.candidates = [...this.candidates, ...res.items];
        } else {
          this.candidates = res.items;
        }

        // Lógica para saber se ainda tem mais itens no banco
        // Se o total que temos na tela for menor que o total do banco, tem mais.
        this.hasNext = this.candidates.length < res.totalCount;
        
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Erro ao carregar dados.');
      }
    });
  }

  // 3. Evento disparado pelo botão do PO-UI
  onShowMore() {
    this.page++; // Incrementa a página
    this.loadData(true); // Chama passando true para concatenar
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
        // Ao excluir, recarregamos do zero para garantir consistência
        this.loadData(false); 
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Não foi possível inativar o candidato.');
      }
    });
  }
}