# Features

This document defines the feature set for the Investment Portfolio Tracker. Each feature is scoped to validate specific architectural patterns — every feature earns its place in the reference architecture.

---

## Domain entities

The system operates on five core entities:

- **User** — a registered account with email and hashed password, managed by .NET Identity
- **Asset** — a tradeable instrument (stock, crypto, bond, ETF) with a canonical ticker, name, and metadata
- **Portfolio** — a named collection of holdings owned by a user
- **Holding** — a position in a specific asset within a portfolio, referenced by asset ID
- **Trade** — an immutable record of a buy, sell, or dividend event against a holding

A simulated **price feed** iterates over the asset registry to publish market price ticks, providing a high-frequency Kafka producer without requiring an external API dependency.

---

## Authentication

Authentication uses JWT issued by the .NET API via ASP.NET Core Identity. No external identity provider is involved. The `Users` table lives in Postgres alongside the rest of the write model, giving portfolios a real foreign key to an authenticated user.

### Register
`POST /auth/register` accepts an email and password. Password is hashed and stored via ASP.NET Core Identity. Returns a `201` with no token — the user must log in after registering.

**Validates:** ASP.NET Core Identity setup, password hashing, user persistence in Postgres.

### Login
`POST /auth/login` validates credentials and returns a short-lived JWT access token and a longer-lived refresh token. The access token is signed with a secret key configured via environment variable.

**Validates:** JWT issuance, .NET Identity credential validation, token signing configuration.

### Refresh token
`POST /auth/refresh` accepts a valid refresh token and returns a new access token. Refresh tokens are stored in Postgres and invalidated on use (rotation). Expired or already-used refresh tokens return `401`.

**Validates:** Refresh token rotation pattern, token storage in Postgres, stateful token invalidation.

### Logout
`POST /auth/logout` invalidates the current refresh token in Postgres. The access token remains valid until it expires naturally — no token blacklist is maintained, keeping the implementation stateless on the access token side.

**Validates:** Refresh token revocation, stateless access token design decision.

### JWT middleware
All protected API endpoints require a valid `Authorization: Bearer <token>` header. A .NET middleware component validates the token signature, expiry, and issuer on every request. Unauthenticated requests return `401`. The authenticated user's ID is extracted from the token claims and used to scope all data access — a user can only access their own portfolios.

**Validates:** JWT validation middleware in .NET, claims-based identity, per-user data scoping.

---

## Asset registry

### Seed assets
A fixed set of assets (stocks, ETFs, crypto) is seeded into Postgres on startup via EF Core migrations or a seed script. Assets are the canonical reference for tickers across the entire system — the price feed, holdings, and trades all reference assets by ID.

**Validates:** Reference data seeding pattern, EF Core migrations, foreign key integrity between assets and holdings.

### Search and look up assets
A read endpoint allows searching assets by ticker symbol or name (e.g. "AAPL" or "Apple"). Used by the frontend when a user adds a holding to a portfolio. Query runs against Postgres — no Kafka involved.

**Validates:** Basic query pattern on the read side, API contract shape for Specmatic contract tests.

### Asset detail
A single endpoint returning full metadata for one asset by ID or ticker: name, type (stock / crypto / bond / ETF), exchange, and the latest known price from the DynamoDB read model.

**Validates:** Joining reference data (Postgres) with projected state (DynamoDB) at the API layer.

---

## Portfolio management

### Create and edit portfolio
A user can create a named portfolio with an optional description and a base currency. The portfolio is linked to the authenticated user's ID extracted from the JWT claims. Updates (rename, description change) are handled as commands through the MediatR pipeline.

**Validates:** CQRS command side, MediatR pipeline, Postgres as write store, user-scoped data via JWT claims.

### Add and remove holdings
A user can add a position to a portfolio by specifying an asset (by ID or ticker), quantity, and average cost price. Holdings reference assets by ID — not by raw ticker string. Each mutation publishes a domain event to Kafka.

**Validates:** Foreign key reference to asset entity, event publishing on write-side mutations, Kafka producer behaviour.

