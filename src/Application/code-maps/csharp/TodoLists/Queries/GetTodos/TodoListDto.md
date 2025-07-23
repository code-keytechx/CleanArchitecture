# 🗺️ Code Map: TodoListDto

## 📁 File Information

**File Path:** `TodoLists/Queries/GetTodos/TodoListDto.cs`
**File Size:** 549 bytes
**Last Modified:** 2025-07-22T16:06:25.726Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/TodoLists/Queries/GetTodos/TodoListDto.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 2 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 1 |
| 🔗 Dependencies | 0 |
| 📞 Method Calls | 0 |
| 👨‍👩‍👧‍👦 Inheritance | 1 |
| 🔧 Service Classes | 2 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **TodoListDto** | 0 | constructor | 1 |
| **Mapping** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    TodoListDto[🔧 TodoListDto]
    Mapping[🔧 Mapping]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class TodoListDto service
    class Mapping service
```

### Service Details

#### 🔧 TodoListDto

**Namespace:** CleanArchitecture.Application.TodoLists.Queries.GetTodos
**Injection Type:** constructor

**Methods with Dependencies:**

#### 🔧 Mapping

**Namespace:** CleanArchitecture.Application.TodoLists.Queries.GetTodos
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

*No parameters found*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    TodoListDto[📦 TodoListDto]
    class TodoListDto publicClass
    Mapping[📦 Mapping]
    class Mapping privateClass
    Profile -.->|inherits| Mapping

    classDef publicClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef privateClass fill:#fce4ec,stroke:#880e4f,stroke-width:2px
    classDef interface fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef enum fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
```

## 🌳 Class Hierarchy

```
📦 TodoListDto
```

## 📋 Dependencies Matrix

| Class | Dependencies |
|-------|---------------|
| **TodoListDto** | _none_ |
| **Mapping** | Profile |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.TodoLists.Queries.GetTodos

#### 🏗️ TodoListDto 📦

**Line:** 5 | **Access:** public

**🔧 Constructors:**
- `TodoListDto()` (Line 7)

#### 🏗️ Mapping 📦

**Line:** 20 | **Access:** private | **Extends:** Profile

**🔗 Dependencies:** Profile

**🔧 Constructors:**
- `Mapping()` (Line 22)

