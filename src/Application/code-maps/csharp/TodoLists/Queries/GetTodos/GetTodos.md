# 🗺️ Code Map: GetTodos

## 📁 File Information

**File Path:** `TodoLists/Queries/GetTodos/GetTodos.cs`
**File Size:** 1259 bytes
**Last Modified:** 2025-07-22T16:06:25.725Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoLists/Queries/GetTodos/GetTodos.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 4 |
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
| **GetTodosQueryHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    GetTodosQueryHandler[🔧 GetTodosQueryHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class GetTodosQueryHandler service
```

### Service Details

#### 🔧 GetTodosQueryHandler

**Namespace:** CleanArchitecture.Application.TodoLists.Queries.GetTodos
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces.IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |
| `IMapper` | `CleanArchitecture.Application.Common.Interfaces.IMapper` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 GetTodosQueryHandler.GetTodosQueryHandler

**Return Type**: `public`

**Parameters**:
- **context**: `IApplicationDbContext` → `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Interfaces/IApplicationDbContext.cs`
- **mapper**: `IMapper` → *File not found for type: IMapper*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    GetTodosQueryHandler[📦 GetTodosQueryHandler]
    class GetTodosQueryHandler publicClass
    IRequestHandlerGetTodosQuery -.->|inherits| GetTodosQueryHandler
    TodosVm -.->|inherits| GetTodosQueryHandler
    GetTodosQueryHandler -->|uses| IApplicationDbContext
    GetTodosQueryHandler -->|uses| IMapper

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
| **GetTodosQueryHandler** | IApplicationDbContext, IMapper, IRequestHandler<GetTodosQuery, TodosVm> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoLists.Queries.GetTodos

#### 🏗️ GetTodosQueryHandler 📦

**Line:** 11 | **Access:** public | **Extends:** IRequestHandler<GetTodosQuery, TodosVm>

**🔗 Dependencies:** IApplicationDbContext, IMapper, IRequestHandler<GetTodosQuery, TodosVm>

**🔧 Constructors:**
- `GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)` (Line 16)
  - **context**: `IApplicationDbContext`
  - **mapper**: `IMapper`

**📊 Fields:**
- `IApplicationDbContext _context` (Line 13) - private [readonly]
- `IMapper _mapper` (Line 14) - private [readonly]

