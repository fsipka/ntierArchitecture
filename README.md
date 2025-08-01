# N-Tier Architecture Template for ASP.NET Core

A production-ready **N-Tier Architecture** solution template for building clean, modular, scalable **ASP.NET Core Web API** applications with **Entity Framework Core** and **JWT Authentication**.


---

## 🧩 Architecture Overview

- **API**: ASP.NET Core Web API layer (Controllers, Middleware)
- **Core**: Domain entities, DTOs, Interfaces
- **Repository**: EF Core, DbContext, Repository implementations
- **Service**: Business logic, Service implementations

Each layer is cleanly separated and connected via Dependency Injection.

---

## ⚙️ Installation

Install the required templates and CLI tool in one command:

```bash
dotnet new install FSipka.ModelTemplates::1.0.0 && dotnet new install FSipka.Templates::1.0.0 && dotnet tool install --global FSipka.CLI --version 1.1.3
```

---

## 🚀 Create a New Project

To scaffold a new N-Tier project, use:

```bash
fsipka create-new -n MyProjectName
```

This creates a solution with:

- `MyProject.API`
- `MyProject.Core`
- `MyProject.Repository`
- `MyProject.Service`

Everything is pre-configured for dependency injection, JWT auth, EF Core, and more.

---

## ➕ Add a New Entity Model

To generate a full entity including all layers:

```bash
fsipka add-model -n Product
```

This creates:

- Entity class in `Core/Entities`
- DTOs in `Core/DTOs`
- Interfaces in `Core/Interfaces`
- Service logic in `Service`
- Repository access in `Repository`
- Controller in `API`
- AutoMapper mapping
- FluentValidation validator (if needed)

---

## 🧱 Project Structure

```
/src
  /MyProject.API         → Web API (controllers, auth, middleware)
  /MyProject.Core        → Entities, DTOs, interfaces
  /MyProject.Repository  → DbContext, EF Core configs, Repositories
  /MyProject.Service     → Business logic, application services
/tests
  /MyProject.Tests       → Unit/integration tests (xUnit)
```

---

## 🛠 Entity Framework Core

The `Repository` project includes pre-configured support for **EF Core**.  
After generating your project or adding a model, you must create and apply a migration to update the database schema.

### Run EF Core Migration

```bash
cd src/MyProject.Repository
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> You must have the `dotnet-ef` tool installed:
```bash
dotnet tool install --global dotnet-ef
```

> Also make sure the connection string is correctly set in `appsettings.Development.json`.

---

## 🔐 Authentication

JWT-based authentication is already integrated.  
You only need to configure your settings in `appsettings.json`:

```json
"Jwt": {
  "Key": "YourSuperSecretKeyHere",
  "Issuer": "MyApp",
  "Audience": "MyAppUsers",
  "ExpiresInMinutes": 60
}
```

---

## 🧪 Testing

The solution includes a test project using **xUnit**, with support for mocking services and dependency injection.

---

## 🛒 NuGet Packages Used

| Package                     | Purpose                         |
|----------------------------|---------------------------------|
| `FSipka.Templates`         | Project scaffolding templates   |
| `FSipka.ModelTemplates`    | Model scaffolding templates     |
| `FSipka.CLI`               | CLI to create project/models    |
| `Microsoft.EntityFrameworkCore` | ORM layer (EF Core)       |
| `AutoMapper`               | Object mapping                  |
| `FluentValidation`         | Model validation                |
| `Swashbuckle.AspNetCore`  | Swagger/OpenAPI docs            |

---

## 💡 Quick Reference

| Task                            | Command                                         |
|---------------------------------|--------------------------------------------------|
| Create new project              | `fsipka create-new -n ProjectName`             |
| Add a new entity model          | `fsipka add-model -n Product`                  |
| Add EF Core migration           | `dotnet ef migrations add InitialCreate`       |
| Apply EF Core migration         | `dotnet ef database update`                    |

---

## 👨‍💻 Maintainer

Created and maintained by **Fatih Sipka**  
Contact: [f_sipka@hotmail.com](mailto:f_sipka@hotmail.com)

---

## 📄 License

This project is open-source and available under the [MIT License](LICENSE).


