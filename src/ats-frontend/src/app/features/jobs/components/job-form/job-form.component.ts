import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { 
  PoPageModule, 
  PoFieldModule, 
  PoButtonModule, 
  PoNotificationService,
  PoBreadcrumb 
} from '@po-ui/ng-components';
import { JobService } from '../../../../core/services/job.service';

@Component({
  selector: 'app-job-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    PoPageModule, 
    PoFieldModule, 
    PoButtonModule
  ],
  templateUrl: './job-form.component.html'
})
export class JobFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private poNotification = inject(PoNotificationService);

  form!: FormGroup;
  isEditing = false;
  jobId: string | null = null;
  isLoading = false;

  public readonly breadcrumb: PoBreadcrumb = {
    items: [
      { label: 'Vagas', link: '/jobs' },
      { label: 'Nova Vaga' }
    ]
  };

  ngOnInit() {
    this.initForm();
    
    // Verifica se tem ID na URL (para edição futura)
    // this.jobId = this.route.snapshot.paramMap.get('id');
    // if (this.jobId) {
    //   this.isEditing = true;
    //   this.breadcrumb.items[1].label = 'Editar Vaga';
    //   this.loadData(this.jobId);
    // }
  }

  private initForm() {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      salary: [null] // Salário é opcional
    });
  }

  save() {
    if (this.form.invalid) {
      this.poNotification.warning('Por favor, preencha os campos obrigatórios.');
      return;
    }

    this.isLoading = true;
    const jobData = this.form.value;

    this.jobService.create(jobData).subscribe({
      next: () => {
        this.poNotification.success('Vaga criada com sucesso!');
        this.router.navigate(['/jobs']);
      },
      error: () => {
        this.isLoading = false;
        this.poNotification.error('Erro ao salvar a vaga.');
      }
    });
  }

  cancel() {
    this.router.navigate(['/jobs']);
  }
}