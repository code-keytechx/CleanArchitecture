# Project Analysis Report
Generated on: 8/7/2025, 1:18:56 AM
Project Path: /Users/quang.vuong/Documents/Development/sentra-web/sentra/cloned-repos/CleanArchitecture
Repository Root: /Users/quang.vuong/Documents/Development/sentra-web/sentra/cloned-repos/CleanArchitecture

## 📋 Project Overview
- **Name:** CleanArchitecture
- **Version:** unknown
- **Project Path:** /Users/quang.vuong/Documents/Development/sentra-web/sentra/cloned-repos/CleanArchitecture
- **Repository Root:** /Users/quang.vuong/Documents/Development/sentra-web/sentra/cloned-repos/CleanArchitecture
- **Primary Language:** C#
- **Total Files:** 355
- **Target Framework:** net9.0
- **SDK Version:** 9.0.100
- **Project Type:** Unknown
- **Output Type:** Unknown
- **Modern .NET:** Yes (Core/.NET 5+)

## 🔤 Languages Detected
- **C#:** 124 files
- **JSON:** 30 files
- **JavaScript:** 19 files
- **TypeScript:** 18 files
- **YAML:** 9 files
- **HTML:** 8 files
- **SCSS:** 3 files
- **CSS:** 2 files

## 📚 Dependencies
## 🛠️ Development Dependencies
## ⚙️ Configuration Files
- .gitignore
- README.md
- LICENSE

## 🧪 Testing Information
### Test Folders (7)
| Folder Name | Path | Type | Files |
|-------------|------|------|-------|
| testing | infra/core/testing | General Testing | 1 |
| tests | tests | General Testing | 0 |
| Application.FunctionalTests | tests/Application.FunctionalTests | General Testing | 14 |
| Application.UnitTests | tests/Application.UnitTests | Unit | 1 |
| Domain.UnitTests | tests/Domain.UnitTests | Unit | 1 |
| Infrastructure.IntegrationTests | tests/Infrastructure.IntegrationTests | E2E/Integration | 2 |
| Web.AcceptanceTests | tests/Web.AcceptanceTests | General Testing | 4 |

### Test Files Summary
- **Total Test Files:** 44

### Test Files by Directory
**src/Web/ClientApp/src/api-authorization** (1 files)
  - authorize.interceptor.spec.ts

**src/Web/ClientApp/src/app/counter** (1 files)
  - counter.component.spec.ts

**src/Web/ClientApp** (1 files)
  - tsconfig.spec.json

**src/Web/ClientApp-React/src** (1 files)
  - App.test.js

**tests/Application.FunctionalTests** (14 files)
  - Application.FunctionalTests.csproj
  - BaseTestFixture.cs
  - CustomWebApplicationFactory.cs
  - GlobalUsings.cs
  - ITestDatabase.cs
  - PostgreSQLTestDatabase.cs
  - PostgreSQLTestcontainersTestDatabase.cs
  - SqlTestDatabase.cs
  - SqlTestcontainersTestDatabase.cs
  - SqliteTestDatabase.cs
  - ... and 4 more files

**tests/Application.FunctionalTests/TodoItems/Commands** (4 files)
  - CreateTodoItemTests.cs
  - DeleteTodoItemTests.cs
  - UpdateTodoItemDetailTests.cs
  - UpdateTodoItemTests.cs

**tests/Application.FunctionalTests/TodoLists/Commands** (4 files)
  - CreateTodoListTests.cs
  - DeleteTodoListTests.cs
  - PurgeTodoListsTests.cs
  - UpdateTodoListTests.cs

**tests/Application.FunctionalTests/TodoLists/Queries** (1 files)
  - GetTodosTests.cs

**tests/Application.UnitTests** (1 files)
  - Application.UnitTests.csproj

**tests/Application.UnitTests/Common/Behaviours** (1 files)
  - RequestLoggerTests.cs

**tests/Application.UnitTests/Common/Exceptions** (1 files)
  - ValidationExceptionTests.cs

**tests/Application.UnitTests/Common/Mappings** (1 files)
  - MappingTests.cs

**tests/Domain.UnitTests** (1 files)
  - Domain.UnitTests.csproj

**tests/Domain.UnitTests/ValueObjects** (1 files)
  - ColourTests.cs

**tests/Infrastructure.IntegrationTests** (2 files)
  - GlobalUsings.cs
  - Infrastructure.IntegrationTests.csproj

**tests/Web.AcceptanceTests** (4 files)
  - ConfigurationHelper.cs
  - GlobalUsings.cs
  - Web.AcceptanceTests.csproj
  - appsettings.json

**tests/Web.AcceptanceTests/Features** (2 files)
  - Login.feature
  - Login.feature.cs

**tests/Web.AcceptanceTests/Pages** (2 files)
  - BasePage.cs
  - LoginPage.cs

**tests/Web.AcceptanceTests/StepDefinitions** (1 files)
  - LoginStepDefinitions.cs

## 🔷 .NET Project Information
### Framework & Project Details
- **Target Framework:** net9.0
- **Framework Version:** .NET 9.0
- **Project Type:** Unknown
- **Output Type:** Unknown
- **SDK Version:** 9.0.100
- **Modern .NET (Core/.NET 5+):** Yes

### Visual Studio Solution
- **Solution File Path:** CleanArchitecture.sln
- **Solution Format Version:** 12.00
- **Visual Studio Version:** 17.0.31903.59
- **Projects in Solution:** 14

