import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskBulkService } from '../../services/task-bulk.service';
import { TaskBulkRequest, TaskBulkResponse } from '../../models/task-bulk.model';

@Component({
  selector: 'app-bulk',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bulk.component.html',
  styleUrls: ['./bulk.component.scss']
})
export class BulkComponent {
  bulkTasks: TaskBulkRequest[] = [
    { title: '', description: '' }
  ];
  
  createdTaskIds: string[] = [];
  taskResults: TaskBulkResponse[] = [];
  loading = false;
  error = '';
  success = '';
  
  searchTaskId = '';
  searchResult: TaskBulkResponse | null = null;

  constructor(private bulkService: TaskBulkService) {}

  addTask() {
    this.bulkTasks.push({ title: '', description: '' });
  }

  removeTask(index: number) {
    if (this.bulkTasks.length > 1) {
      this.bulkTasks.splice(index, 1);
    }
  }

  createBulkTasks() {
    const validTasks = this.bulkTasks.filter(t => t.title.trim() !== '');
    
    if (validTasks.length === 0) {
      this.error = 'Please add at least one task with a title';
      return;
    }

    this.loading = true;
    this.error = '';
    this.success = '';
    
    this.bulkService.createBulkTasks(validTasks).subscribe({
      next: (taskIds) => {
        this.createdTaskIds = taskIds;
        this.success = `Successfully created ${taskIds.length} tasks!`;
        this.bulkTasks = [{ title: '', description: '' }];
        this.loading = false;
        this.checkBulkResults();
      },
      error: (error) => {
        this.error = 'Error creating bulk tasks: ' + error.message;
        this.loading = false;
      }
    });
  }

  checkBulkResults() {
    this.taskResults = [];
    let completed = 0;
    
    this.createdTaskIds.forEach(taskId => {
      this.bulkService.getTaskById(taskId).subscribe({
        next: (result) => {
          this.taskResults.push(result);
          completed++;
          if (completed === this.createdTaskIds.length) {
            this.sortResults();
          }
        },
        error: (error) => {
          this.taskResults.push({
            taskId: taskId,
            title: 'Unknown',
            isSuccess: false,
            errorMessage: error.message
          });
          completed++;
          if (completed === this.createdTaskIds.length) {
            this.sortResults();
          }
        }
      });
    });
  }

  sortResults() {
    this.taskResults.sort((a, b) => {
      if (a.isSuccess === b.isSuccess) return 0;
      return a.isSuccess ? -1 : 1;
    });
  }

  searchTask() {
    if (!this.searchTaskId.trim()) {
      this.error = 'Please enter a task ID';
      return;
    }

    this.loading = true;
    this.error = '';
    this.searchResult = null;

    this.bulkService.getTaskById(this.searchTaskId).subscribe({
      next: (result) => {
        this.searchResult = result;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Error searching task: ' + error.message;
        this.loading = false;
      }
    });
  }

  clearResults() {
    this.createdTaskIds = [];
    this.taskResults = [];
    this.searchResult = null;
    this.success = '';
    this.error = '';
  }
}

