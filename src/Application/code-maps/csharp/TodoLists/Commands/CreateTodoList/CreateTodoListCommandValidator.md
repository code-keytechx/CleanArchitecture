# 🗺️ Code Map: CreateTodoListCommandValidator

## 📁 File Information

**File Path:** `TodoLists/Commands/CreateTodoList/CreateTodoListCommandValidator.cs`
**File Size:** 837 bytes
**Last Modified:** 2025-07-22T16:06:25.725Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoLists/Commands/CreateTodoList/CreateTodoListCommandValidator.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 1 |
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
| **CreateTodoListCommandValidator** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    CreateTodoListCommandValidator[🔧 CreateTodoListCommandValidator]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class CreateTodoListCommandValidator service
```

### Service Details

#### 🔧 CreateTodoListCommandValidator

**Namespace:** CleanArchitecture.Application.TodoLists.Commands.CreateTodoList
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces.IApplicationDbContext` | `CleanArchitecture.Application.Common.Interfaces` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 CreateTodoListCommandValidator.CreateTodoListCommandValidator

**Return Type**: `public`

**Parameters**:
- **context**: `IApplicationDbContext` → `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Interfaces/IApplicationDbContext.cs`

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    CreateTodoListCommandValidator[📦 CreateTodoListCommandValidator]
    class CreateTodoListCommandValidator publicClass
    AbstractValidatorCreateTodoListCommand -.->|inherits| CreateTodoListCommandValidator
    CreateTodoListCommandValidator -->|uses| IApplicationDbContext

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
| **CreateTodoListCommandValidator** | IApplicationDbContext, AbstractValidator<CreateTodoListCommand> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoLists.Commands.CreateTodoList

#### 🏗️ CreateTodoListCommandValidator 📦

**Line:** 5 | **Access:** public | **Extends:** AbstractValidator<CreateTodoListCommand>

**🔗 Dependencies:** IApplicationDbContext, AbstractValidator<CreateTodoListCommand>

**🔧 Constructors:**
- `CreateTodoListCommandValidator(IApplicationDbContext context)` (Line 9)
  - **context**: `IApplicationDbContext`

**📊 Fields:**
- `IApplicationDbContext _context` (Line 7) - private [readonly]

