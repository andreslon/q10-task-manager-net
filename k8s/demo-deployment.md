# Demo del Despliegue - Task Manager en Kubernetes

## 📋 Resumen de lo Creado

He creado una estructura completa de Kubernetes para desplegar tu aplicación Task Manager en Minikube. Aquí está todo lo que se ha configurado:

### 🗂️ Estructura de Archivos

```
k8s/
├── 01-namespace.yaml              # Namespace para la aplicación
├── 02-configmap.yaml             # Configuraciones de la aplicación
├── 03-secret.yaml                # Credenciales sensibles
├── 04-postgres-pvc.yaml          # Almacenamiento para PostgreSQL
├── 05-postgres-deployment.yaml   # Deployment de PostgreSQL
├── 06-postgres-service.yaml      # Service de PostgreSQL
├── 07-rabbitmq-deployment.yaml   # Deployment de RabbitMQ
├── 08-rabbitmq-service.yaml      # Service de RabbitMQ
├── 09-api-deployment.yaml        # Deployment de la API (.NET)
├── 10-api-service.yaml           # Service de la API
├── 11-ui-deployment.yaml         # Deployment de la UI (Angular)
├── 12-ui-service.yaml            # Service de la UI
├── 13-ingress.yaml               # Ingress para enrutamiento
├── 14-nodeport-services.yaml     # Servicios NodePort para acceso directo
├── 15-ingress-nginx.yaml         # Configuración adicional de Nginx
├── 16-hpa.yaml                   # Horizontal Pod Autoscaler
├── apply-all.sh                  # Script de aplicación (Linux/Mac)
├── build-and-deploy.sh           # Script completo (Linux/Mac)
├── delete-all.sh                 # Script de limpieza (Linux/Mac)
├── deploy-windows.ps1            # Script de despliegue (Windows)
├── cleanup-windows.ps1           # Script de limpieza (Windows)
├── install-minikube-windows.ps1  # Instalador de Minikube (Windows)
├── install-minikube-windows.bat  # Instalador de Minikube (Windows - Batch)
├── README.md                     # Documentación completa
├── QUICK-START.md               # Guía de inicio rápido
└── demo-deployment.md           # Este archivo
```

### 🏗️ Arquitectura Desplegada

```
┌─────────────────────────────────────────────────────────────┐
│                    Minikube Cluster                        │
├─────────────────────────────────────────────────────────────┤
│  Namespace: task-manager                                    │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ PostgreSQL  │  │  RabbitMQ   │  │     API     │        │
│  │             │  │             │  │  (.NET)     │        │
│  │ Port: 5432  │  │ Port: 5672  │  │ Port: 80    │        │
│  │             │  │ Management: │  │             │        │
│  │             │  │ Port: 15672 │  │             │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │     UI      │  │   Ingress   │  │  NodePort   │        │
│  │ (Angular)   │  │             │  │  Services   │        │
│  │ Port: 80    │  │ Port: 80    │  │             │        │
│  │             │  │             │  │ UI: 30090   │        │
│  │             │  │             │  │ API: 30080  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### 🔧 Componentes Configurados

#### 1. **PostgreSQL**
- **Imagen**: `postgres:latest`
- **Puerto**: 5432
- **Base de datos**: `TaskManagerDB`
- **Usuario**: `postgres`
- **Contraseña**: `postgres_password123`
- **Almacenamiento**: PVC de 5Gi

#### 2. **RabbitMQ**
- **Imagen**: `rabbitmq:3-management`
- **Puerto AMQP**: 5672
- **Puerto Management**: 15672
- **Usuario**: `admin`
- **Contraseña**: `admin123`

#### 3. **API (.NET Core)**
- **Imagen**: `q10taskmanagerapi:latest`
- **Puerto**: 80
- **Réplicas**: 2
- **Health checks**: `/health`
- **Variables de entorno**: Configuradas desde ConfigMap y Secret

#### 4. **UI (Angular + Nginx)**
- **Imagen**: `q10taskmanagerui:latest`
- **Puerto**: 80
- **Réplicas**: 2
- **Health checks**: `/`

#### 5. **Servicios de Acceso**
- **ClusterIP**: Para comunicación interna
- **NodePort**: Para acceso externo
  - UI: Puerto 30090
  - API: Puerto 30080
  - RabbitMQ Management: Puerto 30672
- **Ingress**: Para enrutamiento basado en host/path

### 🚀 Cómo Ejecutar el Despliegue

#### Opción 1: Windows (Recomendado)
```powershell
# 1. Instalar Minikube
.\k8s\install-minikube-windows.ps1

# 2. Desplegar la aplicación
.\k8s\deploy-windows.ps1

# 3. Acceder a la aplicación
# Obtener IP de Minikube
minikube ip

# URLs de acceso:
# UI: http://<minikube-ip>:30090
# API: http://<minikube-ip>:30080
# RabbitMQ: http://<minikube-ip>:30672
```

#### Opción 2: Linux/Mac
```bash
# 1. Instalar Minikube
curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
sudo install minikube-linux-amd64 /usr/local/bin/minikube
minikube start

# 2. Desplegar la aplicación
cd k8s
./build-and-deploy.sh

# 3. Acceder a la aplicación
minikube ip
# URLs: http://<minikube-ip>:30090, http://<minikube-ip>:30080, etc.
```

### 🔍 Verificación del Despliegue

```bash
# Ver todos los pods
kubectl get pods -n task-manager

# Ver servicios
kubectl get services -n task-manager

# Ver logs de la API
kubectl logs -l app=task-manager-api -n task-manager

# Ver logs de la UI
kubectl logs -l app=task-manager-ui -n task-manager

# Ver estado de los deployments
kubectl get deployments -n task-manager
```

### 🧹 Limpieza

#### Windows
```powershell
.\k8s\cleanup-windows.ps1
```

#### Linux/Mac
```bash
cd k8s
./delete-all.sh
```

### 📊 Características Avanzadas

#### 1. **Health Checks**
- Todos los componentes tienen health checks configurados
- Liveness probes para reiniciar pods no saludables
- Readiness probes para evitar tráfico a pods no listos

#### 2. **Escalabilidad**
- API y UI configuradas con 2 réplicas cada una
- HPA (Horizontal Pod Autoscaler) configurado para escalado automático
- Límites de CPU y memoria configurados

#### 3. **Almacenamiento**
- PVC para PostgreSQL con 5Gi de almacenamiento
- Configuración de storage class estándar

#### 4. **Redes**
- Todos los servicios en el mismo namespace
- Comunicación interna por nombre de servicio
- Acceso externo a través de NodePort e Ingress

#### 5. **Configuración**
- ConfigMap para configuraciones no sensibles
- Secret para credenciales sensibles
- Variables de entorno inyectadas automáticamente

### 🎯 Próximos Pasos

1. **Instalar Minikube** siguiendo las instrucciones del README
2. **Ejecutar el script de despliegue** correspondiente a tu sistema operativo
3. **Verificar que todo funciona** accediendo a las URLs proporcionadas
4. **Explorar la aplicación** y verificar la funcionalidad
5. **Monitorear los logs** para detectar cualquier problema

### 📚 Documentación Adicional

- **README.md**: Documentación completa con troubleshooting
- **QUICK-START.md**: Guía de inicio rápido
- **Scripts**: Automatización completa del proceso

¡El despliegue está listo para ser ejecutado! Solo necesitas instalar Minikube y ejecutar el script correspondiente a tu sistema operativo.
