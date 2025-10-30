# Task Manager UI - Angular

Aplicación Angular simple para conectarse a la API de Task Manager.

## Endpoints disponibles

La aplicación consume todos los endpoints de la API:

### Endpoints de tareas individuales (TasksController)
- **GET** `/api/tasks` - Obtener todas las tareas
- **GET** `/api/tasks/{id}` - Obtener una tarea por ID
- **POST** `/api/tasks` - Crear una nueva tarea
- **PUT** `/api/tasks/{id}` - Actualizar una tarea
- **DELETE** `/api/tasks/{id}` - Eliminar una tarea

### Endpoints de operaciones masivas (TaskBulkController)
- **POST** `/api/taskbulk` - Crear múltiples tareas
- **GET** `/api/taskbulk/{taskId}` - Obtener el estado de una tarea bulk

### Endpoints de sistema
- **GET** `/api/health` - Verificar el estado del servidor
- **GET** `/api/config` - Obtener la configuración del servidor

## Estructura de la aplicación

```
ui/
├── src/
│   ├── app/
│   │   ├── components/       # (No usado actualmente)
│   │   ├── models/           # Modelos TypeScript
│   │   │   ├── task-item.model.ts
│   │   │   └── task-bulk.model.ts
│   │   ├── services/         # Servicios para conectar con la API
│   │   │   ├── task.service.ts
│   │   │   └── task-bulk.service.ts
│   │   ├── views/            # Vistas de la aplicación
│   │   │   ├── dashboard/    # Vista de dashboard
│   │   │   ├── tasks/        # Vista de gestión de tareas
│   │   │   └── bulk/         # Vista de operaciones masivas
│   │   ├── app.component.*   # Componente principal
│   │   └── app.routes.ts     # Rutas de la aplicación
│   ├── styles.scss           # Estilos globales
│   └── index.html            # Página principal
├── nginx.conf                # Configuración de Nginx
├── package.json              # Dependencias
└── angular.json              # Configuración de Angular
```

## Desarrollo local

```bash
cd ui
npm install
npm start
```

La aplicación estará disponible en `http://localhost:4200`

## Docker

### Construir la imagen

```bash
docker-compose build ui
```

### Levantar todos los servicios

```bash
docker-compose up -d
```

La aplicación estará disponible en:
- **UI:** http://localhost:4200
- **API:** http://localhost:5100
- **Swagger:** http://localhost:5100/swagger

### Ver logs

```bash
docker-compose logs -f ui
docker-compose logs -f api
```

### Detener los servicios

```bash
docker-compose down
```

## Características

### Dashboard
- Muestra el total de tareas
- Muestra las últimas 5 tareas creadas
- Verifica el estado de salud de la API en tiempo real

### Gestión de Tareas
- Crear nuevas tareas
- Listar todas las tareas
- Editar tareas existentes
- Eliminar tareas
- Formularios con validación

### Operaciones Masivas (Bulk)
- Crear múltiples tareas a la vez
- Ver el resultado de cada tarea creada
- Búsqueda de tareas por ID para verificar su estado
- Interfaz dinámica para agregar/remover tareas del bulk

## Tecnologías

- Angular 18
- TypeScript
- SCSS
- HttpClient para llamadas a la API
- Standalone Components
- Routing con lazy loading
- FormsModule para formularios

## API URL

Por defecto, la aplicación se conecta a `http://localhost:5100/api`

Para cambiar la URL de la API, modifica el archivo `src/app/services/task.service.ts` y `src/app/services/task-bulk.service.ts`

