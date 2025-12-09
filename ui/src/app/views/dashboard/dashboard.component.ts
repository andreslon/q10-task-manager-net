import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../services/task.service';
import { TaskItem } from '../../models/task-item.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  totalTasks = 0;
  recentTasks: TaskItem[] = [];
  loading = true;
  error = '';

  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.taskService.getAllTasks().subscribe({
      next: (tasks) => {
        this.totalTasks = tasks.length;
        this.recentTasks = tasks.slice(0, 5);
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Error loading dashboard: ' + error.message;
        this.loading = false;
      }
    });
  }
}

