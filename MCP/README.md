# TaskManager MCP Server

MCP (Model Context Protocol) Server para la API de TaskManager Q10.

## Instalacion

```bash
cd MCP
npm install
npm run build
```

## Configuracion en Cursor

Agrega la siguiente configuracion a tu archivo de configuracion de Cursor MCP:

### Windows

Ubicacion del archivo: `%APPDATA%\Cursor\User\globalStorage\cursor.mcp\config.json`

```json
{
  "mcpServers": {
    "taskmanager": {
      "command": "node",
      "args": ["C:\\Dev\\Cedesistemas\\q10-task-manager-net\\MCP\\dist\\index.js"],
      "env": {
        "TASKMANAGER_API_URL": "http://a272ddae62e1b4505851f98b3a62d36f-1573655459.us-east-1.elb.amazonaws.com"
      }
    }
  }
}
```

### Alternativa con npx (desarrollo)

```json
{
  "mcpServers": {
    "taskmanager": {
      "command": "npx",
      "args": ["tsx", "C:\\Dev\\Cedesistemas\\q10-task-manager-net\\MCP\\src\\index.ts"],
      "env": {
        "TASKMANAGER_API_URL": "http://a272ddae62e1b4505851f98b3a62d36f-1573655459.us-east-1.elb.amazonaws.com"
      }
    }
  }
}
```

## Herramientas Disponibles

### Autenticacion

| Herramienta | Descripcion |
|-------------|-------------|
| `auth_register` | Registra un nuevo usuario |
| `auth_login` | Inicia sesion y obtiene token JWT |
| `set_auth_token` | Establece manualmente un token |
| `clear_auth_token` | Elimina el token actual |

### Tareas

| Herramienta | Descripcion |
|-------------|-------------|
| `create_task` | Crea una nueva tarea |
| `get_all_tasks` | Obtiene todas las tareas |
| `get_task` | Obtiene una tarea por ID |
| `update_task` | Actualiza una tarea existente |
| `delete_task` | Elimina una tarea |

### Tareas en Lote (Bulk)

| Herramienta | Descripcion |
|-------------|-------------|
| `create_tasks_bulk` | Crea multiples tareas en lote |
| `get_bulk_task_status` | Consulta el estado de un proceso bulk |

### Sistema

| Herramienta | Descripcion |
|-------------|-------------|
| `health_check` | Verifica el estado de la API |
| `get_config` | Obtiene la configuracion del sistema |

## Variables de Entorno

| Variable | Descripcion | Valor por defecto |
|----------|-------------|-------------------|
| `TASKMANAGER_API_URL` | URL base de la API | `http://a272ddae62e1b4505851f98b3a62d36f-1573655459.us-east-1.elb.amazonaws.com` |

## Ejemplos de Uso

Una vez configurado en Cursor, puedes usar las herramientas directamente en el chat:

- "Verifica el estado de la API de TaskManager"
- "Crea una tarea con titulo 'Revisar codigo' y descripcion 'Revisar PR pendientes'"
- "Muestrame todas las tareas"
- "Actualiza la tarea con ID xyz"
- "Elimina la tarea con ID abc"

## Desarrollo

```bash
# Ejecutar en modo desarrollo
npm run dev

# Compilar
npm run build

# Ejecutar version compilada
npm start
```

