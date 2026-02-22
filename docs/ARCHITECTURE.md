# EFAMS Architecture

## Overview

EFAMS (Enterprise Facility / Asset Management System) is a full-stack, cross-platform asset management system built as an Nx monorepo. It combines an Angular web dashboard, an Ionic mobile app for field use, and a .NET 6 Web API backed by SQL Server.

---

## Repository Structure

```
efams/
├── apps/
│   ├── api-asset/          .NET 6 Web API (asset microservice)
│   ├── web-dashboard/      Angular 14 web app (Bootstrap, SCSS)
│   ├── mobile-field/       Ionic + Angular mobile app (Capacitor/Android)
│   ├── web-dashboard-e2e/  Cypress E2E tests for web-dashboard
│   └── mobile-field-e2e/   Cypress E2E tests for mobile-field
├── libs/                   Shared libraries
├── docs/                   Architecture and API documentation
├── azure-pipelines.yml     Azure DevOps CI/CD pipeline
├── nx.json                 Nx workspace configuration
└── package.json            Node dependencies
```

---

## api-asset (.NET 6 Web API)

The asset microservice owns the asset domain and its own database.

**Layers:**

| Folder | Purpose |
|--------|---------|
| `Entities/` | Domain models (e.g. `Asset`: Id, Name, Description, CreatedAt) |
| `Data/` | `AssetDbContext` (EF Core), migrations, index on `Asset.Name` for performance |
| `Services/` | `IAssetService` / `AssetService` — business/data access logic |
| `Controllers/` | `AssetsController` — HTTP endpoints |

**Endpoints:**

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/assets` | Returns all assets |
| GET | `/api/assets/{id}` | Returns a single asset by id (404 if not found) |

**Database:** SQL Server 2019 (Docker container). One database per microservice (`EfamsAssets`). EF Core manages schema via code-first migrations.

**API spec:** Swagger/OpenAPI available at `/swagger` when running in Development mode.

**Connection string:** stored in `appsettings.Development.json` (`ConnectionStrings:DefaultConnection`), pointing to the local SQL Server container.

---

## web-dashboard (Angular 14)

Angular 14 SPA with Bootstrap 5 and SCSS for the desktop management interface.

- Served locally: `npx nx serve web-dashboard`
- Build: `npx nx build web-dashboard`

---

## mobile-field (Ionic + Angular)

Ionic cross-platform mobile app using Angular 14. Packaged for Android via Capacitor.

- Served in browser: `npx nx serve mobile-field`
- Android: `npx nx build mobile-field && npx cap sync android && npx cap run android`

---

## CI/CD

Azure DevOps pipeline (`azure-pipelines.yml`) triggered on push to `main` or `master`:

| Job | Steps |
|-----|-------|
| **Frontend** | Node 18, `npm ci`, Nx lint/test/build for all projects |
| **Backend** | .NET 6 SDK, `dotnet restore`, `dotnet build apps/api-asset` |

Source code is hosted on **GitHub**. Azure DevOps is used for pipelines only (no Azure Repos).

---

## Running locally

### Prerequisites
- Node 18+
- .NET 6 SDK
- Docker Desktop
- JDK 17 (for Android builds)

### Start SQL Server

```bash
docker start efams-sql
# or first-time:
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<password>" -p 1433:1433 --name efams-sql -d mcr.microsoft.com/mssql/server:2019-latest
```

### Run the API

```bash
dotnet run --project apps/api-asset
```

### Run the web dashboard

```bash
npx nx serve web-dashboard
```

### Run the mobile app (browser)

```bash
npx nx serve mobile-field
```

---

## Future Phases

| Phase | Focus |
|-------|-------|
| 3 | Docker, docker-compose, Redis caching, RabbitMQ messaging, health checks |
| 4 | Keycloak (OAuth 2 / OIDC), JWT auth, OWASP security practices |
| 5 | xUnit + Moq backend tests, Storybook component docs, ELK centralized logging |
| 6 | StencilJS reusable web components consumed by Angular apps |
