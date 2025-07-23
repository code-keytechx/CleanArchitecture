# 🗺️ Code Map: ValidationBehaviour

## 📁 File Information

**File Path:** `Common/Behaviours/ValidationBehaviour.cs`
**File Size:** 1192 bytes
**Last Modified:** 2025-07-22T16:06:25.723Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Behaviours/ValidationBehaviour.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 1 |
| 🔗 Dependencies | 0 |
| 📞 Method Calls | 0 |
| 👨‍👩‍👧‍👦 Inheritance | 2 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 0 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **ValidationBehaviour** | 0 | constructor | 1 |

### Service Dependency Chain

```mermaid
graph TD
    ValidationBehaviour[🔧 ValidationBehaviour]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class ValidationBehaviour service
```

### Service Details

#### 🔧 ValidationBehaviour

**Namespace:** CleanArchitecture.Application.Common.Behaviours
**Injection Type:** constructor

**Methods with Dependencies:**

## 📋 Parameter Type Analysis

### Parameter Type Summary

| Type | Full Path | Namespace | Used In Methods | Occurrences |
|------|-----------|-----------|-----------------|-------------|
| `unknown` | `ValidationException = CleanArchitecture.Application.Common.Exceptions.ValidationException.unknown` | `ValidationException = CleanArchitecture.Application.Common.Exceptions.ValidationException` | 1 | 1 |

### Method Parameter Breakdown

#### 🔧 ValidationBehaviour.ValidationBehaviour

**Return Type**: `public`

**Parameters**:
- **unknown**: `unknown` → *File not found for type: unknown*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    ValidationBehaviour[📦 ValidationBehaviour]
    class ValidationBehaviour publicClass
    IPipelineBehaviorTRequest -.->|inherits| ValidationBehaviour
    TResponse -.->|inherits| ValidationBehaviour

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
| **ValidationBehaviour** | IPipelineBehavior<TRequest, TResponse> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.Common.Behaviours

#### 🏗️ ValidationBehaviour 📦

**Line:** 5 | **Access:** public | **Extends:** IPipelineBehavior<TRequest, TResponse>

**🔗 Dependencies:** IPipelineBehavior<TRequest, TResponse>

**🔧 Constructors:**
- `ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)` (Line 10)
  - **unknown**: `unknown`

