export interface TaskItem {
  id: string;
  title: string;
  description?: string;
  created: Date;
  updated: Date;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
}
