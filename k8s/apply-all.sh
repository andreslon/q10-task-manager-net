#!/bin/bash

# Script para aplicar todos los manifiestos de Kubernetes en orden
echo "Aplicando manifiestos de Kubernetes para Task Manager..."

# Crear namespace
echo "1. Creando namespace..."
kubectl apply -f 01-namespace.yaml

# Aplicar configuraciones
echo "2. Aplicando ConfigMap y Secret..."
kubectl apply -f 02-configmap.yaml
kubectl apply -f 03-secret.yaml

# Aplicar PostgreSQL
echo "3. Desplegando PostgreSQL..."
kubectl apply -f 04-postgres-pvc.yaml
kubectl apply -f 05-postgres-deployment.yaml
kubectl apply -f 06-postgres-service.yaml

# Aplicar RabbitMQ
echo "4. Desplegando RabbitMQ..."
kubectl apply -f 07-rabbitmq-deployment.yaml
kubectl apply -f 08-rabbitmq-service.yaml

# Esperar a que las bases de datos estén listas
echo "5. Esperando a que las bases de datos estén listas..."
kubectl wait --for=condition=ready pod -l app=postgres -n task-manager --timeout=300s
kubectl wait --for=condition=ready pod -l app=rabbitmq -n task-manager --timeout=300s

# Aplicar API
echo "6. Desplegando API..."
kubectl apply -f 09-api-deployment.yaml
kubectl apply -f 10-api-service.yaml

# Aplicar UI
echo "7. Desplegando UI..."
kubectl apply -f 11-ui-deployment.yaml
kubectl apply -f 12-ui-service.yaml

# Aplicar Ingress
echo "8. Aplicando Ingress..."
kubectl apply -f 13-ingress.yaml

# Aplicar servicios NodePort
echo "9. Aplicando servicios NodePort..."
kubectl apply -f 14-nodeport-services.yaml

echo "¡Despliegue completado!"
echo ""
echo "Para verificar el estado:"
echo "kubectl get pods -n task-manager"
echo "kubectl get services -n task-manager"
echo ""
echo "Para acceder a la aplicación:"
echo "UI: http://$(minikube ip):30090"
echo "API: http://$(minikube ip):30080"
echo "RabbitMQ Management: http://$(minikube ip):30672"
