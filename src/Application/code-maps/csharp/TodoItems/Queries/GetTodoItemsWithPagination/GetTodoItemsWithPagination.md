# 🗺️ Code Map: GetTodoItemsWithPagination

## 📁 File Information

**File Path:** `TodoItems/Queries/GetTodoItemsWithPagination/GetTodoItemsWithPagination.cs`
**File Size:** 1310 bytes
**Last Modified:** 2025-07-22T16:06:25.724Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoItems/Queries/GetTodoItemsWithPagination/GetTodoItemsWithPagination.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 3 |
| 🔗 Dependencies | 2 |
| 📞 Method Calls | 0 |
| 👨‍👩‍👧‍👦 Inheritance | 2 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **GetTodoItemsWithPaginationQueryHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    GetTodoItemsWithPaginationQueryHandler[🔧 GetTodoItemsWithPaginationQueryHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class GetTodoItemsWithPaginationQueryHandler service
```

### Service Details

#### 🔧 GetTodoItemsWithPaginationQueryHandler

**Namespace:** CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces.IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |
| `IMapper` | `CleanArchitecture.Application.Common.Interfaces.IMapper` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 GetTodoItemsWithPaginationQueryHandler.GetTodoItemsWithPaginationQueryHandler

**Return Type**: `public`

**Parameters**:
- **context**: `IApplicationDbContext` → `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Interfaces/IApplicationDbContext.cs`
- **mapper**: `IMapper` → *File not found for type: IMapper*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    GetTodoItemsWithPaginationQueryHandler[📦 GetTodoItemsWithPaginationQueryHandler]
    class GetTodoItemsWithPaginationQueryHandler publicClass
    IRequestHandlerGetTodoItemsWithPaginationQuery -.->|inherits| GetTodoItemsWithPaginationQueryHandler
    PaginatedListTodoItemBriefDto -.->|inherits| GetTodoItemsWithPaginationQueryHandler
    GetTodoItemsWithPaginationQueryHandler -->|uses| IApplicationDbContext
    GetTodoItemsWithPaginationQueryHandler -->|uses| IMapper

    classDef publicClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef privateClass fill:#fce4ec,stroke:#880e4f,stroke-width:2px
    classDef interface fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef enum fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
```

## 🌳 Class Hierarchy

*No inheritance relationships found*

## 📋 Dependencies Matrix

| Class | Dependencies |
|-------|---------------|
| **GetTodoItemsWithPaginationQueryHandler** | IApplicationDbContext, IMapper, IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination

#### 🏗️ GetTodoItemsWithPaginationQueryHandler 📦

**Line:** 14 | **Access:** public | **Extends:** IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>

**🔗 Dependencies:** IApplicationDbContext, IMapper, IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>

**🔧 Constructors:**
- `GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)` (Line 19)
  - **context**: `IApplicationDbContext`
  - **mapper**: `IMapper`

**📊 Fields:**
- `IApplicationDbContext _context` (Line 16) - private [readonly]
- `IMapper _mapper` (Line 17) - private [readonly]

