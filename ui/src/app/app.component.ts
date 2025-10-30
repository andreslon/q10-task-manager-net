import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Task Manager UI';
  healthStatus = '';
  configValue = '';

  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.checkHealth();
    this.getConfig();
  }

  checkHealth() {
    this.taskService.checkHealth().subscribe({
      next: (status) => {
        this.healthStatus = status;
      },
      error: (error) => {
        this.healthStatus = 'Error: ' + error.message;
      }
    });
  }

  getConfig() {
    this.taskService.getConfig().subscribe({
      next: (config) => {
        this.configValue = config;
      },
      error: (error) => {
        this.configValue = 'Error: ' + error.message;
      }
    });
  }
}

