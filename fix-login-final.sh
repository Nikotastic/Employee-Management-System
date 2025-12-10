#!/bin/bash

echo "🔧 =========================================="
echo "🔧 SOLUCIONANDO PROBLEMA DE INICIO DE SESIÓN"
echo "🔧 =========================================="
echo ""

cd /home/Coder/RiderProjects/EmployeeManagementSystem

# Paso 1: Detener servicios y limpiar
echo "1️⃣ Deteniendo servicios y limpiando base de datos..."
docker compose down -v

# Paso 2: Reconstruir imágenes
echo ""
echo "2️⃣ Reconstruyendo imágenes (puede tardar 2-3 minutos)..."
docker compose build --no-cache api web

# Paso 3: Iniciar servicios
echo ""
echo "3️⃣ Iniciando servicios..."
docker compose up -d

# Paso 4: Esperar a que todo inicie
echo ""
echo "4️⃣ Esperando a que los servicios inicien (45 segundos)..."
for i in {1..45}; do
    echo -n "."
    sleep 1
done
echo ""

# Paso 5: Verificar logs del usuario administrador
echo ""
echo "5️⃣ Verificando creación del usuario administrador..."
echo ""
docker compose logs api | grep -A 3 -B 1 "Administrator user created\|✅"

# Paso 6: Estado de servicios
echo ""
echo "6️⃣ Estado de servicios:"
docker compose ps

echo ""
echo "🎉 =========================================="
echo "🎉 PROCESO COMPLETADO"
echo "🎉 =========================================="
echo ""
echo "🔐 Credenciales para iniciar sesión:"
echo "   📧 Email: admin@talentoplus.com"
echo "   🔑 Contraseña: Admin123!"
echo ""
echo "🌐 Accede a la aplicación en:"
echo "   💻 Web: http://localhost"
echo "   🔌 API: http://localhost:5000"
echo "   📚 Swagger: http://localhost:5000/swagger"
echo ""
echo "📋 Si aún tienes problemas, ejecuta:"
echo "   docker compose logs api | tail -100"
echo ""

