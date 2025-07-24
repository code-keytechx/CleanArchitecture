# 🗺️ Code Map: AuthorizationBehaviour

## 📁 File Information

**File Path:** `Common/Behaviours/AuthorizationBehaviour.cs`
**File Size:** 2785 bytes
**Last Modified:** 2025-07-24T08:28:13.689Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Behaviours/AuthorizationBehaviour.cs`

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
| 💉 Service Dependencies | 1 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **AuthorizationBehaviour** | 1 | field | 0 |

### Service Dependency Chain

```mermaid
graph TD
    AuthorizationBehaviour[🔧 AuthorizationBehaviour]
    AuthorizationBehaviour -->|injects| IIdentityService

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class AuthorizationBehaviour service
```

### Service Details

#### 🔧 AuthorizationBehaviour

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
    AuthorizationBehaviour[📦 AuthorizationBehaviour]
    class AuthorizationBehaviour publicClass
    IPipelineBehaviorTRequest -.->|inherits| AuthorizationBehaviour
    TResponsewhereTRequestnotnull -.->|inherits| AuthorizationBehaviour
    AuthorizationBehaviour -->|uses| IUser
    AuthorizationBehaviour -->|uses| IIdentityService

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
| **AuthorizationBehaviour** | IUser, IIdentityService, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.Common.Behaviours

#### 🏗️ AuthorizationBehaviour 📦

**Line:** 8 | **Access:** public | **Extends:** IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**🔗 Dependencies:** IUser, IIdentityService, IPipelineBehavior<TRequest, TResponse> where TRequest : notnull

**📊 Fields:**
- `IUser _user` (Line 10) - private [readonly]
- `IIdentityService _identityService` (Line 11) - private [readonly]

