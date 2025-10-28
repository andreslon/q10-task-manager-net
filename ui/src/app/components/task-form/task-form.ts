import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CreateTaskRequest } from '../../models/task-item.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-form.html',
  styleUrl: './task-form.scss'
})
export class TaskFormComponent {
  @Output() taskCreated = new EventEmitter<CreateTaskRequest>();
  
  task: CreateTaskRequest = {
    title: '',
    description: ''
  };
  
  isSubmitting = false;

  onSubmit(): void {
    if (this.task.title.trim()) {
      this.isSubmitting = true;
      this.taskCreated.emit({ ...this.task });
      this.resetForm();
    }
  }

  resetForm(): void {
    this.task = {
      title: '',
      description: ''
    };
    this.isSubmitting = false;
  }
}
