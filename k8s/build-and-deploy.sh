#!/bin/bash

# Script para construir las imágenes Docker y desplegar en Minikube
echo "Construyendo imágenes Docker y desplegando en Minikube..."

# Configurar Docker para usar el daemon de Minikube
echo "1. Configurando Docker para Minikube..."
eval $(minikube docker-env)

# Construir imagen de la API
echo "2. Construyendo imagen de la API..."
docker build -f api.Dockerfile -t q10taskmanagerapi:latest .

# Construir imagen de la UI
echo "3. Construyendo imagen de la UI..."
docker build -f ui.dockerfile -t q10taskmanagerui:latest .

# Verificar que las imágenes se construyeron
echo "4. Verificando imágenes construidas..."
docker images | grep q10taskmanager

# Aplicar manifiestos de Kubernetes
echo "5. Aplicando manifiestos de Kubernetes..."
./apply-all.sh

echo "¡Construcción y despliegue completados!"
