import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaskBulkRequest, TaskBulkResponse } from '../models/task-bulk.model';

@Injectable({
  providedIn: 'root'
})
export class TaskBulkService {
  private apiUrl = 'http://localhost:5100/api/taskbulk';

  constructor(private http: HttpClient) {}

  createBulkTasks(tasks: TaskBulkRequest[]): Observable<string[]> {
    return this.http.post<string[]>(this.apiUrl, tasks);
  }

  getTaskById(taskId: string): Observable<TaskBulkResponse> {
    return this.http.get<TaskBulkResponse>(`${this.apiUrl}/${taskId}`);
  }
}

