# 🚀 Sistema de Gestión de Empleados - TalentoPlus S.A.S

Sistema completo de gestión de recursos humanos desarrollado con **ASP.NET Core 8.0**, **PostgreSQL**, **Clean Architecture** y servicios de IA integrados.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Arquitectura](#arquitectura)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejecución con Docker](#ejecución-con-docker)
- [Endpoints de la API](#endpoints-de-la-api)
- [Credenciales de Acceso](#credenciales-de-acceso)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)

---

## ✨ Características

### Aplicación Web (MVC)
- ✅ **Dashboard interactivo** con métricas en tiempo real
- ✅ **Asistente de IA** para consultas en lenguaje natural (Gemini)
- ✅ **CRUD completo** de empleados
- ✅ **Importación masiva** desde archivos Excel
- ✅ **Generación de PDFs** con hojas de vida
- ✅ **Autenticación** con ASP.NET Core Identity

### API REST
- ✅ **Autoregistro público** de empleados
- ✅ **Autenticación JWT** para empleados
- ✅ **Endpoints protegidos** para consulta de información personal
- ✅ **Descarga de CV** en PDF
- ✅ **Importación de Excel**
- ✅ **CRUD completo** de empleados (admin)
- ✅ **Swagger/OpenAPI** con autenticación JWT integrada

### Funcionalidades Clave
- ✅ **Envío automático de correos** de bienvenida (Gmail SMTP)
- ✅ **Importación inteligente** desde Excel (columnas en español)
- ✅ **Creación automática** de departamentos y cargos
- ✅ **Generación profesional** de hojas de vida en PDF (QuestPDF)
- ✅ **Consultas de IA** sobre datos de empleados (Gemini AI)

---

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture** con las siguientes capas:

```
├── EmployeeManagementSystem.Domain        # Entidades, Value Objects, Enums
├── EmployeeManagementSystem.Application   # DTOs, Interfaces, Servicios, Validadores
├── EmployeeManagementSystem.Infrastructure # Repositorios, EF Core, Servicios externos
├── EmployeeManagementSystem.Api           # API REST con JWT
├── EmployeeManagementSystem.Web           # Aplicación MVC (Admin)
└── EmployeeManagementSystem.Tests         # Pruebas unitarias e integración
```

### Patrones Implementados
- ✅ Repository Pattern
- ✅ Dependency Injection
- ✅ CQRS (Command Query Responsibility Segregation)
- ✅ Value Objects
- ✅ Domain-Driven Design (DDD)

---

## 📦 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) y [Docker Compose](https://docs.docker.com/compose/install/)
- [PostgreSQL 16](https://www.postgresql.org/download/) (opcional, se levanta con Docker)
- [Git](https://git-scm.com/)

---

## ⚙️ Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd EmployeeManagementSystem
```

### 2. Configurar Variables de Entorno

El archivo `.env` ya está configurado con valores por defecto. **NO modificar a menos que sea necesario**.

```env
# Base de Datos
POSTGRES_DB=TalentoPlusDB
POSTGRES_USER=postgres
POSTGRES_PASSWORD=niko
PG_PORT=5432

# JWT
JWT__Key=lu34CLysz31gQwSvh9XgT1q5QoOjIovEu1YlJQZyB5Qo4qUGcuZztxybdjwJgF2d
JWT__Issuer=TalentoPlusAPI
JWT__Audience=TalentoPlusClient
JWT__ExpiryMinutes=15

# Email (Gmail)
EmailSettings__SmtpServer=smtp.gmail.com
EmailSettings__SmtpPort=587
EmailSettings__SenderEmail=velasqueznikol10@gmail.com
EmailSettings__Password=dxvo xvlv pdtm yoxv

# IA (Gemini)
AISettings__ApiKey=AIzaSyCXgQbSQeFABXYnxYXCtrbM7o-UXSJTFws
AISettings__Model=gemini-2.0-flash-exp
```

---

## 🐳 Ejecución con Docker

### Levantar toda la solución

```bash
docker-compose up --build
```

Esto levantará:
- **PostgreSQL** en el puerto `5432`
- **PgAdmin** en `http://localhost:8080`
- **API REST** en `http://localhost:5000`
- **Aplicación Web** en `http://localhost:80`

### Acceder a los servicios

| Servicio | URL | Descripción |
|----------|-----|-------------|
| Aplicación Web | http://localhost:80 | Portal del administrador |
| API REST | http://localhost:5000 | Endpoints REST |
| Swagger UI | http://localhost:5000/swagger | Documentación interactiva |
| PgAdmin | http://localhost:8080 | Administrador de BD |

### Aplicar migraciones (automático)

Las migraciones se aplican automáticamente al iniciar la aplicación. El sistema también crea:
- ✅ Departamentos iniciales
- ✅ Cargos (JobPositions) iniciales
- ✅ Usuario administrador por defecto

---

## 🔐 Credenciales de Acceso

### Aplicación Web (Administrador)
```
Usuario: admin@talentoplus.com
Contraseña: Admin123!
```

### PgAdmin
```
Email: admin@talentoplus.com
Contraseña: admin123
```

### API REST (JWT)
Para obtener un token JWT, hacer POST a `/api/auth/login`:
```json
{
  "email": "admin@talentoplus.com",
  "password": "Admin123!"
}
```

---

## 📡 Endpoints de la API

### Públicos (sin autenticación)

#### Autenticación
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "empleado@correo.com",
  "password": "documento"
}
```

#### Autoregistro de Empleado
```http
POST /api/employees/register
Content-Type: application/json

{
  "document": "123456789",
  "firstName": "Juan",
  "lastName": "Pérez",
  "birthDate": "1990-01-01",
  "address": "Calle 123",
  "email": "juan.perez@correo.com",
  "phone": "3001234567",
  "jobPositionId": 1,
  "salary": 3000000,
  "hiringDate": "2024-01-01",
  "status": 0,
  "educationLevel": 3,
  "professionalProfile": "Profesional en sistemas",
  "departmentId": 1
}
```

#### Listar Departamentos
```http
GET /api/departments
```

### Protegidos (requieren JWT)

#### Información del Empleado Autenticado
```http
GET /api/employees/me
Authorization: Bearer {token}
```

#### Descargar CV del Empleado
```http
GET /api/employees/me/cv
Authorization: Bearer {token}
```

### Administración (requieren autenticación)

#### Listar Todos los Empleados
```http
GET /api/employees
Authorization: Bearer {token}
```

#### Obtener Empleado por ID
```http
GET /api/employees/{id}
Authorization: Bearer {token}
```

#### Actualizar Empleado
```http
PUT /api/employees/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

#### Eliminar Empleado
```http
DELETE /api/employees/{id}
Authorization: Bearer {token}
```

#### Importar desde Excel
```http
POST /api/employees/import-excel
Authorization: Bearer {token}
Content-Type: multipart/form-data

file: [archivo.xlsx]
```

#### Generar PDF de Empleado
```http
GET /api/employees/{id}/cv
Authorization: Bearer {token}
```

---

## 📂 Estructura del Proyecto

```
EmployeeManagementSystem/
│
├── EmployeeManagementSystem.Domain/
│   ├── Entities/           # Employee, Department, JobPosition
│   ├── ValueObjects/       # FullName, ContactInfo, JobInfo, EducationInfo
│   ├── Enums/              # EmployeeStatus, EducationLevel
│   ├── Interfaces/         # IEmployeeRepository, IDepartmentRepository
│   └── Exceptions/         # Custom exceptions
│
├── EmployeeManagementSystem.Application/
│   ├── DTOs/               # CreateEmployeeDto, EmployeeDto, LoginDto
│   ├── Interfaces/         # IEmployeeService, IAuthService, IAIService
│   ├── Services/           # EmployeeService
│   └── Validators/         # FluentValidation validators
│
├── EmployeeManagementSystem.Infrastructure/
│   ├── Data/               # ApplicationDbContext, DataSeeder
│   ├── Repositories/       # EmployeeRepository, DepartmentRepository
│   ├── Services/           # AuthService, EmailService, PdfService, ExcelService, GeminiAIService
│   ├── Identity/           # ApplicationUser
│   ├── Persistence/        # Entity configurations
│   └── Migrations/         # EF Core migrations
│
├── EmployeeManagementSystem.Api/
│   ├── Controllers/        # AuthController, EmployeesController, DepartmentsController
│   └── Program.cs
│
├── EmployeeManagementSystem.Web/
│   ├── Controllers/        # AccountController, DashboardController, EmployeesController
│   ├── Views/              # Login, Dashboard, Employees (CRUD)
│   ├── Models/             # ViewModels
│   └── Program.cs
│
├── EmployeeManagementSystem.Tests/
│   ├── UnitTests/
│   └── IntegrationTests/
│
├── compose.yaml            # Docker Compose configuration
├── .env                    # Environment variables
└── README.md              # Este archivo
```

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8.0** - Framework principal
- **Entity Framework Core 8.0** - ORM
- **PostgreSQL** - Base de datos
- **ASP.NET Core Identity** - Autenticación y autorización
- **JWT (JSON Web Tokens)** - Autenticación API

### Librerías y Servicios
- **FluentValidation** - Validaciones
- **AutoMapper** - Mapeo de objetos
- **QuestPDF** - Generación de PDFs
- **EPPlus** - Lectura/escritura de Excel
- **MailKit** - Envío de correos
- **Google Gemini AI** - Inteligencia artificial
- **Swagger/OpenAPI** - Documentación API

### Frontend
- **Bootstrap 5** - Framework CSS
- **jQuery** - Manipulación DOM
- **Font Awesome** - Iconos

### DevOps
- **Docker** - Contenedores
- **Docker Compose** - Orquestación

### Testing
- **xUnit** - Framework de testing
- **FluentAssertions** - Assertions expresivas
- **Moq** - Mocking framework
- **Microsoft.AspNetCore.Mvc.Testing** - Testing de APIs

---

## 🧪 Tests Automatizados

El sistema cuenta con una **suite completa de 77 tests** que garantizan la calidad del código:

### Resumen de Tests
- **Total:** 77 tests
- **Tests Unitarios:** 66
- **Tests de Integración:** 11
- **Cobertura:** Dominio, Aplicación, Infraestructura y API
- **Estado:** ✅ 100% pasando

### Ejecutar Tests

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar solo tests unitarios
dotnet test --filter "FullyQualifiedName~UnitTests"

# Ejecutar solo tests de integración
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Scripts con Validación de Tests

El sistema incluye scripts que **validan los tests antes de iniciar** la aplicación. Si algún test falla, la aplicación no arranca:

```bash
# Iniciar API (con validación de tests)
./start-api.sh

# Iniciar Web (con validación de tests)
./start-web.sh
```

Para más detalles sobre los tests, ver [TESTS_RESUMEN.md](TESTS_RESUMEN.md)

---

## 📊 Características del Excel de Importación

El sistema puede importar empleados desde archivos Excel con las siguientes columnas (en español):

| Columna | Valores Permitidos |
|---------|-------------------|
| Documento | Texto |
| Nombres | Texto |
| Apellidos | Texto |
| Fecha de Nacimiento | Fecha |
| Dirección | Texto |
| Correo / Email | Email válido |
| Teléfono | Texto |
| Cargo | Texto (se crea si no existe) |
| Departamento | Texto (se crea si no existe) |
| Salario | Número |
| Fecha de Ingreso | Fecha |
| Estado | Activo / Inactivo / Vacaciones |
| Nivel Educativo | Bachiller / Técnico / Tecnólogo / Profesional / Especialización / Maestría / Doctorado |
| Perfil Profesional | Texto |

---

## 🧪 Pruebas

### Ejecutar todas las pruebas
```bash
dotnet test
```

### Ejecutar pruebas específicas
```bash
dotnet test --filter "FullyQualifiedName~EmployeeServiceTests"
```

---

## 📝 Notas Adicionales

### Niveles Educativos Soportados
1. **Bachiller** (HighSchool)
2. **Técnico** (Technical)
3. **Tecnólogo** (Technologist)
4. **Profesional** (Professional)
5. **Especialización** (Specialization)
6. **Maestría** (Master)
7. **Doctorado** (Doctorate)

### Estados de Empleado
- **Active** (Activo)
- **Inactive** (Inactivo)
- **Vacation** (Vacaciones)

---

## 📧 Contacto y Soporte

Para preguntas o problemas, contactar a:
- **Email**: velasqueznikol10@gmail.com

---

## 📄 Licencia

Este proyecto fue desarrollado como prueba técnica para TalentoPlus S.A.S.

---

**Desarrollado con ❤️ usando Clean Architecture y .NET 8.0**

