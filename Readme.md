# Lerevently 🎟️

> **A Modular Monolith Experiment for Event Management**

Lerevently is an open-source Events Management System designed to explore and demonstrate the **Modular Monolith**
architectural pattern. Built toward future extraction into microservices.

## 🏗️ Architecture

This project is structured around the Modular Monolith architecture, where each module follows clean architecture.

### Key Architectural Concepts:

* **Modular Boundaries:** Each module (e.g., `Events`, `Users`) is self-contained with its own database schema, domain
  logic, and infrastructure.
* **Encapsulation:** Modules expose a public API (Contracts) but hide their internal implementation details.
  Cross-module communication is strictly handled via integration events.
* **Data Isolation:** Although deployed as a monolith, modules enforce logical data separation (e.g., separate database
  schemas) to prevent tight coupling at the data layer.
* **Single Deployment Unit:** The entire application runs as a single process (API Host), simplifying deployment and
  observability.

## 🧩 Modules (Planned/Implemented)

* **Events Module:** Managing event creation, scheduling, and details.
* **Ticketing Module:** Handling inventory, ticket types, and reservations.
* **Identity Module:** User authentication and profile management.
* **Payments Module:** Processing transactions (Mock/Integration).

## 🛠️ Tech Stack

* **Language:** C# (.NET 10)
* **Communication:** In-memory integration events (MassTransit)
* **Database:** PostgreSQL (via Docker)
* **Testing:** TUnit (tests project are executables)

## 🚀 Getting Started

### Prerequisites

* .NET SDK (10.0.x)
* Docker Desktop

### Running via Docker

All modules are configured to run together in a single Docker container. Identity provider, Database and Seq are
included.
Migrations are applied on startup, data seeder is commented out but ready to use.

```bash
docker-compose up -d