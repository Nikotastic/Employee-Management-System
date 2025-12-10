# EmployeeManagementSystem

A complete Employee Management System for TalentoPlus S.A.S — ASP.NET Core 8 application with a Web Admin (MVC), REST API, PostgreSQL database, Excel import, PDF generation and a simple AI-assisted dashboard.

This README documents how to run the solution (Docker Compose), useful URLs, common troubleshooting (including JWT issues when Swagger works but the front-end shows "Unauthorized"), environment variables, default credentials and testing commands.

## Quick summary

- Repository name: EmployeeManagementSystem
- Web (Admin MVC): http://localhost/ (port 80)
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- PgAdmin: http://localhost:8080
- PostgreSQL: localhost:5432

> These URLs assume you run the provided Docker Compose. If you changed ports in `compose.yaml` or your environment, use the mapped ports shown by `docker compose ps`.

## What is included

- EmployeeManagementSystem.Api — REST API (JWT protected endpoints)
- EmployeeManagementSystem.Web — MVC admin interface (ASP.NET Core Identity)
- EmployeeManagementSystem.Infrastructure — EF Core, migrations, repositories and services
- PostgreSQL and PgAdmin running via Docker Compose
- Excel import and PDF generation features
- Tests project with unit and integration tests

## Prerequisites

- .NET 8.0 SDK (for local development)
- Docker and Docker Compose (v2 recommended — command: `docker compose`)
- Git

## Run with Docker Compose (recommended)

1. From repository root:

    docker compose up --build -d

2. Check running services and ports:

    docker compose ps

3. Visit the services:

- Web (Admin): http://localhost/ or http://localhost:80
- API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger
- PgAdmin: http://localhost:8080

If a port is already used on your machine, change the ports in `compose.yaml` before `docker compose up` or stop the conflicting service.

## Environment variables (overview)

The project reads sensitive values from environment variables. Typical variables used are:

- Database:
  - POSTGRES_DB
  - POSTGRES_USER
  - POSTGRES_PASSWORD
  - PG_PORT

- JWT:
  - JWT__Key
  - JWT__Issuer
  - JWT__Audience
  - JWT__ExpiryMinutes

- Email (SMTP):
  - EmailSettings__SmtpServer
  - EmailSettings__SmtpPort
  - EmailSettings__SenderEmail
  - EmailSettings__Password

- AI (optional):
  - AISettings__ApiKey
  - AISettings__Model

If you use Docker Compose, these values are normally configured in an `.env` file or directly in `compose.yaml`. Do not commit secrets into source control.

## Default credentials (development)

- Admin web (MVC):
  - Email: admin@talentoplus.com
  - Password: Admin123!

- PgAdmin default (if configured like this in compose):
  - Email: admin@talentoplus.com
  - Password: admin123

Use the admin account to log into the MVC admin site and to get admin-level JWTs via the API if needed.

## Common troubleshooting — Swagger shows token OK but front-end gets "Unauthorized"

If Swagger allows you to Authorize with a JWT and your API endpoints work there, but the Web front-end reports "Unauthorized" or login fails with "Invalid login attempt", check the following areas:

1. CORS and endpoints
   - Ensure the API has CORS configured to allow the front-end origin (for default Docker Compose both are on localhost but different ports; configure allowed origins accordingly).
   - Example: allow `http://localhost` and `http://localhost:80` if the MVC app calls the API from the browser.

2. Authorization header
   - The front-end must send the header exactly as `Authorization: Bearer {token}` (no extra quotes or spaces).
   - When using fetch/Axios, ensure the header is set and not blocked by the browser due to CORS preflight.

3. Cookie vs Bearer token confusion
   - The MVC Admin uses ASP.NET Core Identity cookies; the API uses JWT Bearer tokens. These are separate authentication schemes.
   - If the front-end is an SPA or makes direct AJAX calls to the API, you must attach the JWT to each request.
   - If you rely on cookies, verify the cookie domain/path and that cookies are forwarded in requests (withCredentials=true) and that the API expects cookie authentication.

4. JWT Issuer/Audience/Clock skew/Expiry
   - Ensure `JWT__Issuer` and `JWT__Audience` used by the front-end (or created by the login endpoint) match the API configuration.
   - Check token expiry. Try issuing a fresh token and using it immediately.
   - Check system time in containers; large time skew can make tokens invalid.

5. Development vs Production settings
   - In Development the appsettings.Development.json and environment variables may be different; verify which configuration is loaded inside the running container.

6. Inspect logs
   - Check API logs for authentication errors. Example:

       docker compose logs api --tail 200

   - Look for messages from Microsoft.AspNetCore.Authentication indicating why the token was rejected.

7. Swagger vs browser difference
   - Swagger sends the Authorization header directly to the API. The browser front-end may be subject to CORS preflight and not actually send the header if the server rejects preflight or lacks Access-Control-Allow-Headers for Authorization.
   - Ensure the API allows the `Authorization` header in CORS: Access-Control-Allow-Headers: Authorization, Content-Type

If you'd like, I can make a minimal checklist of exact config lines (`Program.cs`/CORS and JWT options) to add — but you asked not to change more files than necessary. First check the logs and CORS config.

## Database migrations

- Migrations are configured in `EmployeeManagementSystem.Infrastructure` and usually applied on application startup inside the API and/or web projects.
- If migrations are not applied, run locally:

    dotnet ef database update --project EmployeeManagementSystem.Infrastructure --startup-project EmployeeManagementSystem.Api

Or rely on Docker startup which runs migrations automatically if that behavior is enabled.

If you want to reset database state in Docker:

    docker compose down -v
    docker compose up --build -d

This removes volumes and recreates the database (data will be lost).

## Running tests

Run the test suite locally with dotnet:

    dotnet test

You can filter unit vs integration tests using dotnet test filters if the solution uses test categories.

## Useful Docker commands

- Start in detached mode:

    docker compose up --build -d

- View service status and port mappings:

    docker compose ps

- View logs for a service (api, web, db, pgadmin):

    docker compose logs api --follow

- Stop and remove containers (optionally remove volumes):

    docker compose down

### GITHUB: https://github.com/Nikotastic/Employee-Management-System.git