# Investment Portfolio Tracker

A cloud-native, event-driven platform for tracking investment portfolios. Built to validate architectural patterns for high-throughput, observable systems.

> This project was built as a reference architecture using AI-assisted development (Claude Code/Codex).

## Architecture

```
┌──────────────┐       ┌──────────────────┐       ┌──────────────┐
│   Next.js    │──────▶│  .NET Core API   │──────▶│  PostgreSQL  │
│   Frontend   │  HTTP │  (Web API)       │       │              │
└──────────────┘       └───────┬──────────┘       └──────────────┘
                               │
                               │ Publish
                               ▼
                        ┌──────────────┐
                        │    Kafka     │
                        └──────┬───────┘
                               │ Consume
                               ▼
                        ┌──────────────────┐       ┌──────────────┐
                        │  Worker Service  │──────▶│  DynamoDB    │
                        │  (.NET)          │       │  (LocalStack)│
                        └──────────────────┘       └──────────────┘

Monitoring: Prometheus + Grafana
Infrastructure: Terraform → LocalStack (AWS)
Orchestration: Kubernetes (minikube)
CI/CD: GitHub Actions
```

## Tech Stack

- **Backend:** .NET 10, C#, Entity Framework Core, MediatR
- **Frontend:** Next.js, TypeScript, Tailwind CSS
- **Messaging:** Apache Kafka
- **Databases:** PostgreSQL, DynamoDB (via LocalStack)
- **Infrastructure:** Terraform, LocalStack, Docker, Kubernetes
- **Observability:** Prometheus, Grafana, Serilog
- **Testing:** xUnit, Specmatic (contract testing), integration tests
- **CI/CD:** GitHub Actions
- **AI-Assisted Development:** Claude Code, Codex , Cline , GitHub Copilot 

## Getting Started

### Prerequisites

- Docker & Docker Compose
- .NET 10 SDK
- Node.js 20+
- Terraform CLI

### Run Locally

```bash
git clone https://github.com/Huzaim/investment-tracker.git
cd investment-tracker
docker-compose up
```

The API will be available at `http://localhost:5006`, the Scalar API reference at `http://localhost:5006/scalar/v1`, and the frontend at `http://localhost:3000`.

## Project Structure

```
investment-tracker/
├── src/
│   ├── api/                  # .NET Core Web API
│   ├── web/                  # Next.js frontend
│   └── worker/               # Kafka consumer service
├── terraform/                # Infrastructure as Code
├── k8s/                      # Kubernetes manifests
├── monitoring/               # Prometheus & Grafana configs
├── adr/                      # Architecture Decision Records
└── .github/workflows/        # CI/CD pipelines
```

## Architecture Decision Records

Key decisions documented in the `adr/` folder:

## Status

🚧 Work in progress — building phase by phase.

- [x] Project setup and documentation
- [ ] .NET Core API with PostgreSQL
- [ ] Next.js frontend
- [ ] Kafka event-driven messaging
- [ ] Prometheus & Grafana observability
- [ ] Kubernetes deployment
- [ ] Terraform with LocalStack
- [ ] GitHub Actions CI/CD
- [ ] Contract testing with Specmatic
