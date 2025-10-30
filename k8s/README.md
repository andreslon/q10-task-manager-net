# Despliegue de Task Manager en Kubernetes con Minikube

Este directorio contiene todos los archivos necesarios para desplegar la aplicación Task Manager en Kubernetes usando Minikube.

## Estructura de Archivos

### Manifiestos de Kubernetes (en orden de aplicación)

1. **01-namespace.yaml** - Namespace para la aplicación
2. **02-configmap.yaml** - Configuraciones de la aplicación
3. **03-secret.yaml** - Credenciales sensibles (passwords)
4. **04-postgres-pvc.yaml** - PersistentVolumeClaim para PostgreSQL
5. **05-postgres-deployment.yaml** - Deployment de PostgreSQL
6. **06-postgres-service.yaml** - Service de PostgreSQL
7. **07-rabbitmq-deployment.yaml** - Deployment de RabbitMQ
8. **08-rabbitmq-service.yaml** - Service de RabbitMQ
9. **09-api-deployment.yaml** - Deployment de la API (.NET Core)
10. **10-api-service.yaml** - Service de la API
11. **11-ui-deployment.yaml** - Deployment de la UI (Angular + Nginx)
12. **12-ui-service.yaml** - Service de la UI
13. **13-ingress.yaml** - Ingress para enrutamiento
14. **14-nodeport-services.yaml** - Servicios NodePort para acceso directo

### Scripts de Automatización

- **apply-all.sh** - Aplica todos los manifiestos en orden
- **delete-all.sh** - Elimina todos los recursos
- **build-and-deploy.sh** - Construye las imágenes Docker y despliega

## Prerrequisitos

1. **Minikube** instalado y configurado
2. **kubectl** instalado
3. **Docker** instalado
4. **Dockerfiles** de la aplicación (api.Dockerfile y ui.dockerfile)

### Instalación en Windows

#### Opción 1: Script Automatizado
```powershell
# Ejecutar como Administrador
cd k8s
.\install-minikube-windows.ps1
```

#### Opción 2: Instalación Manual
1. Instalar Chocolatey: https://chocolatey.org/install
2. Instalar Minikube: `choco install minikube`
3. Instalar Docker Desktop: `choco install docker-desktop`
4. Reiniciar el sistema

## Instalación y Configuración

### 1. Iniciar Minikube

```bash
# Iniciar Minikube
minikube start

# Verificar que Minikube está corriendo
minikube status

# Habilitar el addon de ingress (opcional)
minikube addons enable ingress
```

### 2. Configurar Docker para Minikube

```bash
# Configurar Docker para usar el daemon de Minikube
eval $(minikube docker-env)

# Verificar que está configurado correctamente
docker ps
```

### 3. Construir las Imágenes Docker

```bash
# Construir imagen de la API
docker build -f api.Dockerfile -t q10taskmanagerapi:latest .

# Construir imagen de la UI
docker build -f ui.dockerfile -t q10taskmanagerui:latest .

# Verificar que las imágenes se construyeron
docker images | grep q10taskmanager
```

### 4. Desplegar en Kubernetes

#### Opción A: Script automatizado para Windows
```powershell
# Desde el directorio raíz del proyecto
.\k8s\deploy-windows.ps1
```

#### Opción B: Script automatizado para Linux/Mac
```bash
cd k8s
./build-and-deploy.sh
```

#### Opción C: Aplicar manualmente
```bash
cd k8s

# Aplicar todos los manifiestos en orden
kubectl apply -f 01-namespace.yaml
kubectl apply -f 02-configmap.yaml
kubectl apply -f 03-secret.yaml
kubectl apply -f 04-postgres-pvc.yaml
kubectl apply -f 05-postgres-deployment.yaml
kubectl apply -f 06-postgres-service.yaml
kubectl apply -f 07-rabbitmq-deployment.yaml
kubectl apply -f 08-rabbitmq-service.yaml

# Esperar a que las bases de datos estén listas
kubectl wait --for=condition=ready pod -l app=postgres -n task-manager --timeout=300s
kubectl wait --for=condition=ready pod -l app=rabbitmq -n task-manager --timeout=300s

# Desplegar la aplicación
kubectl apply -f 09-api-deployment.yaml
kubectl apply -f 10-api-service.yaml
kubectl apply -f 11-ui-deployment.yaml
kubectl apply -f 12-ui-service.yaml
kubectl apply -f 13-ingress.yaml
kubectl apply -f 14-nodeport-services.yaml
```

## Verificación del Despliegue

### 1. Verificar Pods
```bash
kubectl get pods -n task-manager
```

### 2. Verificar Services
```bash
kubectl get services -n task-manager
```

### 3. Verificar Logs
```bash
# Logs de la API
kubectl logs -l app=task-manager-api -n task-manager

# Logs de la UI
kubectl logs -l app=task-manager-ui -n task-manager

# Logs de PostgreSQL
kubectl logs -l app=postgres -n task-manager

# Logs de RabbitMQ
kubectl logs -l app=rabbitmq -n task-manager
```

## Acceso a la Aplicación

### Obtener la IP de Minikube
```bash
minikube ip
```

### URLs de Acceso

- **UI (Frontend)**: `http://$(minikube ip):30090`
- **API (Backend)**: `http://$(minikube ip):30080`
- **RabbitMQ Management**: `http://$(minikube ip):30672`
  - Usuario: `admin`
  - Contraseña: `admin123`

