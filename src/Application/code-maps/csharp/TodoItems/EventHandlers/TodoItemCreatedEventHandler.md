# 🗺️ Code Map: TodoItemCreatedEventHandler

## 📁 File Information

**File Path:** `TodoItems/EventHandlers/TodoItemCreatedEventHandler.cs`
**File Size:** 679 bytes
**Last Modified:** 2025-07-22T16:06:25.724Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoItems/EventHandlers/TodoItemCreatedEventHandler.cs`

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
| **TodoItemCreatedEventHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    TodoItemCreatedEventHandler[🔧 TodoItemCreatedEventHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class TodoItemCreatedEventHandler service
```

### Service Details

#### 🔧 TodoItemCreatedEventHandler

**Namespace:** CleanArchitecture.Application.TodoItems.EventHandlers
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `ILogger<TodoItemCreatedEventHandler>` | `CleanArchitecture.Domain.Events.ILogger` | `CleanArchitecture.Domain.Events` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 TodoItemCreatedEventHandler.TodoItemCreatedEventHandler

**Return Type**: `public`

**Parameters**:
- **logger**: `ILogger<TodoItemCreatedEventHandler>` → *File not found for type: ILogger<TodoItemCreatedEventHandler>*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    TodoItemCreatedEventHandler[📦 TodoItemCreatedEventHandler]
    class TodoItemCreatedEventHandler publicClass
    INotificationHandlerTodoItemCreatedEvent -.->|inherits| TodoItemCreatedEventHandler
    TodoItemCreatedEventHandler -->|uses| ILogger

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
| **TodoItemCreatedEventHandler** | ILogger, INotificationHandler<TodoItemCreatedEvent> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoItems.EventHandlers

#### 🏗️ TodoItemCreatedEventHandler 📦

**Line:** 6 | **Access:** public | **Extends:** INotificationHandler<TodoItemCreatedEvent>

**🔗 Dependencies:** ILogger, INotificationHandler<TodoItemCreatedEvent>

**🔧 Constructors:**
- `TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)` (Line 10)
  - **logger**: `ILogger<TodoItemCreatedEventHandler>` → Generic: ILogger<TodoItemCreatedEventHandler>

**📊 Fields:**
- `ILogger<TodoItemCreatedEventHandler> _logger` (Line 8) - private [readonly]

