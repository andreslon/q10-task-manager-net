# Script de PowerShell para instalar Minikube en Windows
# Ejecutar como Administrador

Write-Host "Instalando Minikube en Windows..." -ForegroundColor Green

# Verificar si Chocolatey está instalado
if (!(Get-Command choco -ErrorAction SilentlyContinue)) {
    Write-Host "Instalando Chocolatey..." -ForegroundColor Yellow
    Set-ExecutionPolicy Bypass -Scope Process -Force
    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
    iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
}

# Instalar Minikube usando Chocolatey
Write-Host "Instalando Minikube..." -ForegroundColor Yellow
choco install minikube -y

# Verificar instalación
Write-Host "Verificando instalación..." -ForegroundColor Yellow
minikube version

# Iniciar Minikube
Write-Host "Iniciando Minikube..." -ForegroundColor Yellow
minikube start

# Habilitar addons
Write-Host "Habilitando addons..." -ForegroundColor Yellow
minikube addons enable ingress
minikube addons enable dashboard

# Verificar estado
Write-Host "Verificando estado de Minikube..." -ForegroundColor Yellow
minikube status

Write-Host "¡Minikube instalado y configurado correctamente!" -ForegroundColor Green
Write-Host "Puedes continuar con el despliegue usando los scripts en este directorio." -ForegroundColor Cyan
