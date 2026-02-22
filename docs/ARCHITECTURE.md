# EFAMS Architecture

## Overview

EFAMS (Enterprise Facility / Asset Management System) is a full-stack, cross-platform system for managing assets, facilities, and maintenance work orders. Built as an Nx monorepo with Angular, Ionic, and **true .NET 6 microservices** — each service owns its own domain, database, and container.

![EFAMS Microservices Architecture](./EFAMS-Architecture.png)

---

## Repository Structure

```
efams/
├── apps/
│   ├── api-asset/          .NET 6 microservice — Asset domain (Port 5001)
│   ├── api-facility/       .NET 6 microservice — Facility domain (Port 5002)
│   ├── api-workorder/      .NET 6 microservice — Work Order domain (Port 5003)
│   ├── web-dashboard/      Angular 14 web app (Bootstrap, SCSS)
│   ├── mobile-field/       Ionic + Angular mobile app (Capacitor/Android)
│   ├── web-dashboard-e2e/  Cypress E2E tests for web-dashboard
│   └── mobile-field-e2e/   Cypress E2E tests for mobile-field
├── libs/                   Shared libraries (StencilJS UI components — Phase 6)
├── docs/                   Architecture and API documentation
├── docker-compose.yml      Runs all services locally (Phase 3+)
├── azure-pipelines.yml     Azure DevOps CI/CD pipeline
└── nx.json                 Nx workspace configuration
```

---

## Microservices

Each .NET 6 service is **independent**: own database, own Dockerfile, own Swagger, own test project. Services do **not** call each other synchronously — they communicate via **RabbitMQ events**.

### api-asset (Port 5001)

Owns the **Asset** domain.

| Folder | Purpose |
|--------|---------|
| `Entities/` | `Asset` (Id, Name, Description, CreatedAt) |
| `Data/` | `AssetDbContext`, EF Core migrations, index on `Name` |
| `Services/` | `IAssetService` / `AssetService` |
| `Controllers/` | `AssetsController` |

**Endpoints:** `GET/POST/PUT/DELETE /api/assets`, `GET /health`  
**Database:** `EfamsAssets` (SQL Server 2019)  
**Events published:** `AssetCreated` → RabbitMQ

---

### api-facility (Port 5002)

Owns the **Facility** domain.

| Folder | Purpose |
|--------|---------|
| `Entities/` | `Facility` (Id, Name, Address, CreatedAt) |
| `Data/` | `FacilityDbContext`, EF Core migrations |
| `Services/` | `IFacilityService` / `FacilityService` |
| `Controllers/` | `FacilitiesController` |

**Endpoints:** `GET/POST/PUT/DELETE /api/facilities`, `GET /health`  
**Database:** `EfamsFacilities` (SQL Server 2019)

---

### api-workorder (Port 5003)

Owns the **Work Order** domain.

| Folder | Purpose |
|--------|---------|
| `Entities/` | `WorkOrder` (Id, AssetId, Title, Description, Status, AssignedTo, CreatedAt) |
| `Data/` | `WorkOrderDbContext`, EF Core migrations |
| `Services/` | `IWorkOrderService` / `WorkOrderService` |
| `Controllers/` | `WorkOrdersController` |

**Endpoints:** `GET/POST/PUT/DELETE /api/workorders`, `GET /health`  
**Database:** `EfamsWorkOrders` (SQL Server 2019)  
**Events consumed:** `AssetCreated` ← RabbitMQ

---

## Shared Infrastructure

| Component | Purpose |
|-----------|---------|
| **RabbitMQ** | Async inter-service events (e.g. `AssetCreated`) |
| **Redis** | Response caching for read-heavy endpoints |
| **Keycloak** | OAuth 2 / OIDC identity provider; roles: `manager`, `field-worker` |
| **ELK Stack** | Centralised logging (Serilog → Elasticsearch → Kibana) |

---

## Frontend Apps

### web-dashboard (Angular 14)

Angular 14 SPA with Bootstrap 5 and SCSS. Calls all 3 microservices.

- Served locally: `npx nx serve web-dashboard`
- Build: `npx nx build web-dashboard`

### mobile-field (Ionic + Angular)

Ionic cross-platform mobile app. Packaged for Android via Capacitor. Calls api-asset and api-workorder.

- Served in browser: `npx nx serve mobile-field`
- Android: `npx nx build mobile-field && npx cap sync android && npx cap run android`

---

## CI/CD

Azure DevOps pipeline (`azure-pipelines.yml`) triggered on push to `main` or `master`:

| Job | Steps |
|-----|-------|
| **Frontend** | Node 18, `npm ci`, Nx lint/test/build for all projects |
| **Backend** | .NET 6 SDK, restore and build api-asset, api-facility, api-workorder |

Source code on **GitHub**; Azure DevOps for pipelines only.

---

## Running locally

### Prerequisites
- Node 18+
- .NET 6 SDK
- Docker Desktop
- JDK 17 (for Android builds)

### Start all services (Phase 3+)

```bash
docker-compose up
```

This starts all 3 APIs, 3 SQL Server databases, Redis, RabbitMQ, Keycloak, and ELK.

### Start api-asset only (Phase 2)

```bash
docker start efams-sql
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

## Phase Roadmap

| Phase | Status | Focus |
|-------|--------|-------|
| 1 | Done | Nx monorepo, Angular, Ionic, .NET skeleton |
| 2 | Done | api-asset data layer, SQL Server, CI/CD, docs |
| 3 | Planned | Full CRUD, api-facility, api-workorder, Docker, Redis, RabbitMQ |
| 4 | Planned | Angular/Ionic UI screens, Keycloak, OAuth 2, OWASP |
| 5 | Planned | xUnit + Moq (all services), Storybook, ELK |
| 6 | Planned | StencilJS reusable web components |
