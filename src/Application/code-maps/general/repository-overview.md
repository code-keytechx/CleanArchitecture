# 🗺️ Repository Code Map

> Comprehensive code map analysis for the entire repository

**Repository Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application`
**Generated on:** 2025-07-24T09:53:40.781Z

## 📋 Table of Contents

1. [Repository Overview](#repository-overview)
2. [Language Breakdown](#language-breakdown)
3. [Code Map Results](#code-map-results)
4. [Dependencies & Frameworks](#dependencies--frameworks)
5. [Project Structure](#project-structure)
6. [Testing Information](#testing-information)

## 🏗️ Repository Overview

**Primary Language:** C#
**Total Files:** 112
**Total Dependencies:** 0
**Test Files:** 0

## 🔤 Language Breakdown

| Language | File Count | Percentage |
|----------|------------|------------|
| C# | 43 | 91.5% |
| JSON | 4 | 8.5% |

## 📊 Code Map Results

### 🔷 C#

**Individual File Maps:** 40 files

*Individual files are organized in a folder structure matching the source code layout under:*
`code-maps/c/`


## 📦 Dependencies & Frameworks

## 🏗️ Project Structure

```
Application/
├── Common/
│   ├── Behaviours/
│   │   ├── AuthorizationBehaviour.cs
│   │   ├── LoggingBehaviour.cs
│   │   ├── PerformanceBehaviour.cs
│   │   ├── UnhandledExceptionBehaviour.cs
│   │   └── ValidationBehaviour.cs
│   ├── Exceptions/
│   │   ├── ForbiddenAccessException.cs
│   │   └── ValidationException.cs
│   ├── Interfaces/
│   │   ├── IApplicationDbContext.cs
│   │   ├── IIdentityService.cs
│   │   └── IUser.cs
│   ├── Mappings/
│   │   └── MappingExtensions.cs
│   ├── Models/
│   │   ├── LookupDto.cs
│   │   ├── PaginatedList.cs
│   │   └── Result.cs
│   └── Security/
│       └── AuthorizeAttribute.cs
├── DependencyInjection.cs
├── GlobalUsings.cs
├── TodoItems/
│   ├── Commands/
│   │   ├── CreateTodoItem/
│   │   │   ├── CreateTodoItem.cs
│   │   │   └── CreateTodoItemCommandValidator.cs
│   │   ├── DeleteTodoItem/
│   │   │   └── DeleteTodoItem.cs
│   │   ├── UpdateTodoItem/
│   │   │   ├── UpdateTodoItem.cs
│   │   │   └── UpdateTodoItemCommandValidator.cs
│   │   └── UpdateTodoItemDetail/
│   │       └── UpdateTodoItemDetail.cs
│   ├── EventHandlers/
│   │   ├── TodoItemCompletedEventHandler.cs
│   │   └── TodoItemCreatedEventHandler.cs
│   └── Queries/
│       └── GetTodoItemsWithPagination/
│           ├── GetTodoItemsWithPagination.cs
│           ├── GetTodoItemsWithPaginationQueryValidator.cs
│           └── TodoItemBriefDto.cs
├── TodoLists/
│   ├── Commands/
│   │   ├── CreateTodoList/
│   │   │   ├── CreateTodoList.cs
│   │   │   └── CreateTodoListCommandValidator.cs
│   │   ├── DeleteTodoList/
│   │   │   └── DeleteTodoList.cs
│   │   ├── PurgeTodoLists/
│   │   │   └── PurgeTodoLists.cs
│   │   └── UpdateTodoList/
│   │       ├── UpdateTodoList.cs
│   │       └── UpdateTodoListCommandValidator.cs
│   └── Queries/
│       └── GetTodos/
│           ├── GetTodos.cs
│           ├── TodoItemDto.cs
│           ├── TodoListDto.cs
│           └── TodosVm.cs
├── WeatherForecasts/
│   └── Queries/
│       └── GetWeatherForecasts/
│           ├── GetWeatherForecastsQuery.cs
│           └── WeatherForecast.cs
├── code-maps/
│   ├── README.md
│   ├── assets/
│   ├── csharp/
│   │   ├── Common/
│   │   │   ├── Behaviours/
│   │   │   ├── Exceptions/
│   │   │   ├── Interfaces/
│   │   │   ├── Mappings/
│   │   │   ├── Models/
│   │   │   └── Security/
│   │   ├── DependencyInjection.md
│   │   ├── GlobalUsings.md
│   │   ├── TodoItems/
│   │   │   ├── Commands/
│   │   │   ├── EventHandlers/
│   │   │   └── Queries/
│   │   ├── TodoLists/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   └── WeatherForecasts/
│   │       └── Queries/
│   ├── general/
│   │   ├── project-analysis.md
│   │   └── repository-overview.md
│   ├── java/
│   ├── javascript/
│   ├── python/
│   └── typescript/
└── project-analysis.md
```

## 🧪 Testing Information

**Test Folders:** 0
**Test Files:** 0

## 📋 Summary

This repository contains:
- **2** different programming languages
- **40** generated code maps
- **0** total dependencies
- **0** frameworks/libraries

---
*Generated by Repository Code Map Generator*