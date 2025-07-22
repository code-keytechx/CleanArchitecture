# 🗺️ Code Map: MappingExtensions

## 📁 File Information

**File Path:** `Common/Mappings/MappingExtensions.cs`
**File Size:** 827 bytes
**Last Modified:** 2025-07-22T16:06:25.723Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/Common/Mappings/MappingExtensions.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 1 |
| 🔗 Dependencies | 2 |
| 📞 Method Calls | 1 |
| 👨‍👩‍👧‍👦 Inheritance | 0 |
| 🔧 Service Classes | 1 |
| 💉 Service Dependencies | 1 |
| 🎯 Method Dependencies | 0 |

## 🔧 Service Hierarchy Analysis

### Service Classes Overview

| Service | Dependencies | Injection Type | Methods |
|---------|--------------|----------------|---------|
| **MappingExtensions** | 1 | none | 0 |

### Service Dependency Chain

```mermaid
graph TD
    MappingExtensions[🔧 MappingExtensions]
    MappingExtensions -->|injects| IConfigurationProvider

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class MappingExtensions service
```

### Service Details

#### 🔧 MappingExtensions

**Namespace:** CleanArchitecture.Application.Common.Mappings
**Injection Type:** none

**Dependencies:**
- IConfigurationProvider

## 📋 Parameter Type Analysis

*No parameters found*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    MappingExtensions[📦 MappingExtensions]
    class MappingExtensions publicClass
    MappingExtensions -->|uses| CancellationToken
    MappingExtensions -->|uses| CancellationToken
    MappingExtensions -.->|calls| queryable

    classDef publicClass fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef privateClass fill:#fce4ec,stroke:#880e4f,stroke-width:2px
    classDef interface fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef enum fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
```

## 🌳 Class Hierarchy

```
📦 MappingExtensions
```

## 📋 Dependencies Matrix

| Class | Dependencies |
|-------|---------------|
| **MappingExtensions** | CancellationToken, queryable |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.Common.Mappings

#### 🏗️ MappingExtensions 🔧

**Line:** 5 | **Access:** public | **Modifiers:** static

**🔗 Dependencies:** CancellationToken, queryable

**📞 Calls:** queryable.AsNoTracking()

