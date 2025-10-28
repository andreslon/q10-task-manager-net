import { Routes } from '@angular/router';
import { DashboardComponent } from './views/dashboard/dashboard';
import { TasksComponent } from './views/tasks/tasks';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'tasks', component: TasksComponent },
  { path: '**', redirectTo: '/dashboard' }
];
