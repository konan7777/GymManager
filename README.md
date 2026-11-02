# GymManager – Backend API

A multi-tenant gym management system built with ASP.NET Core Web API, PostgreSQL, and Entity Framework Core.

GymManager provides gyms with a centralized platform for managing members, membership payments, renewals, and business statistics.

## Live Demo

- **Frontend:** https://gymmanager-frontend-a20a.onrender.com
- **Backend API:** https://gymmanager-b8eh.onrender.com
- **Frontend Repository:** https://github.com/konan7777/GymManager-Frontend

## Features

- JWT-based authentication and authorization
- Multi-tenant architecture with gym-level data isolation
- Gym and user registration
- Member management (CRUD operations)
- Member search
- Membership payment management
- Membership renewal with automatic date calculation
- Payment history
- Dashboard statistics
- Pagination
- Soft deletion for membership payments
- Centralized exception handling

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| .NET 10 | Application framework |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core | ORM and database access |
| PostgreSQL | Relational database |
| JWT | Authentication and authorization |
| BCrypt | Password hashing |
| Docker | Containerization |

## Architecture

The backend follows a structured architecture with separated responsibilities.

```text
GymManager/
│
├── GymManager.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Entities/
│   ├── Middleware/
│   ├── Migrations/
│   ├── Properties/
│   ├── Services/
│   ├── appsettings.json
│   └── Program.cs
│
├── Dockerfile
├── GymManager.slnx
└── README.md
```

### Main Components

- **Controllers** – Handle HTTP requests and API operations.
- **Entities** – Define the application's database models.
- **DTOs** – Separate API request and response models from database entities.
- **Data** – Contains the Entity Framework Core database context.
- **Services** – Handle application services such as JWT generation.
- **Middleware** – Provides centralized exception handling.
- **Migrations** – Track database schema changes.

## Multi-Tenant Architecture

GymManager uses a multi-tenant architecture.

Each gym has its own identifier (`GymId`), which is included as a claim in the authenticated user's JWT.

The API retrieves the gym identifier from the authenticated user's claims and uses it to scope data access.

This approach allows multiple gyms to use the same application while keeping their business data logically separated.

## Database Structure

The main database entities are:

- **Gym** – Represents a gym or fitness business.
- **User** – Stores user account information and authentication data.
- **Member** – Stores gym membership information.
- **MembershipPayment** – Stores membership payments and validity periods.

Entity Framework Core migrations are used to manage database schema changes.

## API Overview

The API is organized into the following main areas:

| Endpoint Group | Description |
|---|---|
| `/api/auth` | Authentication and registration |
| `/api/members` | Member management and search |
| `/api/membershippayments` | Membership payments and renewals |
| `/api/dashboard` | Dashboard statistics |

The API supports standard HTTP operations such as GET, POST, PUT, and DELETE where applicable.

## Authentication

The application uses JWT Bearer authentication.

After successful login, the API returns a token containing user identity and gym information.

Protected endpoints require the token in the HTTP Authorization header:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

Passwords are hashed using BCrypt rather than stored as plaintext.

## Configuration

The application uses configuration values for database connectivity and JWT authentication.

For local development, configure the following environment variables or provide the corresponding values through your local configuration:

```text
ConnectionStrings__DefaultConnection
Jwt__Key
Jwt__Issuer
Jwt__Audience
```

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_POSTGRESQL_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "GymManager",
    "Audience": "GymManagerUsers"
  }
}
```

**Important:** Never commit real database credentials, JWT keys, or other secrets to source control.

## Local Setup

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- Git

### 1. Clone the repository

```bash
git clone https://github.com/konan7777/GymManager.git
cd GymManager
```

### 2. Configure the application

Set the required database connection string and JWT configuration values using environment variables or local configuration.


### 3. Install Entity Framework Core CLI

If the EF Core CLI is not installed, run:

```bash
dotnet tool install --global dotnet-ef
```

### 4. Apply database migrations

```bash
dotnet ef database update --project GymManager.Api
```

### 5. Run the API

```bash
dotnet run --project GymManager.Api
```

The local API URLs are configured in `GymManager.Api/Properties/launchSettings.json`.

Swagger is enabled in the Development environment.

## Docker

The repository includes a multi-stage Dockerfile for building and running the API.

Build the image:

```bash
docker build -t gymmanager-api .
```

The container listens on port `10000`.

Database and JWT configuration must be provided through environment variables when running the container.

## Deployment

The backend is deployed using Render as a Docker Web Service.

PostgreSQL is hosted on Neon.

Production configuration and sensitive environment variables are managed through the hosting platform rather than committed to the repository.

## Project Status

**Backend MVP completed.**

The current implementation includes authentication, multi-tenant data access, member management, membership payments, renewals, and dashboard statistics.

## Related Project

[GymManager Frontend – React + TypeScript](https://github.com/konan7777/GymManager-Frontend)