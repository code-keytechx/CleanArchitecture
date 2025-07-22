# 🗺️ Code Map: CreateTodoList

## 📁 File Information

**File Path:** `TodoLists/Commands/CreateTodoList/CreateTodoList.cs`
**File Size:** 850 bytes
**Last Modified:** 2025-07-22T16:06:25.725Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoLists/Commands/CreateTodoList/CreateTodoList.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 2 |
| 🔗 Dependencies | 1 |
| 📞 Method Calls | 0 |
| 👨‍👩‍👧‍👦 Inheritance | 2 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **CreateTodoListCommandHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    CreateTodoListCommandHandler[🔧 CreateTodoListCommandHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class CreateTodoListCommandHandler service
```

### Service Details

#### 🔧 CreateTodoListCommandHandler

**Namespace:** CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces.IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 CreateTodoListCommandHandler.CreateTodoListCommandHandler

**Return Type**: `public`

**Parameters**:
- **context**: `IApplicationDbContext` → `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Interfaces/IApplicationDbContext.cs`

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    CreateTodoListCommandHandler[📦 CreateTodoListCommandHandler]
    class CreateTodoListCommandHandler publicClass
    IRequestHandlerCreateTodoListCommand -.->|inherits| CreateTodoListCommandHandler
    int -.->|inherits| CreateTodoListCommandHandler
    CreateTodoListCommandHandler -->|uses| IApplicationDbContext

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
| **CreateTodoListCommandHandler** | IApplicationDbContext, IRequestHandler<CreateTodoListCommand, int> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoLists.Commands.CreateTodoList

#### 🏗️ CreateTodoListCommandHandler 📦

**Line:** 11 | **Access:** public | **Extends:** IRequestHandler<CreateTodoListCommand, int>

**🔗 Dependencies:** IApplicationDbContext, IRequestHandler<CreateTodoListCommand, int>

**🔧 Constructors:**
- `CreateTodoListCommandHandler(IApplicationDbContext context)` (Line 15)
  - **context**: `IApplicationDbContext`

**📊 Fields:**
- `IApplicationDbContext _context` (Line 13) - private [readonly]

