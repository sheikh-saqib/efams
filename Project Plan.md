# EFAMS Project Plan

EFAMS (Enterprise Facility / Asset Management System) is a full-stack, cross-platform system for managing assets, facilities, and maintenance work orders. Built as an Nx monorepo with Angular, Ionic, and **true .NET 6 microservices** — each service owns its own domain, database, and container.

---

## Microservices Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Nx Monorepo (efams)                     │
├────────────────────┬────────────────────┬───────────────────┤
│   apps/            │   apps/            │   apps/           │
│   web-dashboard    │   mobile-field     │   api-gateway     │
│   (Angular 14)     │   (Ionic/Angular)  │   (optional)      │
├────────────────────┴────────────────────┴───────────────────┤
│                    .NET 6 Microservices                      │
│  ┌──────────────┐ ┌────────────────┐ ┌──────────────────┐  │
│  │  api-asset   │ │  api-facility  │ │  api-workorder   │  │
│  │  (Assets DB) │ │ (Facilities DB)│ │ (WorkOrders DB)  │  │
│  └──────┬───────┘ └───────┬────────┘ └────────┬─────────┘  │
│         └─────────────────┼──────────────────┘             │
│                     RabbitMQ (events)                        │
│                     Redis   (caching)                        │
│                     ELK     (logging)                        │
└─────────────────────────────────────────────────────────────┘
```

### Services

| Service | Domain | Database | Port |
|---------|--------|----------|------|
| **api-asset** | Asset CRUD | `EfamsAssets` (SQL Server) | 5001 |
| **api-facility** | Facility CRUD | `EfamsFacilities` (SQL Server) | 5002 |
| **api-workorder** | Work order CRUD | `EfamsWorkOrders` (SQL Server) | 5003 |

Each service is an **independent .NET 6 Web API** with its own:
- Domain entities and EF Core DbContext.
- SQL Server database (separate schema, separate container volume).
- Dockerfile and docker-compose service entry.
- Swagger/OpenAPI spec.
- Unit test project.

Services communicate **asynchronously via RabbitMQ** (e.g. api-asset publishes `AssetCreated`; api-workorder listens and can auto-create an onboarding work order). They do **not** call each other synchronously.

---

## Phase 1 — Foundation (Done)

**What was built:**
- Nx monorepo with `apps/` and `libs/` structure.
- **web-dashboard** — Angular 14, Bootstrap 5, SCSS.
- **mobile-field** — Ionic + Angular, Capacitor/Android.
- **api-asset** — .NET 6 Web API skeleton.
- Jest (frontend unit tests), Cypress (E2E).

---

## Phase 2 — api-asset Data Layer and CI/CD (Done)

**What was built:**
- **Asset entity** (`Id`, `Name`, `Description`, `CreatedAt`) with EF Core, SQL Server 2019 (Docker), migrations, index on `Name`.
- **GET endpoints** — `GET /api/assets`, `GET /api/assets/{id}` via `AssetService`.
- **Azure DevOps pipeline** (`azure-pipelines.yml`) — Node (Nx lint/test/build) + .NET (restore/build) jobs; GitHub as source.
- **docs/ARCHITECTURE.md** — design and architecture document.
- **Swagger/OpenAPI** — at `/swagger` in Development.

---

## Phase 3 — Full CRUD, New Microservices, Containers, and Messaging

**Goal:** Complete api-asset, add api-facility and api-workorder as independent microservices, containerise everything, wire up Redis and RabbitMQ.

### api-asset (complete)
- Full CRUD: `POST`, `PUT`, `DELETE /api/assets`.
- Health check: `/health`.
- Dockerfile.

### api-facility (new microservice)
- **Facility entity** (`Id`, `Name`, `Address`, `CreatedAt`).
- Full CRUD: `GET`, `POST`, `PUT`, `DELETE /api/facilities`.
- Own SQL Server database (`EfamsFacilities`), EF Core, migrations, index on `Name`.
- Health check: `/health`.
- Dockerfile.

### api-workorder (new microservice)
- **WorkOrder entity** (`Id`, `AssetId`, `Title`, `Description`, `Status`, `AssignedTo`, `CreatedAt`).
- Full CRUD: `GET`, `POST`, `PUT`, `DELETE /api/workorders`.
- Own SQL Server database (`EfamsWorkOrders`), EF Core, migrations.
- Listens for `AssetCreated` event from RabbitMQ and logs/processes it.
- Health check: `/health`.
- Dockerfile.

### Infrastructure
- **docker-compose.yml** at repo root — one command runs all 3 APIs + 3 SQL Server DBs + Redis + RabbitMQ.
- **Redis caching** — api-asset caches `GET /api/assets`; api-facility caches `GET /api/facilities`; invalidate on write.
- **RabbitMQ** — api-asset publishes `AssetCreated`; api-workorder consumes it.
- **Update azure-pipelines.yml** — add build jobs for api-facility and api-workorder.

### What this demonstrates
- True microservices (3 independent services, 3 databases, async communication).
- Containerisation and cloud-native patterns.
- Message queues (RabbitMQ) and Redis caching.

---

## Phase 4 — Frontend UI Screens and Security

**Goal:** Make the apps real and usable. Add login, auth, and secure coding across all services.

### web-dashboard (Angular)
- **Home dashboard** — summary cards: total assets, total facilities, open work orders. Calls all 3 APIs.
- **Asset list page** — filterable/sortable table; calls `GET /api/assets` (api-asset).
- **Asset detail page** — view/edit; calls `GET /api/assets/{id}`, `PUT`.
- **Add asset form** — calls `POST /api/assets`.
- **Facility list page** — calls `GET /api/facilities` (api-facility).
- **Work order list** — manager creates, assigns, closes work orders; calls api-workorder.

### mobile-field (Ionic)
- **Asset search/list** — search assets; calls api-asset.
- **Asset detail** — view asset info and status.
- **Report issue** — creates a work order via api-workorder.
- **My work orders** — field worker sees their assigned work orders.

### Security (all 3 services)
- **Keycloak** in docker-compose — OAuth 2 / OIDC identity provider; realm and clients for "efams"; roles: `manager`, `field-worker`.
- **JWT bearer auth** on all 3 APIs — write endpoints protected; role-based (e.g. only `manager` can delete).
- **Angular and Ionic** — login via Keycloak; token sent with all API calls.
- **OWASP practices** — input validation on POST/PUT (all 3 services), security headers, parameterised queries (EF Core), HTTPS in production.
- **Security** section updated in `docs/ARCHITECTURE.md`.

### What this demonstrates
- Angular + Ionic building real cross-platform UI across multiple backend services.
- OAuth 2, Keycloak (IAM), OWASP secure coding.

---

## Phase 5 — Testing and Observability

**Goal:** Unit tests for all 3 services, frontend component docs, and centralised logging across all microservices.

### Testing
- **xUnit + Moq** test projects for each service:
  - `apps/api-asset.Tests/` — tests for `AssetService`, `AssetsController`.
  - `apps/api-facility.Tests/` — tests for `FacilityService`, `FacilitiesController`.
  - `apps/api-workorder.Tests/` — tests for `WorkOrderService`, `WorkOrdersController`, and RabbitMQ consumer.
- All 3 test projects wired into Azure DevOps pipeline (`dotnet test`).
- **Storybook** on web-dashboard (`@nrwl/storybook`) — stories for 3 components: asset card, status badge, work order row.

### Observability
- **ELK stack** in docker-compose — Elasticsearch + Logstash + Kibana.
- **Serilog** in all 3 APIs — structured logs sent to Elasticsearch.
- Kibana dashboards: request counts per service, errors, work order status events.

### What this demonstrates
- Backend unit tests (xUnit/NUnit + Moq) across multiple services.
- Frontend component testing (Storybook).
- ELK stack centralised logging for microservices.

---

## Phase 6 — Shared UI Components (StencilJS)

**Goal:** Extract real, reusable UI components shared between web-dashboard and mobile-field.

### StencilJS library (`libs/ui-components/`)
- `efams-status-badge` — coloured badge for asset/work order status (Active, In Repair, Decommissioned, Closed).
- `efams-asset-card` — card showing asset name, facility, and status.
- `efams-loading-spinner` — shared loading indicator used across all pages.

Both **web-dashboard** and **mobile-field** consume these components, replacing any inline equivalents.

### What this demonstrates
- Reusable web components built with StencilJS, consumed by Angular applications.

## Full Coverage Checklist

| Area | Phase |
|------|-------|
| Angular 14 + Ionic, cross-platform apps | 1, 4 |
| SCSS + Bootstrap | 1 |
| Nx monorepo | 1 |
| .NET 6 microservices, clean architecture | 2, 3 |
| SQL Server (EF Core, modeling, index, queries) — per service | 2, 3 |
| Azure DevOps CI/CD (all services) | 2, 3 |
| Technical artefacts (design doc, API spec) | 2 |
| Docker, containerisation, cloud-native | 3 |
| Message queues (RabbitMQ) inter-service events | 3 |
| Redis caching | 3 |
| Real frontend UI (web + mobile) calling all services | 4 |
| OAuth 2 + JWT | 4 |
| Keycloak (IAM) with roles | 4 |
| OWASP secure coding (all services) | 4 |
| Backend unit tests (xUnit + Moq) — all 3 services | 5 |
| Frontend component tests (Jest + Storybook) | 1, 5 |
| ELK stack centralised logging across microservices | 5 |
| StencilJS reusable web components | 6 |