### Portfolio snapshot view
A read-optimised view of a portfolio showing all current holdings with asset name and ticker (from the asset registry), quantity, cost basis, current market value, and P&L. Served exclusively from the DynamoDB read model — not from Postgres.

**Validates:** CQRS read side, DynamoDB as a read store, eventual consistency between write and read models.

### Delete portfolio
Soft-deletes a portfolio. Publishes a `PortfolioDeleted` event; the worker tombstones the corresponding read model entries in DynamoDB.

**Validates:** Event-driven cleanup across services, tombstone pattern in the read store.

---

## Transaction tracking

### Record a trade
A user can record a buy, sell, or dividend event against a holding. Each trade is an immutable append to the event log — the authoritative source of truth. Input is validated through a MediatR pipeline behaviour before being persisted and published.

**Validates:** Immutable event log, MediatR validation pipeline, Kafka producer on command completion.

### Transaction history
A paginated, chronological list of all trades for a given portfolio. Read directly from the Postgres event log (not the DynamoDB read model), since this is a faithful historical record rather than a derived projection.

**Validates:** Query side of CQRS, Postgres for ordered event retrieval, pagination pattern.

### Recalculate holding on trade
The worker service consumes `TradeRecorded` events from Kafka and recomputes the holding's quantity, average cost basis, realised P&L, and unrealised P&L in the DynamoDB read model.

**Validates:** Kafka consumer, event-driven projection updates, DynamoDB write from worker.

---

## Price and valuation

### Price ingestion (simulated feed)
An in-process background service reads the asset registry to get the list of tracked tickers, then generates random price ticks and publishes `PriceUpdated` events to Kafka at a configurable rate. Using the asset registry as the source of tickers keeps the simulator consistent with the rest of the system. This is the primary high-throughput path — kept as a simulator to remove external API dependency and keep focus on infrastructure patterns.

**Validates:** High-frequency Kafka producer, consumer lag under load, throughput metrics in Grafana, asset registry as the canonical ticker source.

### Holding market value
The worker consumes `PriceUpdated` events and updates the current market value of each affected holding in DynamoDB by joining the latest price with the stored quantity.

**Validates:** Consumer group behaviour, DynamoDB upsert patterns, lag visibility in Prometheus.

### P&L calculation
Unrealised P&L is computed as `(current price − average cost) × quantity` and stored on the read model. Realised P&L is accumulated from sell events. Both are derived fields maintained by the worker — never stored on the write side.

**Validates:** Derived projection fields, separation between write model (raw events) and read model (computed state).

---

## Frontend

The frontend is a Next.js application with TypeScript and Tailwind CSS. It is scoped to be a realistic API consumer — enough to exercise every API endpoint and make the observability stack visible, without becoming a full product UI.

### Login page — `/login`
A login form with email and password fields. Submits to `POST /auth/login`. On success, stores the JWT access token and refresh token in `httpOnly` cookies and redirects to the dashboard. Displays validation errors inline.

### Register page — `/register`
A registration form with email, password, and confirm password fields. Submits to `POST /auth/register`. On success, redirects to `/login` with a success message.

### Token refresh
Next.js middleware intercepts requests to protected routes, checks the access token expiry, and silently calls `POST /auth/refresh` when needed. The user is redirected to `/login` only if the refresh token is also expired or invalid.

### Route protection
All routes except `/login` and `/register` are protected by Next.js middleware. Unauthenticated users are redirected to `/login`. The logout action calls `POST /auth/logout`, clears the cookies, and redirects to `/login`.

### Dashboard — `/`
Summary view across all portfolios for the authenticated user: total portfolio value, total unrealised P&L, total number of holdings. Each portfolio is listed as a card with its individual value and P&L.

**Consumes:** DynamoDB read model via API.

### Portfolio detail — `/portfolios/[id]`
Holdings table showing ticker, asset name, quantity, average cost, current price, market value, and unrealised P&L per holding. Prices update in near-real time via polling against the DynamoDB read model. This is the primary page where the price feed's effect is visible.

**Consumes:** DynamoDB read model via API. Polls for price updates.

### Transaction history — `/portfolios/[id]/trades`
Paginated, chronological list of all trades for a portfolio. Columns: trade type (buy / sell / dividend), asset ticker, quantity, price, total value, date. Read from the Postgres event log.

