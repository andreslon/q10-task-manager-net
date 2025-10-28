export interface TaskBulkRequest {
  id: string;
  title: string;
  description?: string;
}

export interface TaskBulkResponse {
  taskId: string;
  title: string;
  isSuccess: boolean;
  errorMessage?: string;
}
