import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { TaskItem } from '../../models/task-item.model';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.scss']
})
export class TasksComponent implements OnInit {
  tasks: TaskItem[] = [];
  loading = false;
  error = '';
  success = '';
  
  newTask: TaskItem = { title: '', description: '' };
  editingTask: TaskItem | null = null;

  constructor(private taskService: TaskService) {}

  ngOnInit() {
    this.loadTasks();
  }

  loadTasks() {
    this.loading = true;
    this.error = '';
    this.taskService.getAllTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.loading = false;
      },
      error: (error) => {
        this.error = 'Error loading tasks: ' + error.message;
        this.loading = false;
      }
    });
  }

  createTask() {
    if (!this.newTask.title) {
      this.error = 'Title is required';
      return;
    }

    this.taskService.createTask(this.newTask).subscribe({
      next: () => {
        this.success = 'Task created successfully!';
        this.newTask = { title: '', description: '' };
        this.loadTasks();
        setTimeout(() => this.success = '', 3000);
      },
      error: (error) => {
        this.error = 'Error creating task: ' + error.message;
      }
    });
  }

  editTask(task: TaskItem) {
    this.editingTask = { ...task };
  }

  updateTask() {
    if (!this.editingTask || !this.editingTask.id) return;

    this.taskService.updateTask(this.editingTask.id, this.editingTask).subscribe({
      next: () => {
        this.success = 'Task updated successfully!';
        this.editingTask = null;
        this.loadTasks();
        setTimeout(() => this.success = '', 3000);
      },
      error: (error) => {
        this.error = 'Error updating task: ' + error.message;
      }
    });
  }

  cancelEdit() {
    this.editingTask = null;
  }

  deleteTask(id: string | undefined) {
    if (!id || !confirm('Are you sure you want to delete this task?')) return;

    this.taskService.deleteTask(id).subscribe({
      next: () => {
        this.success = 'Task deleted successfully!';
        this.loadTasks();
        setTimeout(() => this.success = '', 3000);
      },
      error: (error) => {
        this.error = 'Error deleting task: ' + error.message;
      }
    });
  }
}

