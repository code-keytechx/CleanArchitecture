# 🗺️ Code Map: UnhandledExceptionBehaviour

## 📁 File Information

**File Path:** `Common/Behaviours/UnhandledExceptionBehaviour.cs`
**File Size:** 856 bytes
**Last Modified:** 2025-07-22T16:06:25.723Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Behaviours/UnhandledExceptionBehaviour.cs`

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
| 👨‍👩‍👧‍👦 Inheritance | 2 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **UnhandledExceptionBehaviour** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    UnhandledExceptionBehaviour[🔧 UnhandledExceptionBehaviour]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class UnhandledExceptionBehaviour service
```

### Service Details

#### 🔧 UnhandledExceptionBehaviour

**Namespace:** CleanArchitecture.Application.Common.Behaviours
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `ILogger<TRequest>` | `Microsoft.Extensions.Logging.ILogger` | `Microsoft.Extensions.Logging` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 UnhandledExceptionBehaviour.UnhandledExceptionBehaviour

**Return Type**: `public`

**Parameters**:
- **logger**: `ILogger<TRequest>` → *File not found for type: ILogger<TRequest>*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    UnhandledExceptionBehaviour[📦 UnhandledExceptionBehaviour]
    class UnhandledExceptionBehaviour publicClass
    IPipelineBehaviorTRequest -.->|inherits| UnhandledExceptionBehaviour
    TResponsewhereTRequestnotnull -.->|inherits| UnhandledExceptionBehaviour
    UnhandledExceptionBehaviour -->|uses| ILogger

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
| **UnhandledExceptionBehaviour** | ILogger, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.Common.Behaviours

#### 🏗️ UnhandledExceptionBehaviour 📦

**Line:** 5 | **Access:** public | **Extends:** IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**🔗 Dependencies:** ILogger, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**🔧 Constructors:**
- `UnhandledExceptionBehaviour(ILogger<TRequest> logger)` (Line 9)
  - **logger**: `ILogger<TRequest>` → Generic: ILogger<TRequest>

**📊 Fields:**
- `ILogger<TRequest> _logger` (Line 7) - private [readonly]

