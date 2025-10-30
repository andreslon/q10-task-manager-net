# Script de PowerShell para limpiar el despliegue de Task Manager
# Ejecutar desde el directorio raíz del proyecto

Write-Host "=== Limpieza del Despliegue de Task Manager ===" -ForegroundColor Red

# Confirmar eliminación
$confirmation = Read-Host "¿Estás seguro de que quieres eliminar todos los recursos? (y/N)"
if ($confirmation -ne 'y' -and $confirmation -ne 'Y') {
    Write-Host "Operación cancelada" -ForegroundColor Yellow
    exit 0
}

Write-Host "1. Eliminando recursos de Kubernetes..." -ForegroundColor Yellow

# Eliminar recursos en orden inverso
$manifests = @(
    "14-nodeport-services.yaml",
    "13-ingress.yaml",
    "12-ui-service.yaml",
    "11-ui-deployment.yaml",
    "10-api-service.yaml",
    "09-api-deployment.yaml",
    "08-rabbitmq-service.yaml",
    "07-rabbitmq-deployment.yaml",
    "06-postgres-service.yaml",
    "05-postgres-deployment.yaml",
    "04-postgres-pvc.yaml",
    "03-secret.yaml",
    "02-configmap.yaml",
    "01-namespace.yaml"
)

foreach ($manifest in $manifests) {
    Write-Host "Eliminando $manifest..." -ForegroundColor Cyan
    kubectl delete -f "k8s\$manifest" --ignore-not-found=true
}

Write-Host "2. Verificando eliminación..." -ForegroundColor Yellow
$pods = kubectl get pods -n task-manager 2>$null
if ($pods) {
    Write-Host "Advertencia: Aún hay pods en el namespace task-manager" -ForegroundColor Yellow
    kubectl get pods -n task-manager
} else {
    Write-Host "Namespace task-manager eliminado correctamente" -ForegroundColor Green
}

Write-Host "3. Limpiando imágenes Docker (opcional)..." -ForegroundColor Yellow
$cleanImages = Read-Host "¿Eliminar las imágenes Docker construidas? (y/N)"
if ($cleanImages -eq 'y' -or $cleanImages -eq 'Y') {
    Write-Host "Eliminando imágenes..." -ForegroundColor Cyan
    docker rmi q10taskmanagerapi:latest 2>$null
    docker rmi q10taskmanagerui:latest 2>$null
    Write-Host "Imágenes eliminadas" -ForegroundColor Green
}

Write-Host "=== Limpieza Completada ===" -ForegroundColor Green
Write-Host "Todos los recursos han sido eliminados" -ForegroundColor Cyan
