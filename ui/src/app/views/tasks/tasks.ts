import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskListComponent } from '../../components/task-list/task-list';
import { TaskFormComponent } from '../../components/task-form/task-form';
import { TaskBulkComponent } from '../../components/task-bulk/task-bulk';
import { CreateTaskRequest } from '../../models/task-item.model';
import { TaskService } from '../../services/task.service';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, TaskListComponent, TaskFormComponent, TaskBulkComponent],
  templateUrl: './tasks.html',
  styleUrl: './tasks.scss'
})
export class TasksComponent {
  activeTab: 'list' | 'create' | 'bulk' = 'list';

  constructor(private readonly taskService: TaskService) {}

  onTaskCreated(task: CreateTaskRequest): void {
    this.taskService.createTask(task).subscribe({
      next: () => {
        this.activeTab = 'list';
        // The task list will refresh automatically
      },
      error: (error) => {
        console.error('Error creating task:', error);
      }
    });
  }

  setActiveTab(tab: 'list' | 'create' | 'bulk'): void {
    this.activeTab = tab;
  }
}
