#!/bin/bash

# Script para eliminar todos los recursos de Kubernetes
echo "Eliminando recursos de Kubernetes para Task Manager..."

# Eliminar recursos en orden inverso
kubectl delete -f 14-nodeport-services.yaml
kubectl delete -f 13-ingress.yaml
kubectl delete -f 12-ui-service.yaml
kubectl delete -f 11-ui-deployment.yaml
kubectl delete -f 10-api-service.yaml
kubectl delete -f 09-api-deployment.yaml
kubectl delete -f 08-rabbitmq-service.yaml
kubectl delete -f 07-rabbitmq-deployment.yaml
kubectl delete -f 06-postgres-service.yaml
kubectl delete -f 05-postgres-deployment.yaml
kubectl delete -f 04-postgres-pvc.yaml
kubectl delete -f 03-secret.yaml
kubectl delete -f 02-configmap.yaml
kubectl delete -f 01-namespace.yaml

echo "¡Recursos eliminados!"