**Consumes:** Postgres query API.

### Asset browser — `/assets`
Search and browse seeded assets by name or ticker. Each result shows asset type, exchange, and latest price from the read model. Used as the starting point when adding a new holding to a portfolio.

**Consumes:** Postgres API (asset search) + DynamoDB API (latest price).

### System status — `/status`
An embedded Grafana dashboard panel showing Kafka consumer lag, API request rate, and event throughput. Rendered as an iframe embed. Makes the observability stack visible without building a custom metrics UI.

**Consumes:** Grafana embed.

### Create portfolio (modal)
Form with name, description, and base currency fields. Accessible from the dashboard. Posts to the command API.

### Add holding (modal)
Asset search input (backed by the asset browser endpoint), quantity, and average cost fields. Validates that the selected asset exists before submitting. Accessible from the portfolio detail page.

### Record trade (modal)
Trade type selector (buy / sell / dividend), quantity, price, and date fields. Accessible from the portfolio detail page or transaction history. Submits to the command API and triggers a Kafka event downstream.

---

## Observability and alerting

### Health endpoints
Both the API and the Worker expose `/health/live` and `/health/ready` endpoints. These are scraped by Prometheus and wired into Kubernetes liveness and readiness probes.

**Validates:** Health check pattern, K8s probe integration, Prometheus scrape config.

### Metrics dashboard
A Grafana dashboard covering: Kafka event throughput, consumer group lag per topic, DynamoDB write latency, API request rate, API p95/p99 latency, and error rate by endpoint.

**Validates:** End-to-end observability pipeline — instrumentation → Prometheus → Grafana.

### Structured logging with correlation IDs
Serilog is configured across both the API and Worker with a shared correlation ID attached to each Kafka message header. A single trade can be traced from the HTTP request → Kafka publish → Worker consumption → DynamoDB write using this ID.

**Validates:** Distributed tracing across service boundaries without a full tracing backend.

### Consumer lag alert
A Prometheus alerting rule fires when the Kafka consumer group lag on the price ingestion topic exceeds a configurable threshold for more than a defined duration. The alert is routed through Alertmanager.

**Validates:** Alerting pipeline end-to-end (Prometheus rules → Alertmanager → notification).

---

## Infrastructure and developer experience

### Local environment bootstrap
A single `docker-compose up` starts the full local stack: Kafka, Zookeeper, Postgres, LocalStack (DynamoDB), Prometheus, and Grafana. Terraform provisions the LocalStack DynamoDB tables and Kafka topics declaratively.

**Validates:** Terraform with LocalStack, infrastructure-as-code for local dev, reproducible environment setup.

### Kubernetes manifests
Kubernetes Deployments, Services, and ConfigMaps are provided for both the API and Worker, targeting minikube. Liveness and readiness probes are wired to the health endpoints. Resource requests and limits are set.

**Validates:** K8s deployment patterns, probe configuration, ConfigMap-based environment injection.

### CI pipeline
GitHub Actions runs on every pull request: build → unit tests → integration tests → contract tests (Specmatic) → Docker image build. The pipeline fails fast if any stage fails.

**Validates:** CI/CD pipeline, contract testing integration, Docker build reproducibility.

---

## Out of scope

The following are intentionally excluded to keep the focus on architectural patterns:

| Excluded | Reason |
|---|---|
| OAuth / external identity provider (Keycloak, Auth0) | JWT via .NET Identity covers the auth pattern without the overhead of a separate identity service |
| Email verification / password reset | Adds email infrastructure with no architectural payoff for the patterns being validated |
| Historical price charts / time-series queries | Requires a time-series store (InfluxDB, TimescaleDB) outside the defined stack |
| Multi-currency conversion | Domain complexity irrelevant to the infrastructure patterns |
| Email / push notifications | Introduces another async service boundary not needed for this reference |
| Real market data API | External dependency introduces flakiness; simulator keeps focus on infra |
| Multi-user tenancy isolation | Per-user data scoping via JWT claims is sufficient; row-level security is out of scope |
| Admin UI for asset management | Assets are seeded; a full CRUD admin surface adds frontend scope without architectural value |