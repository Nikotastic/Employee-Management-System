#!/bin/bash

echo "🔧 Solucionando problema de inicio de sesión..."
echo ""

# Detener servicios
echo "1. Deteniendo servicios..."
cd /home/Coder/RiderProjects/EmployeeManagementSystem
docker compose down -v

# Reconstruir imágenes
echo ""
echo "2. Reconstruyendo imágenes (esto puede tardar unos minutos)..."
docker compose build api web

# Iniciar servicios
echo ""
echo "3. Iniciando servicios..."
docker compose up -d

# Esperar a que la base de datos esté lista
echo ""
echo "4. Esperando a que los servicios inicien (40 segundos)..."
sleep 40

# Verificar logs
echo ""
echo "5. Verificando inicialización..."
docker compose logs api | grep -E "Administrator user created|Database initialized" || echo "⚠️  No se encontró mensaje de usuario creado"

echo ""
echo "6. Verificando estado de servicios..."
docker compose ps

echo ""
echo "✅ Proceso completado!"
echo ""
echo "🔐 Intenta iniciar sesión con:"
echo "   📧 Email: admin@talentoplus.com"
echo "   🔑 Password: Admin123!"
echo ""
echo "🌐 URLs:"
echo "   Web: http://localhost"
echo "   API: http://localhost:5000/swagger"
echo ""
echo "💡 Si aún no funciona, ejecuta:"
echo "   docker compose logs api --tail=100"
echo "   para ver los errores detallados"

