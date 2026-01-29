# Online Examination System API

[![.NET 10](https://img.shields.io/badge/.NET-10.0-blueviolet?logo=dotnet)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/apis?view=aspnetcore-8.0&WT.mc_id=dotnet-35129-website)
[![Swagger](https://img.shields.io/badge/Swagger-API%20Docs-green?logo=swagger)](https://my-exam-system.runasp.net/swagger/index.html)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Abdelrahman%20Zagloul-blue?logo=linkedin)](https://www.linkedin.com/in/abdelrahman-zagloul/)

A **production-ready ASP.NET Core Web API** built with **Clean Architecture** and the **CQRS pattern**.  
Designed to power **secure and scalable online examinations**, with a strong focus on **performance, reliability, and maintainability** <br>
through background processing, Redis caching, robust health checks, and extensive automated testing.

---

## Quick Overview

- **Architecture:** Clean Architecture + CQRS (MediatR)
- **Authentication:** JWT + Refresh Tokens (HttpOnly cookies)
- **Caching:** Redis with automatic invalidation
- **Testing:** 317+ unit tests across API, Application, and Infrastructure
- **API Responses:** Standardized API response model across all endpoints
- **API Documentation:** Swagger with JWT authentication support

---

## Table of Contents

- [Features](#features)
  - [Authentication & Authorization](#authentication--authorization)
  - [Exams Management](#exams-management)
  - [Questions Management](#questions-management)
  - [Exam Results](#exam-results)
  - [Caching & Performance](#caching--performance)
  - [Background Jobs & Email](#background-jobs--email)
  - [Health Monitoring](#health-monitoring)
- [Architecture](#architecture)
- [Testing](#testing)
- [API & Platform Features](#api--platform-features)
- [Tech Stack](#tech-stack)
- [API Endpoints](#api-endpoints)
- [Setup Instructions](#setup-instructions)
- [Author](#author)

---

## Features

### Authentication & Authorization

- User registration, login, and logout
- Email confirmation with resend support
- Password management (forgot, reset, change)
- JWT authentication with refresh token support
- Secure refresh token rotation and revocation
- Role-based authorization (Doctor / Student)
- Secure HttpOnly cookie handling

### Exams Management

- Create, update, and delete exams (Doctor)
- Start and submit exams (Student)
- Exam session tracking per student
- Exam lifecycle validation (start/end constraints)
- Prevent duplicate submissions
- Pagination and status filtering

### Questions Management

- Create, update, and delete exam questions
- Retrieve questions by exam
- Retrieve a question by ID
- Validation for correct option integrity
- Pagination support

### Exam Results

- Asynchronous exam result calculation
- Exam results listing for doctors and students
- Detailed exam review (answers, correctness, score)
- Percentage and pass/fail evaluation
- Scheduled exam result publishing

### Caching & Performance

- Redis-based response caching for GET endpoints
- Attribute-based caching (`RedisCacheAttribute`)
- Automatic cache invalidation on write operations
- Prefix-based cache invalidation
- Fail-safe Redis behavior when Redis is unavailable
- Optimized query execution

### Background Jobs & Email

- Hangfire-based background job processing
- Asynchronous exam result calculation
- Scheduled exam result publishing
- HTML email notifications
- Idempotent background job execution

### Health Monitoring

- Custom health checks for:
  - Database
  - Redis
  - Hangfire
  - Core application services
- Liveness, readiness, and full health endpoints
- Graceful degradation on infrastructure failures

---

## Architecture

This project follows **Clean Architecture** principles with a feature-based structure that mirrors the actual repository layout.

```
ExamSystem.slnx
└── src
    ├── ExamSystem.API              # ASP.NET Core Web API (controllers, filters, middleware, attribute)
    ├── ExamSystem.Application      # Application layer (CQRS, handlers, validators, behaviors)
    ├── ExamSystem.Domain           # Domain layer (entities, enums, business rules)
    └── ExamSystem.Infrastructure   # Infrastructure layer (persistence, EF Core, Redis, Hangfire, email, identity)

└── tests
    ├── ExamSystem.Tests.API            # API controller unit tests
    ├── ExamSystem.Tests.Application    # Application layer unit tests
    └── ExamSystem.Tests.Infrastructure # Infrastructure layer unit tests
```

### Architecture Highlights

* Clean Architecture with strict separation of concerns
* Feature-based organization inside Application layer
* CQRS pattern implemented using MediatR
* No framework or HTTP dependencies in Domain layer
* Infrastructure concerns isolated behind abstractions
* Layer-specific dependency injection via extension methods
* Unified Result pattern for consistent API responses

---

## Testing

* xUnit-based unit testing
* Total test cases: **317**
* Coverage includes:
  * Application layer (handlers, validators, behaviors)
  * Infrastructure layer (repositories, services, jobs)
  * API controllers
* EF Core In-Memory provider for tests
* Background job and caching behavior tests
* Authentication and authorization scenarios

---

## API & Platform Features

* Standardized API response model
* API versioning (header, query, media type)
* Swagger UI with JWT authentication
* Global exception handling
* Rate limiting (sliding window)
* CORS configuration (development & production)
* Structured logging using Serilog

---

## Tech Stack

### Core & Architecture
- ASP.NET Core (.NET 10)
- Clean Architecture
- CQRS Pattern (MediatR)
- Result Pattern for unified success and error handling

### Data & Persistence
- Entity Framework Core
- Generic Repository & Unit of Work
- SQL Server
- EF Core In-Memory Provider (Testing)

### Authentication & Security
- JWT Authentication
- Refresh Tokens with rotation & revocation
- HttpOnly & Secure Cookies
- Role-Based Authorization (Doctor / Student)

### Caching & Performance
- Redis
- Attribute-based response caching
- Automatic cache invalidation
- Prefix-based cache invalidation
- Fail-safe caching when Redis is unavailable

### Background Processing & Scheduling
- Hangfire
- Background jobs for exam result calculation
- Scheduled exam result publishing
- Idempotent background job execution

### API & Platform
- Unified API Response model for all endpoints
- Centralized ApiResponse factory
- API Versioning (Header, Query, Media Type)
- Swagger / OpenAPI with JWT support
- Global exception handling
- Sliding window rate limiting
- CORS (Development & Production)

### Health & Monitoring
- Custom Health Checks
- Liveness, Readiness, and Health endpoints
- Health checks for Database, Redis, Hangfire, and core services

### Validation & Mapping
- FluentValidation
- Centralized validation behaviors
- Custom validation extensions
- AutoMapper for auto mapping 

### Email & Notifications
- SMTP Email Service
- HTML Email Templates
- Background email sending
- Exam result notification emails

### Logging & Diagnostics
- Serilog
- Structured logging
- Request/response logging via MediatR behaviors

### Testing
- xUnit
- Unit testing for API, Application, and Infrastructure layers
- Authentication & authorization test coverage
- Background job and caching behavior tests

---

## API Endpoints

**Base URL:**
`https://my-exam-system.runasp.net`

**Swagger Specification:**
`https://my-exam-system.runasp.net/swagger/v1/swagger.json`

---

### Authentication

| Method | Endpoint                              | Description               |
| ------ | ------------------------------------- | ------------------------- |
| POST   | `/api/auth/login`                     | User login                |
| POST   | `/api/auth/register`                  | User registration         |
| POST   | `/api/auth/logout`                    | User logout               |
| GET    | `/api/auth/confirm-email`             | Confirm email             |
| POST   | `/api/auth/email/resend-confirmation` | Resend confirmation email |
| POST   | `/api/auth/token/refresh`             | Refresh access token      |
| POST   | `/api/auth/token/revoke`              | Revoke refresh token      |
| POST   | `/api/auth/password/change`           | Change password           |
| POST   | `/api/auth/password/forgot`           | Forgot password           |
| POST   | `/api/auth/password/reset`            | Reset password            |

---

### Exams

| Method | Endpoint                     | Description           |
| ------ | ---------------------------- | --------------------- |
| POST   | `/api/exams`                 | Create exam           |
| GET    | `/api/exams/{examId}`        | Get exam by ID        |
| PUT    | `/api/exams/{examId}`        | Update exam           |
| DELETE | `/api/exams/{examId}`        | Delete exam           |
| GET    | `/api/exams/doctor`          | Get exams for doctor  |
| GET    | `/api/exams/student`         | Get exams for student |
| POST   | `/api/exams/{examId}/start`  | Start exam            |
| POST   | `/api/exams/{examId}/submit` | Submit exam           |

---

### Questions

| Method | Endpoint                                     | Description          |
| ------ | -------------------------------------------- | -------------------- |
| GET    | `/api/exams/{examId}/questions`              | Get exam questions   |
| POST   | `/api/exams/{examId}/questions`              | Create exam question |
| GET    | `/api/exams/{examId}/questions/{questionId}` | Get question by ID   |
| PUT    | `/api/exams/{examId}/questions/{questionId}` | Update question      |
| DELETE | `/api/exams/{examId}/questions/{questionId}` | Delete question      |

---

### Exam Results

| Method | Endpoint                      | Description                      |
| ------ | ----------------------------- | -------------------------------- |
| GET    | `/api/exams/{examId}/results` | Get exam results for doctor      |
| GET    | `/api/exams/{examId}/review`  | Get exam review                  |
| GET    | `/api/results/me`             | Get current student exam results |

---

##  Setup Instructions

### 1️ Clone the Repository

```bash
git clone https://github.com/Abdelrahman-Zagloul/ExamSystem-API.git
cd ExamSystem-API
```

---

### 2️ Create `appsettings.json`

Create a new `appsettings.json` file in the API project root and add the following configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "<SQL_SERVER_CONNECTION_STRING>",
    "RedisConnection": "<REDIS_CONNECTION_STRING>"
  },

  "DefaultUsersSettings": {
    "AdminEmail": "admin@example.com",
    "AdminPassword": "StrongAdminPassword!",
    "DoctorEmail": "doctor@example.com",
    "DoctorPassword": "StrongDoctorPassword!"
  },

  "JWTSettings": {
    "SecretKey": "<JWT_SECRET_KEY>",
    "Issuer": "ExamSystem.API",
    "Audience": "ExamSystem.Client",
    "DurationInMinutes": 60
  },

  "MailSettings": {
    "Email": "test@example.com",
    "DisplayName": "Exam System",
    "Password": "<EMAIL_PASSWORD>",
    "Host": "smtp.gmail.com",
    "Port": 587
  },

  "FrontendUrlsSettings": {
    "BaseURL": "http://localhost:4200",
    "ConfirmEmailPath": "/auth/confirm-email",
    "ResetPasswordPath": "/auth/reset-password",
    "ReviewExamResultPath": "/api/exams/{id}/review"
  },

  "CorsSettings": {
    "AllowedOrigins": [ "http://localhost:4200" ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE" ],
    "AllowedHeaders": [
      "Content-Type",
      "Authorization",
      "Accept-Language",
      "Api-Version"
    ],
    "AllowCredentials": true
  },

  "RefreshTokenSettings": {
    "RefreshTokenLifetimeDays": 10,
    "RefreshTokenHashKey": "<REFRESH_TOKEN_HASH_KEY>"
  }
}
```

---

### 3️ Apply Database Migrations

```bash
dotnet ef database update
```

---

### 4 Run the Application

```bash
dotnet run
```

The API will start and listen on the configured ports.

---

## Testing

* Unit Tests: **xUnit**
* Mocking supported via dependency injection

Run tests using:

```bash
dotnet test
```

## API Documentation

Swagger UI is available at:

```
https://my-exam-system.runasp.net/swagger/index.html
```

You can explore and test all API endpoints directly from the browser.

---

## Author
**Abdelrahman Zagloul**  
Software Engineer | Back End .NET Developer 

<p align="center">
  <a href="mailto:abdelrahman.zagloul.dev@gmail.com">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=gmail" alt="Gmail"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://github.com/Abdelrahman-Zagloul">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=github" alt="GitHub"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://www.linkedin.com/in/abdelrahman-zagloul/">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=linkedin" alt="LinkedIn"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://wa.me/0201285168885">
    <img height="53" src="https://upload.wikimedia.org/wikipedia/commons/6/6b/WhatsApp.svg" alt="WhatsApp"/>
  </a>
  &nbsp;&nbsp;
  <a href="https://www.facebook.com/bdalrhmnzghlwl.291648">
    <img height="75" src="https://go-skill-icons.vercel.app/api/icons?i=facebook" alt="Facebook"/>
  </a>
</p>

