# 🗺️ Repository Code Map

> Comprehensive code map analysis for the entire repository

**Repository Path:** `/Users/duythai/KeyTechX/Sentra/sentra-web/sentra/cloned-repos/CleanArchitecture`
**Generated on:** 2025-08-09T00:39:13.001Z

## 📋 Table of Contents

1. [Repository Overview](#repository-overview)
2. [Language Breakdown](#language-breakdown)
3. [Code Map Results](#code-map-results)
4. [Dependencies & Frameworks](#dependencies--frameworks)
5. [Project Structure](#project-structure)
6. [Testing Information](#testing-information)

## 🏗️ Repository Overview

**Primary Language:** C#
**Total Files:** 355
**Total Dependencies:** 0
**Test Files:** 44

## 🔤 Language Breakdown

| Language | File Count | Percentage |
|----------|------------|------------|
| C# | 124 | 58.2% |
| JSON | 30 | 14.1% |
| JavaScript | 19 | 8.9% |
| TypeScript | 18 | 8.5% |
| YAML | 9 | 4.2% |
| HTML | 8 | 3.8% |
| SCSS | 3 | 1.4% |
| CSS | 2 | 0.9% |

## 📊 Code Map Results

### ⚛️ JavaScript/TypeScript

**Project-level Maps:**
- [Full Repository](../typescript/dependency-map.md) - All JS/TS files files
- [Full Repository](../typescript/dependency-map.md) - All JS/TS files files


## 📦 Dependencies & Frameworks

## 🏗️ Project Structure

```
CleanArchitecture/
├── .azdo/
│   └── pipelines/
│       └── azure-dev.yml
├── .devcontainer/
│   └── devcontainer.json
├── .github/
│   ├── FUNDING.yml
│   ├── ISSUE_TEMPLATE/
│   │   ├── bug_report.md
│   │   ├── config.yml
│   │   └── feature_request.md
│   └── workflows/
│       ├── azure-dev.yml
│       ├── build.yml
│       ├── codeql.yml
│       ├── jekyll-gh-pages.yml
│       └── package.yml
├── .template.config/
│   ├── dotnetcli.host.json
│   ├── ide.host.json
│   └── template.json
├── CODE_OF_CONDUCT.md
├── LICENSE
├── README-template.md
├── README.md
├── azure.yaml
├── code-maps/
│   ├── assets/
│   ├── csharp/
│   ├── general/
│   ├── java/
│   ├── javascript/
│   ├── python/
│   └── typescript/
│       └── dependency-map.md
├── global.json
├── infra/
│   ├── abbreviations.json
│   ├── core/
│   │   ├── ai/
│   │   ├── config/
│   │   ├── database/
│   │   │   ├── cosmos/
│   │   │   ├── mysql/
│   │   │   ├── postgresql/
│   │   │   └── sqlserver/
│   │   ├── gateway/
│   │   ├── host/
│   │   ├── monitor/
│   │   ├── networking/
│   │   ├── search/
│   │   ├── security/
│   │   ├── storage/
│   │   └── testing/
│   ├── main.parameters.json
│   └── services/
├── project-analysis.md
├── src/
│   ├── AppHost/
│   │   ├── Program.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── appsettings.Development.json
│   │   └── appsettings.json
│   ├── Application/
│   │   ├── Common/
│   │   │   ├── Behaviours/
│   │   │   ├── Exceptions/
│   │   │   ├── Interfaces/
│   │   │   ├── Mappings/
│   │   │   ├── Models/
│   │   │   └── Security/
│   │   ├── DependencyInjection.cs
│   │   ├── GlobalUsings.cs
│   │   ├── TodoItems/
│   │   │   ├── Commands/
│   │   │   ├── EventHandlers/
│   │   │   └── Queries/
│   │   ├── TodoLists/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   ├── WeatherForecasts/
│   │   │   └── Queries/
│   │   └── code-maps/
│   │       ├── README.md
│   │       ├── csharp/
│   │       └── general/
│   ├── Domain/
│   │   ├── Common/
│   │   │   ├── BaseAuditableEntity.cs
│   │   │   ├── BaseEntity.cs
│   │   │   ├── BaseEvent.cs
│   │   │   └── ValueObject.cs
│   │   ├── Constants/
│   │   │   ├── Policies.cs
│   │   │   └── Roles.cs
│   │   ├── Entities/
│   │   │   ├── TodoItem.cs
│   │   │   └── TodoList.cs
│   │   ├── Enums/
│   │   │   └── PriorityLevel.cs
│   │   ├── Events/
│   │   │   ├── TodoItemCompletedEvent.cs
│   │   │   ├── TodoItemCreatedEvent.cs
│   │   │   └── TodoItemDeletedEvent.cs
│   │   ├── Exceptions/
│   │   │   └── UnsupportedColourException.cs
│   │   ├── GlobalUsings.cs
│   │   └── ValueObjects/
│   │       └── Colour.cs
│   ├── Infrastructure/
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── ApplicationDbContextInitialiser.cs
│   │   │   ├── Configurations/
│   │   │   ├── Interceptors/
│   │   │   ├── Migrations/
│   │   │   ├── PostgreSQL/
│   │   │   └── SQLite/
│   │   ├── DependencyInjection.cs
│   │   ├── GlobalUsings.cs
│   │   ├── Identity/
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── IdentityResultExtensions.cs
│   │   │   └── IdentityService.cs
│   ├── ServiceDefaults/
│   │   ├── Extensions.cs
│   └── Web/
│       ├── ClientApp/
│       │   ├── README.md
│       │   ├── angular.json
│       │   ├── aspnetcore-https.js
│       │   ├── karma.conf.js
│       │   ├── package-lock.json
│       │   ├── package.json
│       │   ├── proxy.conf.js
│       │   ├── src/
│       │   ├── tsconfig.app.json
│       │   ├── tsconfig.json
│       │   └── tsconfig.spec.json
│       ├── ClientApp-React/
│       │   ├── README.md
│       │   ├── aspnetcore-https.js
│       │   ├── aspnetcore-react.js
│       │   ├── package-lock.json
│       │   ├── package.json
│       │   ├── public/
│       │   └── src/
│       ├── DependencyInjection.cs
│       ├── Endpoints/
│       │   ├── TodoItems.cs
│       │   ├── TodoLists.cs
│       │   ├── Users.cs
│       │   └── WeatherForecasts.cs
│       ├── GlobalUsings.cs
│       ├── Infrastructure/
│       │   ├── CustomExceptionHandler.cs
│       │   ├── EndpointGroupBase.cs
│       │   ├── IEndpointRouteBuilderExtensions.cs
│       │   ├── MethodInfoExtensions.cs
│       │   └── WebApplicationExtensions.cs
│       ├── Pages/
│       │   ├── Error.cshtml.cs
│       │   ├── Shared/
│       ├── Program.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Services/
│       │   └── CurrentUser.cs
│       ├── Templates/
│       ├── appsettings.Development.json
│       ├── appsettings.PostgreSQL.json
│       ├── appsettings.SQLite.json
│       ├── appsettings.json
│       └── wwwroot/
│           ├── api/
├── templates/
│   └── ca-use-case/
│       ├── .template.config/
│       │   ├── dotnetcli.host.json
│       │   └── template.json
│       └── FeatureName/
│           ├── Commands/
│           └── Queries/
└── tests/
    ├── Application.FunctionalTests/
    │   ├── BaseTestFixture.cs
    │   ├── CustomWebApplicationFactory.cs
    │   ├── GlobalUsings.cs
    │   ├── ITestDatabase.cs
    │   ├── PostgreSQLTestDatabase.cs
    │   ├── PostgreSQLTestcontainersTestDatabase.cs
    │   ├── SqlTestDatabase.cs
    │   ├── SqlTestcontainersTestDatabase.cs
    │   ├── SqliteTestDatabase.cs
    │   ├── TestDatabaseFactory.cs
    │   ├── Testing.cs
    │   ├── TodoItems/
    │   │   └── Commands/
    │   ├── TodoLists/
    │   │   ├── Commands/
    │   │   └── Queries/
    │   ├── appsettings.PostgreSQL.json
    │   └── appsettings.json
    ├── Application.UnitTests/
    │   └── Common/
    │       ├── Behaviours/
    │       ├── Exceptions/
    │       └── Mappings/
    ├── Domain.UnitTests/
    │   └── ValueObjects/
    │       └── ColourTests.cs
    ├── Infrastructure.IntegrationTests/
    │   ├── GlobalUsings.cs
    └── Web.AcceptanceTests/
        ├── ConfigurationHelper.cs
        ├── Features/
        │   └── Login.feature.cs
        ├── GlobalUsings.cs
        ├── Pages/
        │   ├── BasePage.cs
        │   └── LoginPage.cs
        ├── StepDefinitions/
        │   └── LoginStepDefinitions.cs
        └── appsettings.json
```

## 🧪 Testing Information

**Test Folders:** 7
**Test Files:** 44

## 📋 Summary

This repository contains:
- **8** different programming languages
- **2** generated code maps
- **0** total dependencies
- **0** frameworks/libraries

---
*Generated by Repository Code Map Generator*