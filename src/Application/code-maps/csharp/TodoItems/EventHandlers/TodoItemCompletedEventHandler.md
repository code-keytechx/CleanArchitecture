# 🗺️ Code Map: TodoItemCompletedEventHandler

## 📁 File Information

**File Path:** `TodoItems/EventHandlers/TodoItemCompletedEventHandler.cs`
**File Size:** 691 bytes
**Last Modified:** 2025-07-22T16:06:25.724Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoItems/EventHandlers/TodoItemCompletedEventHandler.cs`

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
| **TodoItemCompletedEventHandler** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    TodoItemCompletedEventHandler[🔧 TodoItemCompletedEventHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class TodoItemCompletedEventHandler service
```

### Service Details

#### 🔧 TodoItemCompletedEventHandler

**Namespace:** CleanArchitecture.Application.TodoItems.EventHandlers
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `ILogger<TodoItemCompletedEventHandler>` | `CleanArchitecture.Domain.Events.ILogger` | `CleanArchitecture.Domain.Events` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 TodoItemCompletedEventHandler.TodoItemCompletedEventHandler

**Return Type**: `public`

**Parameters**:
- **logger**: `ILogger<TodoItemCompletedEventHandler>` → *File not found for type: ILogger<TodoItemCompletedEventHandler>*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    TodoItemCompletedEventHandler[📦 TodoItemCompletedEventHandler]
    class TodoItemCompletedEventHandler publicClass
    INotificationHandlerTodoItemCompletedEvent -.->|inherits| TodoItemCompletedEventHandler
    TodoItemCompletedEventHandler -->|uses| ILogger

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
| **TodoItemCompletedEventHandler** | ILogger, INotificationHandler<TodoItemCompletedEvent> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoItems.EventHandlers

#### 🏗️ TodoItemCompletedEventHandler 📦

**Line:** 6 | **Access:** public | **Extends:** INotificationHandler<TodoItemCompletedEvent>

**🔗 Dependencies:** ILogger, INotificationHandler<TodoItemCompletedEvent>

**🔧 Constructors:**
- `TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)` (Line 10)
  - **logger**: `ILogger<TodoItemCompletedEventHandler>` → Generic: ILogger<TodoItemCompletedEventHandler>

**📊 Fields:**
- `ILogger<TodoItemCompletedEventHandler> _logger` (Line 8) - private [readonly]

