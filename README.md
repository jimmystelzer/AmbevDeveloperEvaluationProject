# Ambev Developer Evaluation Project

This project implements a sales management API following the principles of Domain-Driven Design (DDD) with .NET 8.0.

## Table of Contents

- [Used Technologies](#used-technologies)
- [Architecture](#architecture)
- [Business Rules](#business-rules)
- [How to Run](#how-to-run)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Tests](#tests)
- [Docker](#docker)

## Used Technologies

- **Backend**: .NET 8.0
- **Databases**:
  - PostgreSQL (Relational)
  - MongoDB (NoSQL)
  - Redis (Cache)
- **Frameworks**:
  - Entity Framework Core 8.0
  - MediatR
  - AutoMapper
  - FluentValidation
- **DevOps**:
  - Docker
  - Docker Compose
- **Testing**:
  - xUnit
  - NSubstitute

## Architecture

The project follows a layered architecture based on Domain-Driven Design (DDD) with CQRS (Command Query Responsibility Segregation):

1. **Domain**:
   - Business entities (Sale, SaleItem)
   - Repository interfaces
   - Domain events
   - Business rules
   - Domain services

2. **Application**:
   - Commands and queries (CQRS)
   - Handlers for processing commands
   - Response DTOs 

3. **Infrastructure**:
   - Repository implementations
   - Entity Framework configurations 
   - Implementations of external services

4. **WebApi**:
   - Controllers
   - Request/response DTOs
   - API configurations
   - Middleware

## Business Rules

The system implements the following rules for sales:

1. **Quantity Discounts**:
   - 4 or more identical items: 10% discount
   - 10 to 20 identical items: 20% discount

2. **Restrictions**:
   - It is not possible to sell more than 20 identical items
   - Purchases with fewer than 4 items do not receive a discount

3. **Events**:
   - SaleCreated: when a new sale is created
   - SaleModified: when a sale is updated
   - SaleCancelled: when a sale is canceled
   - ItemCancelled: when an item is removed from the sale

## How to Run

### Prerequisites

- Docker and Docker Compose
- .NET 8.0 SDK (for local development)

### First Run

1. Clone the repository:
   ```bash
   git clone https://github.com/renankcb/SalesCRUD.git
   cd ambev-developer-evaluation
   ```

2. Run with Docker Compose:
   ```bash
   docker-compose up -d
   ```

3. Access the application:
   - API: http://localhost:5119
   - Swagger: http://localhost:5119/swagger

### Local Development

To run the project locally:

1. Restore packages:
   ```bash
   dotnet restore
   ```

2. Apply database migrations:
   ```bash
   dotnet ef database update --project src/Ambev.DeveloperEvaluation.ORM
   ```

3. Run the application:
   ```bash
   dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
   ```

## API Endpoints

### Sales

- **GET /api/sales**
  - Lists all sales (paginated)
  - Parameters: page, pageSize
  - Response: Paginated list of sales

- **GET /api/sales/{id}**
  - Retrieves details of a specific sale
  - Response: Details of the sale with items

- **POST /api/sales**
  - Creates a new sale
  - Body: Sale data and items
  - Response: Created sale with applied discount

- **PUT /api/sales/{id}**
  - Updates an existing sale
  - Body: Updated sale data
  - Response: Updated sale

- **DELETE /api/sales/{id}**
  - Cancels a sale (soft delete)
  - Response: Confirmation of cancellation

## Project Structure

```
/
├── doc/                                          # Documentação do projeto
├── src/
│   ├── Ambev.DeveloperEvaluation.Application/    # Camada de aplicação (CQRS)
│   ├── Ambev.DeveloperEvaluation.Common/         # Bibliotecas compartilhadas
│   ├── Ambev.DeveloperEvaluation.Domain/         # Camada de domínio
│   ├── Ambev.DeveloperEvaluation.IoC/            # Injeção de dependência
│   ├── Ambev.DeveloperEvaluation.ORM/            # Acesso a dados (EF Core)
│   └── Ambev.DeveloperEvaluation.WebApi/         # API REST
├── tests/
│   ├── Ambev.DeveloperEvaluation.Functional/      # Testes funcionais
│   ├── Ambev.DeveloperEvaluation.Integration/    # Testes de integração
│   └── Ambev.DeveloperEvaluation.Unit/           # Testes unitários
├── docker-compose.yml      # Configuração Docker Compose
└── README.md               # Este arquivo
```

## Tests

The project contains unit, integration, and functional tests:

### Running Tests

```bash
# Run all tests
dotnet test

# Run unit tests
dotnet test tests/Ambev.DeveloperEvaluation.Unit

# Run integration tests
dotnet test tests/Ambev.DeveloperEvaluation.Integration

# Run functional tests
dotnet test tests/Ambev.DeveloperEvaluation.Functional
```

## Docker

The project is configured for execution in Docker containers:

- **WebApi**: Main application (.NET 8.0)
- **Database**: PostgreSQL 13
- **NoSQL**: MongoDB 8.0
- **Cache**: Redis 7.4.1

### Ports

- WebApi: 5119
- PostgreSQL: 5433
- MongoDB: 27017 (internal)
- Redis: 6379 (internal)

### Recreating Containers

To recreate the containers (e.g., after changes):

```bash
docker-compose down
docker-compose build
docker-compose up -d
```

## Docs

See [General Architecture](/.doc/architecture-documentation.md)
See [Improvements](/.doc/improvement.md)