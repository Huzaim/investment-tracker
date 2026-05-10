# Implementation Roadmap

This document turns the feature spec into a step-by-step delivery order so we can work methodically and tick items off as they land.

## How to use this

- Treat this as the execution checklist for the repo.
- Only start the next phase when the current phase is complete enough to be useful.
- Mark items here as they ship, then mirror any major milestone changes in the feature docs and README.

## Phase 0 - Project foundation

- [x] Repository setup and documentation
- [x] Solution structure for API, Application, Domain, Infrastructure, and Tests

## Phase 1 - Asset registry foundation

- [x] Asset domain entity and validation
- [x] Asset type enum
- [x] EF Core DbContext and asset configuration
- [x] Initial asset registry migration
- [x] Asset seed data
- [x] Asset query service
- [x] Assets controller with search and lookup endpoints
- [ ] Asset detail endpoint with latest price enrichment

## Phase 2 - Authentication

- [ ] ASP.NET Core Identity setup
- [ ] User registration endpoint
- [ ] Login endpoint with JWT issuance
- [ ] Refresh token rotation
- [ ] Logout and token revocation
- [ ] JWT middleware and user scoping

## Phase 3 - Portfolio management

- [ ] Portfolio domain model and persistence
- [ ] Create and edit portfolio commands
- [ ] Add and remove holdings commands
- [ ] Portfolio snapshot read model
- [ ] Delete portfolio flow

## Phase 4 - Transaction tracking

- [ ] Trade domain model and append-only storage
- [ ] Record trade command
- [ ] Transaction history query endpoint
- [ ] Worker projection for trade events

## Phase 5 - Price and valuation

- [ ] Simulated price feed publisher
- [ ] Price update consumer and read-model projection
- [ ] Holding market value calculation
- [ ] Unrealised and realised P&L calculation

## Phase 6 - Frontend

- [ ] Login page
- [ ] Register page
- [ ] Route protection and token refresh
- [ ] Dashboard page
- [ ] Portfolio detail page
- [ ] Transaction history page
- [ ] Asset browser page
- [ ] Status page
- [ ] Create portfolio modal
- [ ] Add holding modal
- [ ] Record trade modal

## Phase 7 - Observability

- [ ] Health endpoints
- [ ] Metrics dashboard
- [ ] Structured logging with correlation IDs
- [ ] Consumer lag alert

## Phase 8 - Infrastructure and delivery

- [ ] Local environment bootstrap
- [ ] Kubernetes manifests
- [ ] Terraform with LocalStack
- [ ] GitHub Actions CI/CD
- [ ] Contract testing with Specmatic

## Current checkpoint

Completed so far:

- Project setup and documentation
- Asset registry foundation through the query and controller layer
- Asset seed data

Next best step:

1. Add the asset detail endpoint if we want to finish the asset registry slice before moving to auth.
2. Start the authentication phase once the asset registry slice is closed out.
