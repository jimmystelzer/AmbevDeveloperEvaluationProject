# Project Architecture

This document details the architecture of the Developer Evaluation project, explaining its layers, design patterns, and data flow.

## Table of Contents

- [Overview](#overview)
- [Architectural Principles](#architectural-principles)
- [Architecture Layers](#architecture-layers)
- [Design Patterns](#design-patterns)
- [Data Flow](#data-flow)
- [Component Diagram](#component-diagram)
- [Database Schema](#database-schema)

## Overview

The project follows a layered architecture based on the principles of Domain-Driven Design (DDD) and Clean Architecture, incorporating the CQRS (Command Query Responsibility Segregation) pattern. This approach promotes separation of responsibilities, facilitates testability, and allows greater flexibility and maintainability of the code.

## Architectural Principles

1. **Separation of Responsibilities**: Each component has a clear and well-defined function.
2. **Dependency Inversion**: High-level layers do not depend on low-level layers.
3. **Encapsulation**: Implementation details are encapsulated within their respective components.
4. **Testability**: The architecture facilitates unit and integration testing.
5. **Single Responsibility Principle**: Each class has only one reason to change.

## Architecture Layers

### 1. Domain Layer (Ambev.DeveloperEvaluation.Domain)

The domain layer contains business entities, domain rules, repository interfaces, and domain services.

**Main Components**:
- **Entities**: Sale, SaleItem, User
- **Enums**: SaleStatus
- **Events**: SaleCreatedEvent, SaleModifiedEvent, SaleCancelledEvent
- **Repositories (Interfaces)**: ISaleRepository, IUserRepository
- **Services**: SaleService, UserService
- **Specifications**: ActiveSaleSpecification, SaleItemQuantitySpecification
- **Discount Strategies**: NoDiscountStrategy, TenPercentDiscountStrategy, TwentyPercentDiscountStrategy

This layer is independent of any external implementation details and contains the core business logic.

### 2. Application Layer (Ambev.DeveloperEvaluation.Application)

The application layer coordinates the execution of domain tasks and directs the application's flow.

**Main Components**:
- **Commands**: CreateSaleCommand, UpdateSaleCommand, DeleteSaleCommand
- **Queries**: GetSaleQuery, GetSalesQuery
- **Handlers**: CreateSaleHandler, UpdateSaleHandler, DeleteSaleHandler
- **DTOs**: GetSaleResult, CreateSaleResult, UpdateSaleResult
- **Validation**: Validators for commands and queries

This layer implements the CQRS pattern, separating read (queries) and write (commands) operations.

### 3. Infrastructure Layer

#### 3.1. ORM (Ambev.DeveloperEvaluation.ORM)

Responsible for data access and repository implementations.

**Main Components**:
- **DbContext**: DefaultContext
- **Repository Implementations**: SaleRepository, UserRepository
- **Configurations**: Entity configurations for EF Core
- **Migrations**: Database migrations

#### 3.2. Common (Ambev.DeveloperEvaluation.Common)

Contains components and utilities shared among layers.

**Main Components**:
- **Logging**: Logging configurations
- **Security**: Authentication and authorization services
- **Validation**: Base classes for validation
- **Health Checks**: Application health checks

#### 3.3. IoC (Ambev.DeveloperEvaluation.IoC)

Manages dependency injection configuration.

**Main Components**:
- **DependencyInjection**: Registration of services and components

### 4. Presentation Layer (Ambev.DeveloperEvaluation.WebApi)

The presentation layer exposes the REST API and manages interaction with clients.

**Main Components**:
- **Controllers**: SalesController, UsersController, AuthController
- **Request/Response Models**: CreateSaleRequest, GetSaleResponse
- **Middleware**: ErrorHandling, Authentication
- **Mappings**: AutoMapper profiles

## Design Patterns

### 1. Domain-Driven Design (DDD)

The project uses DDD concepts such as:
- **Entities**: Objects with identity and lifecycle
- **Value Objects**: Objects without identity, defined by attributes
- **Aggregates**: Groups of entities treated as a unit
- **Repository Pattern**: Abstraction for data persistence
- **Domain Events**: Events that occur in the domain

### 2. Command Query Responsibility Segregation (CQRS)

The project separates read (queries) and write (commands) operations:
- **Commands**: Requests to change the system state
- **Queries**: Requests for information
- **Handlers**: Specific processors for each command or query

### 3. Mediator Pattern

Implemented through MediatR, to:
- Decouple senders and receivers
- Simplify communication between components
- Facilitate the processing of commands and events

### 4. Repository Pattern

Used to:
- Abstract data access
- Centralize query logic
- Facilitate unit testing

### 5. Specification Pattern

Used to:
- Encapsulate business rules
- Compose complex validations
- Reuse validation logic

### 6. Strategy Pattern

Implemented in discount strategies to:
- Encapsulate specific algorithms
- Allow behavior selection at runtime
- Facilitate the addition of new discount strategies

## Data Flow

### Example: Creating a Sale

1. **Client → WebApi**:
   - The client sends an HTTP POST request to `/api/sales`
   - The request body contains sale details (customer, branch, items)

2. **WebApi → Application**:
   - `SalesController` receives the request
   - The request is validated by `CreateSaleRequestValidator`
   - `AutoMapper` converts `CreateSaleRequest` to `CreateSaleCommand`
   - `MediatR` sends the command to the appropriate handler

3. **Application → Domain**:
   - `CreateSaleHandler` receives the command
   - Creates a new instance of `Sale` and `SaleItem`
   - Calls `SaleService.CreateSaleAsync()` to process the sale

4. **Domain → Infrastructure**:
   - `SaleService` applies business rules (discounts)
   - `SaleService` validates the sale through specifications
   - `SaleRepository` persists the sale in the database
   - `EventService` publishes the event `SaleCreatedEvent`

5. **Infrastructure → Application → WebApi → Client**:
   - The persisted sale returns to the handler
   - `AutoMapper` converts the result to `CreateSaleResult`
   - `CreateSaleResult` is converted to `CreateSaleResponse`
   - The controller returns HTTP 201 Created with the response

### Example: Querying Sales

1. **Client → WebApi**:
   - The client sends an HTTP GET request to `/api/sales?page=1&pageSize=10`

2. **WebApi → Application**:
   - `SalesController` receives the request
   - Creates a `GetSalesCommand` with pagination parameters
   - `MediatR` sends the command to the appropriate handler

3. **Application → Domain → Infrastructure**:
   - `GetSalesHandler` receives the command
   - Calls `SaleService.GetAllSalesAsync()` to retrieve sales
   - `SaleRepository` queries the database with pagination
   - The results are mapped to `GetSalesItemResult`

4. **Infrastructure → Application → WebApi → Client**:
   - The results return to the handler
   - `GetSalesHandler` creates a `GetSalesResult` object
   - `AutoMapper` converts to `GetSalesItemResponse`
   - The controller returns HTTP 200 OK with the paginated response


## Database Schema

### PostgreSQL (Relational)

#### Sales Table
```
+---------------+--------------+-------------------------------+
| Column        | Type         | Description                   |
+---------------+--------------+-------------------------------+
| Id            | UUID         | Primary Key                   |
| SaleNumber    | VARCHAR(20)  | Unique sale reference number  |
| Date          | TIMESTAMP    | Date and time of sale         |
| CustomerId    | UUID         | Reference to customer         |
| CustomerName  | VARCHAR(100) | Name of customer              |
| BranchId      | UUID         | Reference to branch           |
| BranchName    | VARCHAR(100) | Name of branch                |
| TotalAmount   | DECIMAL(18,2)| Total amount of sale          |
| Status        | INTEGER      | Sale status enum value        |
| CreatedAt     | TIMESTAMP    | Creation timestamp            |
| UpdatedAt     | TIMESTAMP    | Last update timestamp         |
+---------------+--------------+-------------------------------+
```

#### SaleItems Table
```
+---------------+--------------+-------------------------------+
| Column        | Type         | Description                   |
+---------------+--------------+-------------------------------+
| Id            | UUID         | Primary Key                   |
| SaleId        | UUID         | Foreign Key to Sales          |
| ProductId     | UUID         | Reference to product          |
| ProductName   | VARCHAR(100) | Name of product               |
| Quantity      | INTEGER      | Quantity of items             |
| UnitPrice     | DECIMAL(18,2)| Price per unit                |
| Discount      | DECIMAL(18,2)| Discount amount               |
| TotalAmount   | DECIMAL(18,2)| Total amount for this item    |
| CreatedAt     | TIMESTAMP    | Creation timestamp            |
| UpdatedAt     | TIMESTAMP    | Last update timestamp         |
+---------------+--------------+-------------------------------+
```

## Performance Considerations

2. **Pagination**:
   - Implemented in all list queries to avoid memory overload.
   - Sensible parameters for page limits (maximum of 100 items).

3. **Lazy Loading vs. Eager Loading**:
   - Explicit use of `Include()` for eager loading of necessary relationships.
   - Avoids the N+1 problem in list queries.

4. **Caching**:
   - Redis used for caching frequently accessed data.
   - Cache invalidation on modification events.
