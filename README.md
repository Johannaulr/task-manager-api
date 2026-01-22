# Task Manager API
A RESTful API built with ASP.NET Core that allows users to manage tasks, create custom tags, and filter tasks by tags. Includes unit tests (xUnit + Moq) and interactive API testing via Swagger UI (OpenAPI).

## Features
- CRUD operations for tasks
- User-defined tags (create, assign, and manage tags)
- Filter tasks by one or multiple tags
- RESTful API design
- Interactive Swagger/OpenAPI documentation
- Unit tests for services and controllers
- No authentication required
- Easy local setup


## Tech stack
- ASP.NET Core Web API
- Entity Framework Core + SQLite
- EF Core Tools for migrations
- Swagger / OpenAPI
- xUnit + Moq
- .NET SDK

## Local setup

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/en-us/download) (version 10.0 or higher)

### Clone the repository and run application
```
git clone https://github.com/Johannaulr/task-manager-api.git
cd task-manager-api
dotnet restore
dotnet run
```

The application will start locally on a URL shown in the console (usually `http://localhost:5069` or `https://localhost:7180`).

Once running, go to:
```
<BASE_URL>/swagger
```
Swagger UI allows you to:
- Explore available endpoints
- Create tasks and tags
- Assign tags to tasks
- Filter tasks using tags
- Execute requests directly from the browser

## Testing
The project includes unit tests covering:
- Service layer (business logic)
- Controller layer (API behavior)

### Run tests
1. Navigate to the test project:
```
cd ../
cd TaskManager.Api.Tests
```

2. Run tests
```
dotnet test
```

## Optional enhancements
- Add integration tests using WebApplicationFactory
- Dockerize the API for easy deployment
- Add sorting for tasks
