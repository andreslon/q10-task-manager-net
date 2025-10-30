# Quick Start - Task Manager en Kubernetes

## 🚀 Despliegue Rápido

### Windows
```powershell
# 1. Instalar Minikube (solo la primera vez)
.\k8s\install-minikube-windows.ps1

# 2. Desplegar la aplicación
.\k8s\deploy-windows.ps1

# 3. Acceder a la aplicación
# UI: http://$(minikube ip):30090
# API: http://$(minikube ip):30080
# RabbitMQ: http://$(minikube ip):30672
```

### Linux/Mac
```bash
# 1. Instalar Minikube
curl -LO https://storage.googleapis.com/minikube/releases/latest/minikube-linux-amd64
sudo install minikube-linux-amd64 /usr/local/bin/minikube
minikube start

# 2. Desplegar la aplicación
cd k8s
./build-and-deploy.sh

# 3. Acceder a la aplicación
# UI: http://$(minikube ip):30090
# API: http://$(minikube ip):30080
# RabbitMQ: http://$(minikube ip):30672
```

## 📋 Comandos Útiles

```bash
# Ver estado
kubectl get pods -n task-manager
kubectl get services -n task-manager

# Ver logs
kubectl logs -l app=task-manager-api -n task-manager
kubectl logs -l app=task-manager-ui -n task-manager

# Eliminar todo
kubectl delete namespace task-manager
```

## 🔧 Troubleshooting

### Problema: Pods en estado Pending
```bash
kubectl describe pod <pod-name> -n task-manager
```

### Problema: Imágenes no encontradas
```bash
# Verificar imágenes
docker images | grep q10taskmanager

# Reconstruir si es necesario
docker build -f api.Dockerfile -t q10taskmanagerapi:latest .
docker build -f ui.dockerfile -t q10taskmanagerui:latest .
```

### Problema: Servicios no accesibles
```bash
# Verificar servicios
kubectl get services -n task-manager

# Probar conectividad
kubectl exec -it <pod-name> -n task-manager -- nslookup postgres
```

## 📊 Monitoreo

```bash
# Dashboard de Minikube
minikube dashboard

# Ver recursos
kubectl top pods -n task-manager
kubectl top nodes
```

## 🧹 Limpieza

### Windows
```powershell
.\k8s\cleanup-windows.ps1
```

### Linux/Mac
```bash
cd k8s
./delete-all.sh
```
