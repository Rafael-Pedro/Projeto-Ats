import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { PoPageModule, PoWidgetModule } from '@po-ui/ng-components';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [PoPageModule, PoWidgetModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  private router = inject(Router);

  goToNewCandidate() {
    this.router.navigate(['/candidates']);
  }
}