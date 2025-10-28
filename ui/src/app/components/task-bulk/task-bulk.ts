import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskBulkRequest, TaskBulkResponse } from '../../models/task-bulk.model';
import { TaskBulkService } from '../../services/task-bulk.service';

@Component({
  selector: 'app-task-bulk',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-bulk.html',
  styleUrl: './task-bulk.scss'
})
export class TaskBulkComponent implements OnInit {
  tasks: TaskBulkRequest[] = [];
  results: TaskBulkResponse[] = [];
  isProcessing = false;
  error: string | null = null;
  success: string | null = null;

  constructor(private readonly taskBulkService: TaskBulkService) {}

  ngOnInit(): void {
    this.addEmptyTask();
  }

  addEmptyTask(): void {
    this.tasks.push({
      id: '',
      title: '',
      description: ''
    });
  }

  removeTask(index: number): void {
    if (this.tasks.length > 1) {
      this.tasks.splice(index, 1);
    }
  }

  processBulkTasks(): void {
    const validTasks = this.tasks.filter(task => task.title.trim());
    
    if (validTasks.length === 0) {
      this.error = 'Please add at least one task with a title';
      return;
    }

    this.isProcessing = true;
    this.error = null;
    this.success = null;

    this.taskBulkService.createBulkTasks(validTasks).subscribe({
      next: (taskIds) => {
        this.success = `Successfully created ${taskIds.length} tasks`;
        this.isProcessing = false;
        this.tasks = [];
        this.addEmptyTask();
      },
      error: (error) => {
        this.error = 'Error processing bulk tasks: ' + error.message;
        this.isProcessing = false;
      }
    });
  }

  clearResults(): void {
    this.results = [];
    this.error = null;
    this.success = null;
  }
}
