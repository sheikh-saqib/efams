EFAMS — Enterprise Facility / Asset Management System
EFAMS is an end‑to‑end asset and facility management system built as a modern, scalable full‑stack application.
Frontend: Angular 14 web dashboard (Bootstrap, SCSS) and Ionic cross‑platform mobile app (Capacitor, Android) for field users.
Backend: .NET 6 microservice API with clean separation (controllers, services, data access), SQL Server for persistence (EF Core, migrations, and performance‑aware queries), and optional event‑driven flows via RabbitMQ with Redis for caching.
Infrastructure & DevOps: Nx monorepo; Azure DevOps pipelines for build, test, and lint; Docker and docker-compose for API, SQL Server, Redis, and RabbitMQ (and optionally ELK).
Security & identity: OAuth 2 / JWT; Keycloak as IdP; secure coding practices aligned with OWASP (parameterized queries, validation, HTTPS, headers).
Quality & observability: Backend unit tests (xUnit, Moq); frontend unit tests (Jest) and component documentation (Storybook); ELK stack for centralized logging and a sample Kibana dashboard.
Reuse: StencilJS web components consumed by both Angular apps; design and API documentation in the repo.
Suitable as a reference implementation for enterprise-style full‑stack, cross‑platform, and DevOps practices.
