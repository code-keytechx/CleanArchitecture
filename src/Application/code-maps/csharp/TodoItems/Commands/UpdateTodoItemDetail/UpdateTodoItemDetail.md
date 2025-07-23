# 🗺️ Code Map: UpdateTodoItemDetail

## 📁 File Information

**File Path:** `TodoItems/Commands/UpdateTodoItemDetail/UpdateTodoItemDetail.cs`
**File Size:** 1137 bytes
**Last Modified:** 2025-07-22T16:06:25.724Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoItems/Commands/UpdateTodoItemDetail/UpdateTodoItemDetail.cs`

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
| 👨‍👩‍👧‍👦 Inheritance | 1 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **UpdateTodoItemDetailCommandHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    UpdateTodoItemDetailCommandHandler[🔧 UpdateTodoItemDetailCommandHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class UpdateTodoItemDetailCommandHandler service
```

### Service Details

#### 🔧 UpdateTodoItemDetailCommandHandler

**Namespace:** CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces.IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 UpdateTodoItemDetailCommandHandler.UpdateTodoItemDetailCommandHandler

**Return Type**: `public`

**Parameters**:
- **context**: `IApplicationDbContext` → `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Interfaces/IApplicationDbContext.cs`

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    UpdateTodoItemDetailCommandHandler[📦 UpdateTodoItemDetailCommandHandler]
    class UpdateTodoItemDetailCommandHandler publicClass
    IRequestHandlerUpdateTodoItemDetailCommand -.->|inherits| UpdateTodoItemDetailCommandHandler
    UpdateTodoItemDetailCommandHandler -->|uses| IApplicationDbContext

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
| **UpdateTodoItemDetailCommandHandler** | IApplicationDbContext, IRequestHandler<UpdateTodoItemDetailCommand> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail

#### 🏗️ UpdateTodoItemDetailCommandHandler 📦

**Line:** 17 | **Access:** public | **Extends:** IRequestHandler<UpdateTodoItemDetailCommand>

**🔗 Dependencies:** IApplicationDbContext, IRequestHandler<UpdateTodoItemDetailCommand>

**🔧 Constructors:**
- `UpdateTodoItemDetailCommandHandler(IApplicationDbContext context)` (Line 21)
  - **context**: `IApplicationDbContext`

**📊 Fields:**
- `IApplicationDbContext _context` (Line 19) - private [readonly]

