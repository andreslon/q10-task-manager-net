import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./views/dashboard/dashboard.component').then(m => m.DashboardComponent)
  },
  {
    path: 'tasks',
    loadComponent: () => import('./views/tasks/tasks.component').then(m => m.TasksComponent)
  },
  {
    path: 'bulk',
    loadComponent: () => import('./views/bulk/bulk.component').then(m => m.BulkComponent)
  }
];