#### Solution Projects
| Project Name | Type | Path |
|--------------|------|------|
| Domain | .NET Core Project | src\Domain\Domain.csproj |
| Application | .NET Core Project | src\Application\Application.csproj |
| Infrastructure | .NET Core Project | src\Infrastructure\Infrastructure.csproj |
| src | Solution Folder | src |
| tests | Solution Folder | tests |
| Application.UnitTests | .NET Core Project | tests\Application.UnitTests\Application.UnitTests.csproj |
| Domain.UnitTests | .NET Core Project | tests\Domain.UnitTests\Domain.UnitTests.csproj |
| Solution Items | Solution Folder | Solution Items |
| Web | .NET Core Project | src\Web\Web.csproj |
| Web.AcceptanceTests | .NET Core Project | tests\Web.AcceptanceTests\Web.AcceptanceTests.csproj |
| Application.FunctionalTests | .NET Core Project | tests\Application.FunctionalTests\Application.FunctionalTests.csproj |
| Infrastructure.IntegrationTests | .NET Core Project | tests\Infrastructure.IntegrationTests\Infrastructure.IntegrationTests.csproj |
| AppHost | C# Project | src\AppHost\AppHost.csproj |
| ServiceDefaults | C# Project | src\ServiceDefaults\ServiceDefaults.csproj |

### Central Package Version Management (CPVM)
- **Enabled:** Yes
- **Directory.Packages.props:** Directory.Packages.props
- **Centrally Managed Packages:** 53

#### Centrally Managed Package Versions
| Package Name | Version |
|--------------|---------|
| Aspire.Npgsql.EntityFrameworkCore.PostgreSQL | $(AspireVersion) |
| Aspire.Hosting.PostgreSQL | $(AspireVersion) |
| Aspire.Microsoft.EntityFrameworkCore.SqlServer | $(AspireVersion) |
| Aspire.Hosting.SqlServer | $(AspireVersion) |
| Aspire.Hosting.AppHost | $(AspireVersion) |
| Microsoft.Extensions.Http.Resilience | 9.0.0 |
| Microsoft.Extensions.ServiceDiscovery | 9.0.0 |
| OpenTelemetry.Exporter.OpenTelemetryProtocol | 1.10.0 |
| OpenTelemetry.Extensions.Hosting | 1.10.0 |
| OpenTelemetry.Instrumentation.AspNetCore | 1.9.0 |
| OpenTelemetry.Instrumentation.Http | 1.9.0 |
| OpenTelemetry.Instrumentation.Runtime | 1.9.0 |
| Ardalis.GuardClauses | 4.6.0 |
| AutoMapper | 13.0.1 |
| Azure.Extensions.AspNetCore.Configuration.Secrets | 1.3.2 |
| Azure.Identity | 1.13.1 |
| coverlet.collector | 6.0.2 |
| FluentAssertions | 6.12.2 |
| FluentValidation.AspNetCore | 11.3.0 |
| FluentValidation.DependencyInjectionExtensions | 11.11.0 |
| MediatR | 12.4.1 |
| Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore | $(AspnetVersion) |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | $(AspnetVersion) |
| Microsoft.AspNetCore.Mvc.Testing | $(AspnetVersion) |
| Microsoft.AspNetCore.OpenApi | $(AspnetVersion) |
| Microsoft.AspNetCore.Identity.UI | $(AspnetVersion) |
| Microsoft.AspNetCore.SpaProxy | $(AspnetVersion) |
| Microsoft.Data.SqlClient | 5.2.2 |
| Microsoft.EntityFrameworkCore | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.Design | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.Relational | $(EfcoreVersion) |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.1 |
| Testcontainers.PostgreSql | 4.0.0 |
| Microsoft.EntityFrameworkCore.Sqlite | $(EfcoreVersion) |
| Microsoft.EntityFrameworkCore.SqlServer | $(EfcoreVersion) |
| Testcontainers.MsSql | 4.0.0 |
| Microsoft.EntityFrameworkCore.Tools | $(EfcoreVersion) |
| Microsoft.Extensions.Configuration.Json | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Configuration.EnvironmentVariables | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | $(MicrosoftExtensionsVersion) |
| Microsoft.Extensions.Hosting | $(MicrosoftExtensionsVersion) |
| Microsoft.NET.Test.Sdk | 17.11.1 |
| Moq | 4.20.72 |
| NSwag.AspNetCore | 14.2.0 |
| NSwag.MSBuild | 14.2.0 |
| nunit | 3.14.0 |
| NUnit.Analyzers | 3.10.0 |
| NUnit3TestAdapter | 4.6.0 |
| Respawn | 6.2.1 |
| System.Configuration.ConfigurationManager | 9.0.0 |
| Microsoft.Playwright | 1.48.0 |
| SpecFlow.Plus.LivingDocPlugin | 3.9.57 |
| SpecFlow.NUnit | 3.9.74 |

### Project References (9)
| Reference Name | Path |
|----------------|------|
| ..\Domain\Domain | ..\Domain\Domain.csproj |
| ..\Application\Application | ..\Application\Application.csproj |
| ..\Application\Application | ..\Application\Application.csproj |
| ..\Infrastructure\Infrastructure | ..\Infrastructure\Infrastructure.csproj |
| ..\ServiceDefaults\ServiceDefaults | ..\ServiceDefaults\ServiceDefaults.csproj |
| ..\..\src\Web\Web | ..\..\src\Web\Web.csproj |
| ..\..\src\Application\Application | ..\..\src\Application\Application.csproj |
| ..\..\src\Infrastructure\Infrastructure | ..\..\src\Infrastructure\Infrastructure.csproj |
| ..\..\src\Domain\Domain | ..\..\src\Domain\Domain.csproj |

## 📊 Statistics
- **Total Files:** 355
- **Total Dependencies:** 0
- **Total Dev Dependencies:** 0
- **Total Test Folders:** 7
- **Total Test Files:** 44
- **Total Project References:** 9
- **Total Third-Party Libraries:** 0

---
*Report generated by Language Detector & Dependency Scanner v3*