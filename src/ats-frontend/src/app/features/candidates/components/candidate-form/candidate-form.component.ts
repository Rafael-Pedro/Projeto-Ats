import { Component, inject, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  PoDynamicFormField,
  PoDynamicFormComponent,
  PoPageModule,
  PoDynamicModule,
  PoNotificationService,
  PoFieldModule,
  PoUploadFile
} from '@po-ui/ng-components';
import { CandidateService } from '../../../../core/services/candidate.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-candidate-form',
  standalone: true,
  imports: [PoPageModule, PoDynamicModule, PoFieldModule, FormsModule],
  templateUrl: './candidate-form.component.html'
})
export class CandidateFormComponent {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private candidateService = inject(CandidateService);
  private notification = inject(PoNotificationService);

  @ViewChild('dynamicForm', { static: true }) dynamicForm!: PoDynamicFormComponent;

  title = 'Novo Candidato';
  candidateId: string | null = null;
  candidate: any = {};

  uploadFiles: Array<PoUploadFile> = [];
  resumeFile: File | null = null; 

  readonly fields: Array<PoDynamicFormField> = [
    { property: 'name', label: 'Nome Completo', divider: 'DADOS PESSOAIS', required: true, gridColumns: 6 },
    { property: 'email', label: 'E-mail', required: true, gridColumns: 6, icon: 'po-icon-mail' },
    { property: 'age', label: 'Idade', type: 'number', gridColumns: 2 },
    { property: 'linkedIn', label: 'LinkedIn', gridColumns: 10 } 
  ];

  ngOnInit() {
    this.candidateId = this.route.snapshot.paramMap.get('id');

    if (this.candidateId) {
      this.title = 'Editar Candidato';
      this.loadCandidate(this.candidateId);
    }
  }

  loadCandidate(id: string) {
    this.candidateService.getById(id).subscribe({
      next: (data) => {
        this.candidate = data;
      },
      error: () => {
        this.notification.error('Erro ao carregar candidato');
        this.cancel();
      }
    });
  }

  onFileChange(files: Array<PoUploadFile>) {
    this.uploadFiles = files; 

    if (files && files.length > 0) {
      this.resumeFile = files[0].rawFile; 
    } else {
      this.resumeFile = null;
    }
  }

  save() {
    // Validação do formulário
    if (this.dynamicForm.form.invalid) {
      this.notification.warning('Por favor, preencha os campos obrigatórios.');
      return;
    }

    const candidateData = this.dynamicForm.value;

    if (this.candidateId) {
      this.candidateService.update(this.candidateId, candidateData, this.resumeFile).subscribe({
        next: () => {
          this.notification.success('Candidato atualizado com sucesso!');
          this.router.navigate(['/candidates']);
        },
        error: (err) => {
          console.error('Erro na atualização:', err);
          this.notification.error('Erro ao atualizar candidato.');
        }
      });

    } else {
      this.candidateService.create(candidateData, this.resumeFile).subscribe({
        next: () => {
          this.notification.success('Candidato cadastrado com sucesso!');
          this.router.navigate(['/candidates']);
        },
        error: (err) => {
          console.error('Erro na criação:', err);
          this.notification.error('Erro ao salvar candidato.');
        }
      });
    }
  }
  cancel() {
    this.router.navigate(['/candidates']);
  }
}