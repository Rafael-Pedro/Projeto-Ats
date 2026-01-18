import { Component, inject, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
    PoDynamicFormField,
    PoDynamicFormComponent,
    PoPageModule,
    PoDynamicModule,
    PoNotificationService
} from '@po-ui/ng-components';
import { CandidateService } from '../../../../core/services/candidate.service';

@Component({
    selector: 'app-candidate-form',
    standalone: true,
    imports: [PoPageModule, PoDynamicModule],
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

    readonly fields: Array<PoDynamicFormField> = [
        { property: 'name', label: 'Nome Completo', divider: 'DADOS PESSOAIS', required: true, gridColumns: 6 },
        { property: 'email', label: 'E-mail', required: true, gridColumns: 6, icon: 'po-icon-mail' },
        { property: 'age', label: 'Idade', type: 'number', gridColumns: 2 },
        { property: 'resume', label: 'Currículo (Link ou Texto)', gridColumns: 10, rows: 3 }
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

    save() {
        const candidateData = this.dynamicForm.value;

        if (this.candidateId) {
            this.candidateService.update(this.candidateId, candidateData).subscribe({
                next: () => {
                    this.notification.success('Candidato atualizado com sucesso!');
                    this.router.navigate(['/candidates']);
                },
                error: () => this.notification.error('Erro ao atualizar.')
            });
        } else {
            this.candidateService.create(candidateData).subscribe({
                next: () => {
                    this.notification.success('Candidato cadastrado com sucesso!');
                    this.router.navigate(['/candidates']);
                },
                error: () => this.notification.error('Erro ao salvar.')
            });
        }
    }

    cancel() {
        this.router.navigate(['/candidates']);
    }
}