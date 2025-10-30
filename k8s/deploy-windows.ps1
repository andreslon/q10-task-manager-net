# Script de PowerShell para desplegar Task Manager en Minikube
# Ejecutar desde el directorio raíz del proyecto

param(
    [switch]$SkipBuild,
    [switch]$SkipMinikubeCheck
)

Write-Host "=== Despliegue de Task Manager en Minikube ===" -ForegroundColor Green

# Verificar si estamos en el directorio correcto
if (!(Test-Path "api.Dockerfile") -or !(Test-Path "ui.dockerfile")) {
    Write-Host "Error: Ejecutar este script desde el directorio raíz del proyecto" -ForegroundColor Red
    exit 1
}

# Verificar Minikube
if (!$SkipMinikubeCheck) {
    Write-Host "1. Verificando Minikube..." -ForegroundColor Yellow
    try {
        $minikubeStatus = minikube status 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Minikube no está ejecutándose. Iniciando..." -ForegroundColor Yellow
            minikube start
            if ($LASTEXITCODE -ne 0) {
                Write-Host "Error: No se pudo iniciar Minikube" -ForegroundColor Red
                exit 1
            }
        } else {
            Write-Host "Minikube está ejecutándose correctamente" -ForegroundColor Green
        }
    } catch {
        Write-Host "Error: Minikube no está instalado o no está en el PATH" -ForegroundColor Red
        Write-Host "Ejecuta primero: .\k8s\install-minikube-windows.ps1" -ForegroundColor Yellow
        exit 1
    }
}

# Configurar Docker para Minikube
Write-Host "2. Configurando Docker para Minikube..." -ForegroundColor Yellow
try {
    minikube docker-env | Invoke-Expression
    Write-Host "Docker configurado para Minikube" -ForegroundColor Green
} catch {
    Write-Host "Error: No se pudo configurar Docker para Minikube" -ForegroundColor Red
    exit 1
}

# Construir imágenes Docker
if (!$SkipBuild) {
    Write-Host "3. Construyendo imágenes Docker..." -ForegroundColor Yellow
    
    # Construir API
    Write-Host "Construyendo imagen de la API..." -ForegroundColor Cyan
    docker build -f api.Dockerfile -t q10taskmanagerapi:latest .
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Falló la construcción de la imagen de la API" -ForegroundColor Red
        exit 1
    }
    
    # Construir UI
    Write-Host "Construyendo imagen de la UI..." -ForegroundColor Cyan
    docker build -f ui.dockerfile -t q10taskmanagerui:latest .
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Falló la construcción de la imagen de la UI" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Imágenes construidas correctamente" -ForegroundColor Green
} else {
    Write-Host "3. Omitiendo construcción de imágenes..." -ForegroundColor Yellow
}

# Verificar imágenes
Write-Host "4. Verificando imágenes construidas..." -ForegroundColor Yellow
$images = docker images | Select-String "q10taskmanager"
if ($images) {
    Write-Host "Imágenes encontradas:" -ForegroundColor Green
    $images | ForEach-Object { Write-Host "  $_" -ForegroundColor Cyan }
} else {
    Write-Host "Advertencia: No se encontraron las imágenes q10taskmanager" -ForegroundColor Yellow
}

# Aplicar manifiestos de Kubernetes
Write-Host "5. Aplicando manifiestos de Kubernetes..." -ForegroundColor Yellow

$manifests = @(
    "01-namespace.yaml",
    "02-configmap.yaml", 
    "03-secret.yaml",
    "04-postgres-pvc.yaml",
    "05-postgres-deployment.yaml",
    "06-postgres-service.yaml",
    "07-rabbitmq-deployment.yaml",
    "08-rabbitmq-service.yaml"
)

foreach ($manifest in $manifests) {
    Write-Host "Aplicando $manifest..." -ForegroundColor Cyan
    kubectl apply -f "k8s\$manifest"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Falló la aplicación de $manifest" -ForegroundColor Red
        exit 1
    }
}

# Esperar a que las bases de datos estén listas
Write-Host "6. Esperando a que las bases de datos estén listas..." -ForegroundColor Yellow
Write-Host "Esperando PostgreSQL..." -ForegroundColor Cyan
kubectl wait --for=condition=ready pod -l app=postgres -n task-manager --timeout=300s
if ($LASTEXITCODE -ne 0) {
    Write-Host "Advertencia: PostgreSQL no está listo en el tiempo esperado" -ForegroundColor Yellow
}

Write-Host "Esperando RabbitMQ..." -ForegroundColor Cyan
kubectl wait --for=condition=ready pod -l app=rabbitmq -n task-manager --timeout=300s
if ($LASTEXITCODE -ne 0) {
    Write-Host "Advertencia: RabbitMQ no está listo en el tiempo esperado" -ForegroundColor Yellow
}

# Aplicar la aplicación
Write-Host "7. Desplegando la aplicación..." -ForegroundColor Yellow
$appManifests = @(
    "09-api-deployment.yaml",
    "10-api-service.yaml",
    "11-ui-deployment.yaml",
    "12-ui-service.yaml",
    "13-ingress.yaml",
    "14-nodeport-services.yaml"
)

foreach ($manifest in $appManifests) {
    Write-Host "Aplicando $manifest..." -ForegroundColor Cyan
    kubectl apply -f "k8s\$manifest"
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: Falló la aplicación de $manifest" -ForegroundColor Red
        exit 1
    }
}

# Verificar el despliegue
Write-Host "8. Verificando el despliegue..." -ForegroundColor Yellow
Write-Host "Pods:" -ForegroundColor Cyan
kubectl get pods -n task-manager

Write-Host "`nServicios:" -ForegroundColor Cyan
kubectl get services -n task-manager

# Obtener IP de Minikube
$minikubeIP = minikube ip
Write-Host "`n=== Despliegue Completado ===" -ForegroundColor Green
Write-Host "IP de Minikube: $minikubeIP" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs de acceso:" -ForegroundColor Yellow
Write-Host "  UI (Frontend): http://$minikubeIP`:30090" -ForegroundColor Cyan
Write-Host "  API (Backend): http://$minikubeIP`:30080" -ForegroundColor Cyan
Write-Host "  RabbitMQ Management: http://$minikubeIP`:30672" -ForegroundColor Cyan
Write-Host "    Usuario: admin" -ForegroundColor Gray
Write-Host "    Contraseña: admin123" -ForegroundColor Gray
Write-Host ""
Write-Host "Comandos útiles:" -ForegroundColor Yellow
Write-Host "  Ver pods: kubectl get pods -n task-manager" -ForegroundColor Gray
Write-Host "  Ver logs: kubectl logs -l app=task-manager-api -n task-manager" -ForegroundColor Gray
Write-Host "  Eliminar todo: kubectl delete namespace task-manager" -ForegroundColor Gray
