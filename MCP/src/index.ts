#!/usr/bin/env node

import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema,
  Tool,
} from "@modelcontextprotocol/sdk/types.js";

// Configuracion de la API
const API_BASE_URL = process.env.TASKMANAGER_API_URL || 
  "http://a272ddae62e1b4505851f98b3a62d36f-1573655459.us-east-1.elb.amazonaws.com";

let authToken: string | null = null;

// Tipos basados en el swagger
interface LoginRequest {
  username: string;
  password: string;
}

interface UserRequest {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

interface TaskItem {
  id?: string;
  title: string;
  description?: string;
  created?: string;
  updated?: string;
}

interface TaskBulkRequest {
  id?: string;
  title: string;
  description?: string;
}

interface TaskBulkResponse {
  taskId?: string;
  title?: string;
  isSuccess: boolean;
  errorMessage?: string;
}

// Funcion helper para hacer requests
async function apiRequest<T>(
  endpoint: string,
  method: string = "GET",
  body?: unknown
): Promise<T> {
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
  };

  if (authToken) {
    headers["Authorization"] = `Bearer ${authToken}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`API Error ${response.status}: ${errorText}`);
  }

  const contentType = response.headers.get("content-type");
  if (contentType && contentType.includes("application/json")) {
    return response.json() as Promise<T>;
  }
  
  return response.text() as unknown as T;
}

// Definicion de herramientas
const tools: Tool[] = [
  // Auth Tools
  {
    name: "auth_register",
    description: "Registra un nuevo usuario en el sistema TaskManager",
    inputSchema: {
      type: "object",
      properties: {
        username: {
          type: "string",
          description: "Nombre de usuario (maximo 100 caracteres)",
          maxLength: 100,
        },
        email: {
          type: "string",
          description: "Correo electronico del usuario",
          format: "email",
        },
        password: {
          type: "string",
          description: "Contrasena (minimo 6 caracteres)",
          minLength: 6,
        },
        firstName: {
          type: "string",
          description: "Nombre (opcional, maximo 100 caracteres)",
          maxLength: 100,
        },
        lastName: {
          type: "string",
          description: "Apellido (opcional, maximo 100 caracteres)",
          maxLength: 100,
        },
      },
      required: ["username", "email", "password"],
    },
  },
  {
    name: "auth_login",
    description: "Inicia sesion y obtiene un token JWT para autenticacion",
    inputSchema: {
      type: "object",
      properties: {
        username: {
          type: "string",
          description: "Nombre de usuario",
        },
        password: {
          type: "string",
          description: "Contrasena del usuario",
        },
      },
      required: ["username", "password"],
    },
  },

  // Config Tools
  {
    name: "get_config",
    description: "Obtiene la configuracion actual del sistema TaskManager",
    inputSchema: {
      type: "object",
      properties: {},
      required: [],
    },
  },

  // Health Tools
  {
    name: "health_check",
    description: "Verifica el estado de salud de la aplicacion TaskManager",
    inputSchema: {
      type: "object",
      properties: {},
      required: [],
    },
  },

  // Task Tools
  {
    name: "create_task",
    description: "Crea una nueva tarea en el sistema",
    inputSchema: {
      type: "object",
      properties: {
        title: {
          type: "string",
          description: "Titulo de la tarea (requerido, 1-400 caracteres)",
          minLength: 1,
          maxLength: 400,
        },
        description: {
          type: "string",
          description: "Descripcion de la tarea (opcional, maximo 500 caracteres)",
          maxLength: 500,
        },
      },
      required: ["title"],
    },
  },
  {
    name: "get_all_tasks",
    description: "Obtiene todas las tareas del sistema",
    inputSchema: {
      type: "object",
      properties: {},
      required: [],
    },
  },
  {
    name: "get_task",
    description: "Obtiene una tarea especifica por su ID",
    inputSchema: {
      type: "object",
      properties: {
        id: {
          type: "string",
          description: "ID unico de la tarea",
        },
      },
      required: ["id"],
    },
  },
  {
    name: "update_task",
    description: "Actualiza una tarea existente",
    inputSchema: {
      type: "object",
      properties: {
        id: {
          type: "string",
          description: "ID de la tarea a actualizar",
        },
        title: {
          type: "string",
          description: "Nuevo titulo de la tarea (1-400 caracteres)",
          minLength: 1,
          maxLength: 400,
        },
        description: {
          type: "string",
          description: "Nueva descripcion de la tarea (maximo 500 caracteres)",
          maxLength: 500,
        },
      },
      required: ["id", "title"],
    },
  },
  {
    name: "delete_task",
    description: "Elimina una tarea por su ID",
    inputSchema: {
      type: "object",
      properties: {
        id: {
          type: "string",
          description: "ID de la tarea a eliminar",
        },
      },
      required: ["id"],
    },
  },

  // Task Bulk Tools
  {
    name: "create_tasks_bulk",
    description: "Crea multiples tareas en una sola operacion (procesamiento en lote)",
    inputSchema: {
      type: "object",
      properties: {
        tasks: {
          type: "array",
          description: "Lista de tareas a crear",
          items: {
            type: "object",
            properties: {
              id: {
                type: "string",
                description: "ID opcional de la tarea",
              },
              title: {
                type: "string",
                description: "Titulo de la tarea (requerido)",
              },
              description: {
                type: "string",
                description: "Descripcion de la tarea (opcional)",
              },
            },
            required: ["title"],
          },
        },
      },
      required: ["tasks"],
    },
  },
  {
    name: "get_bulk_task_status",
    description: "Obtiene el estado de una tarea de procesamiento en lote",
    inputSchema: {
      type: "object",
      properties: {
        taskId: {
          type: "string",
          description: "ID de la tarea bulk a consultar",
        },
      },
      required: ["taskId"],
    },
  },

  // Utility Tools
  {
    name: "set_auth_token",
    description: "Establece manualmente un token de autenticacion JWT",
    inputSchema: {
      type: "object",
      properties: {
        token: {
          type: "string",
          description: "Token JWT para autenticacion",
        },
      },
      required: ["token"],
    },
  },
  {
    name: "clear_auth_token",
    description: "Elimina el token de autenticacion actual",
    inputSchema: {
      type: "object",
      properties: {},
      required: [],
    },
  },
];

// Handler para las herramientas
async function handleToolCall(
  name: string,
  args: Record<string, unknown>
): Promise<string> {
  switch (name) {
    // Auth
    case "auth_register": {
      const request: UserRequest = {
        username: args.username as string,
        email: args.email as string,
        password: args.password as string,
        firstName: args.firstName as string | undefined,
        lastName: args.lastName as string | undefined,
      };
      const result = await apiRequest("/api/Auth/register", "POST", request);
      return JSON.stringify(result, null, 2);
    }

    case "auth_login": {
      const request: LoginRequest = {
        username: args.username as string,
        password: args.password as string,
      };
      const result = await apiRequest<{ token?: string }>("/api/Auth/login", "POST", request);
      
      // Guardar el token si viene en la respuesta
      if (result && typeof result === "object" && "token" in result) {
        authToken = result.token as string;
      }
      
      return JSON.stringify(result, null, 2);
    }

    // Config
    case "get_config": {
      const result = await apiRequest("/api/Config", "GET");
      return JSON.stringify(result, null, 2);
    }

    // Health
    case "health_check": {
      const result = await apiRequest("/api/Health", "GET");
      return JSON.stringify(result, null, 2);
    }

    // Tasks
    case "create_task": {
      const task: TaskItem = {
        title: args.title as string,
        description: args.description as string | undefined,
      };
      const result = await apiRequest("/api/Tasks", "POST", task);
      return JSON.stringify(result, null, 2);
    }

    case "get_all_tasks": {
      const result = await apiRequest("/api/Tasks", "GET");
      return JSON.stringify(result, null, 2);
    }

    case "get_task": {
      const id = args.id as string;
      const result = await apiRequest(`/api/Tasks/${encodeURIComponent(id)}`, "GET");
      return JSON.stringify(result, null, 2);
    }

    case "update_task": {
      const id = args.id as string;
      const task: TaskItem = {
        id: id,
        title: args.title as string,
        description: args.description as string | undefined,
      };
      const result = await apiRequest(`/api/Tasks/${encodeURIComponent(id)}`, "PUT", task);
      return JSON.stringify(result, null, 2);
    }

    case "delete_task": {
      const id = args.id as string;
      const result = await apiRequest(`/api/Tasks/${encodeURIComponent(id)}`, "DELETE");
      return JSON.stringify(result, null, 2);
    }

    // Task Bulk
    case "create_tasks_bulk": {
      const tasks = args.tasks as TaskBulkRequest[];
      const result = await apiRequest<string[]>("/api/TaskBulk", "POST", tasks);
      return JSON.stringify(result, null, 2);
    }

    case "get_bulk_task_status": {
      const taskId = args.taskId as string;
      const result = await apiRequest<TaskBulkResponse>(
        `/api/TaskBulk/${encodeURIComponent(taskId)}`,
        "GET"
      );
      return JSON.stringify(result, null, 2);
    }

    // Utility
    case "set_auth_token": {
      authToken = args.token as string;
      return JSON.stringify({ success: true, message: "Token establecido correctamente" });
    }

    case "clear_auth_token": {
      authToken = null;
      return JSON.stringify({ success: true, message: "Token eliminado correctamente" });
    }

    default:
      throw new Error(`Herramienta desconocida: ${name}`);
  }
}

// Crear y configurar el servidor MCP
const server = new Server(
  {
    name: "taskmanager-mcp",
    version: "1.0.0",
  },
  {
    capabilities: {
      tools: {},
    },
  }
);

// Handler para listar herramientas
server.setRequestHandler(ListToolsRequestSchema, async () => {
  return { tools };
});

// Handler para ejecutar herramientas
server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;

  try {
    const result = await handleToolCall(name, args as Record<string, unknown>);
    return {
      content: [
        {
          type: "text",
          text: result,
        },
      ],
    };
  } catch (error) {
    const errorMessage = error instanceof Error ? error.message : String(error);
    return {
      content: [
        {
          type: "text",
          text: `Error: ${errorMessage}`,
        },
      ],
      isError: true,
    };
  }
});

// Iniciar el servidor
async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("TaskManager MCP Server iniciado");
}

main().catch((error) => {
  console.error("Error fatal:", error);
  process.exit(1);
});

