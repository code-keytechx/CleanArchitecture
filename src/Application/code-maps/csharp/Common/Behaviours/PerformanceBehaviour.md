# 🗺️ Code Map: PerformanceBehaviour

## 📁 File Information

**File Path:** `Common/Behaviours/PerformanceBehaviour.cs`
**File Size:** 1662 bytes
**Last Modified:** 2025-07-22T16:06:25.722Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Behaviours/PerformanceBehaviour.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 3 |
| 🔗 Dependencies | 4 |
| 📞 Method Calls | 0 |
| 👨‍👩‍👧‍👦 Inheritance | 2 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 1 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **PerformanceBehaviour** | 1 | field | 0 |

### Service Dependency Chain

```mermaid
graph TD
    PerformanceBehaviour[🔧 PerformanceBehaviour]
    PerformanceBehaviour -->|injects| IIdentityService

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class PerformanceBehaviour service
```

### Service Details

#### 🔧 PerformanceBehaviour

**Namespace:** CleanArchitecture.Application.Common.Behaviours
**Injection Type:** field

**Dependencies:**
- IIdentityService

## 📋 Parameter Type Analysis

*No parameters found*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    PerformanceBehaviour[📦 PerformanceBehaviour]
    class PerformanceBehaviour publicClass
    IPipelineBehaviorTRequest -.->|inherits| PerformanceBehaviour
    TResponsewhereTRequestnotnull -.->|inherits| PerformanceBehaviour
    PerformanceBehaviour -->|uses| Stopwatch
    PerformanceBehaviour -->|uses| ILogger
    PerformanceBehaviour -->|uses| IUser
    PerformanceBehaviour -->|uses| IIdentityService

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
| **PerformanceBehaviour** | Stopwatch, ILogger, IUser, IIdentityService, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.Common.Behaviours

#### 🏗️ PerformanceBehaviour 📦

**Line:** 7 | **Access:** public | **Extends:** IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**🔗 Dependencies:** Stopwatch, ILogger, IUser, IIdentityService, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**📊 Fields:**
- `Stopwatch _timer` (Line 9) - private [readonly]
- `ILogger<TRequest> _logger` (Line 10) - private [readonly]
- `IUser _user` (Line 11) - private [readonly]
- `IIdentityService _identityService` (Line 12) - private [readonly]

