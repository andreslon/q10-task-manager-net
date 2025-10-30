@echo off
echo Instalando Minikube en Windows...

REM Verificar si Chocolatey esta instalado
where choco >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo Instalando Chocolatey...
    powershell -Command "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))"
)

REM Instalar Minikube
echo Instalando Minikube...
choco install minikube -y

REM Verificar instalacion
echo Verificando instalacion...
minikube version

REM Iniciar Minikube
echo Iniciando Minikube...
minikube start

REM Habilitar addons
echo Habilitando addons...
minikube addons enable ingress
minikube addons enable dashboard

REM Verificar estado
echo Verificando estado de Minikube...
minikube status

echo.
echo Minikube instalado y configurado correctamente!
echo Puedes continuar con el despliegue usando los scripts en este directorio.
pause