### Usando Ingress (si está habilitado)
```bash
# Agregar entrada al archivo hosts
echo "$(minikube ip) task-manager.local" | sudo tee -a /etc/hosts

# Acceder a la aplicación
# UI: http://task-manager.local
# API: http://task-manager.local/api
```

## Comandos Útiles

### Monitoreo
```bash
# Ver todos los recursos
kubectl get all -n task-manager

# Describir un pod específico
kubectl describe pod <pod-name> -n task-manager

# Ver eventos del namespace
kubectl get events -n task-manager --sort-by='.lastTimestamp'
```

### Escalado
```bash
# Escalar la API a 3 réplicas
kubectl scale deployment task-manager-api --replicas=3 -n task-manager

# Escalar la UI a 3 réplicas
kubectl scale deployment task-manager-ui --replicas=3 -n task-manager
```

### Port Forwarding (para debugging)
```bash
# API
kubectl port-forward service/task-manager-api 8080:80 -n task-manager

# UI
kubectl port-forward service/task-manager-ui 8081:80 -n task-manager

# PostgreSQL
kubectl port-forward service/postgres 5432:5432 -n task-manager

# RabbitMQ Management
kubectl port-forward service/rabbitmq 15672:15672 -n task-manager
```

## Limpieza

### Windows
```powershell
# Script automatizado
.\k8s\cleanup-windows.ps1

# O eliminar manualmente
kubectl delete namespace task-manager
```

### Linux/Mac
```bash
cd k8s
./delete-all.sh

# O eliminar manualmente
kubectl delete namespace task-manager
```

## Configuración de la Aplicación

### Variables de Entorno

La aplicación utiliza las siguientes variables de entorno:

- `ASPNETCORE_ENVIRONMENT`: Entorno de ejecución (.NET)
- `ASPNETCORE_URLS`: URLs de escucha de la API
- `CONNECTION_STRING`: Cadena de conexión a PostgreSQL
- `API_KEY`: Clave de API para autenticación
- `MAX_ITEMS_PER_PAGE`: Número máximo de elementos por página

### Configuración de Base de Datos

- **PostgreSQL**: Puerto 5432, Base de datos: `TaskManagerDB`
- **Usuario**: `postgres`
- **Contraseña**: `postgres_password123`

### Configuración de RabbitMQ

- **Puerto AMQP**: 5672
- **Puerto Management**: 15672
- **Usuario**: `admin`
- **Contraseña**: `admin123`

## Troubleshooting

### Problemas Comunes

1. **Pods en estado Pending**
   ```bash
   kubectl describe pod <pod-name> -n task-manager
   ```

2. **Imágenes no encontradas**
   ```bash
   # Verificar que las imágenes existen
   docker images | grep q10taskmanager
   
   # Reconstruir si es necesario
   docker build -f api.Dockerfile -t q10taskmanagerapi:latest .
   docker build -f ui.dockerfile -t q10taskmanagerui:latest .
   ```

3. **Problemas de conectividad entre servicios**
   ```bash
   # Verificar que los servicios están expuestos
   kubectl get services -n task-manager
   
   # Probar conectividad desde un pod
   kubectl exec -it <pod-name> -n task-manager -- nslookup postgres
   ```

4. **Problemas de almacenamiento**
   ```bash
   # Verificar PVCs
   kubectl get pvc -n task-manager
   
   # Verificar PVs
   kubectl get pv
   ```

### Logs Detallados

```bash
# Logs con timestamps
kubectl logs -l app=task-manager-api -n task-manager --timestamps

# Logs de los últimos 100 eventos
kubectl logs -l app=task-manager-api -n task-manager --tail=100

# Seguir logs en tiempo real
kubectl logs -l app=task-manager-api -n task-manager -f
```

## Arquitectura del Despliegue

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Minikube      │    │   Namespace     │    │   Components    │
│                 │    │   task-manager  │    │                 │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│                 │    │                 │    │ • PostgreSQL    │
│  ┌─────────────┐│    │  ┌─────────────┐│    │ • RabbitMQ      │
│  │   Ingress   ││    │  │   Ingress   ││    │ • API (.NET)    │
│  └─────────────┘│    │  └─────────────┘│    │ • UI (Angular)  │
│                 │    │                 │    │                 │
│  ┌─────────────┐│    │  ┌─────────────┐│    │                 │
│  │   Services  ││    │  │   Services  ││    │                 │
│  │   (NodePort)││    │  │             ││    │                 │
│  └─────────────┘│    │  └─────────────┘│    │                 │
│                 │    │                 │    │                 │
│  ┌─────────────┐│    │  ┌─────────────┐│    │                 │
│  │  Deployments││    │  │  Deployments││    │                 │
│  │             ││    │  │             ││    │                 │
│  └─────────────┘│    │  └─────────────┘│    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Notas Importantes

1. **Imágenes Docker**: Las imágenes se construyen localmente y se usan con `imagePullPolicy: Never`
2. **Almacenamiento**: Se usa un PVC de 5Gi para PostgreSQL
3. **Red**: Todos los servicios están en el mismo namespace y pueden comunicarse por nombre
4. **Escalabilidad**: La API y UI están configuradas con 2 réplicas cada una
5. **Monitoreo**: Se incluyen health checks para todos los componentes

## Próximos Pasos

1. Configurar un registry de imágenes (Docker Hub, ECR, etc.)
2. Implementar CI/CD con GitHub Actions
3. Agregar monitoring con Prometheus y Grafana
4. Configurar logging centralizado con ELK Stack
5. Implementar backup automático de la base de datos
6. Configurar autoscaling horizontal (HPA)
