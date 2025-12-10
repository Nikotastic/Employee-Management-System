#!/bin/bash

echo "🔄 Deteniendo servicios..."
docker compose down

echo "🗑️  Eliminando volúmenes de base de datos..."
docker compose down -v

echo "🚀 Iniciando servicios..."
docker compose up -d

echo "⏳ Esperando a que la base de datos se inicialice (30 segundos)..."
sleep 30

echo "📋 Verificando logs de la API..."
docker compose logs api | tail -50

echo ""
echo "✅ Proceso completado!"
echo ""
echo "🔐 Credenciales de acceso:"
echo "   Email: admin@talentoplus.com"
echo "   Contraseña: Admin123!"
echo ""
echo "🌐 URLs disponibles:"
echo "   Web: http://localhost"
echo "   API: http://localhost:5000"
echo "   Swagger: http://localhost:5000/swagger"
echo "   pgAdmin: http://localhost:8080"

