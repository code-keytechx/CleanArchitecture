# 🗺️ Code Map: GetWeatherForecastsQuery

## 📁 File Information

**File Path:** `WeatherForecasts/Queries/GetWeatherForecasts/GetWeatherForecastsQuery.cs`
**File Size:** 1092 bytes
**Last Modified:** 2025-07-22T16:06:25.726Z

---


**File Path:** `/Users/quang.vuong/Documents/Development/CleanArchitecture/src/Application/WeatherForecasts/Queries/GetWeatherForecasts/GetWeatherForecastsQuery.cs`

## 📊 Quick Stats

| Type | Count |
|------|-------|
| 📁 Namespaces | 1 |
| 🏗️ Classes | 1 |
| 🎭 Interfaces | 0 |
| 📝 Enums | 0 |
| 📚 Using Statements | 0 |
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
| **GetWeatherForecastsQueryHandler** | 0 | none | 0 |

### Service Dependency Chain

```mermaid
graph TD
    GetWeatherForecastsQueryHandler[🔧 GetWeatherForecastsQueryHandler]

    classDef service fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    class GetWeatherForecastsQueryHandler service
```

### Service Details

#### 🔧 GetWeatherForecastsQueryHandler

**Namespace:** CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts
**Injection Type:** none

## 📋 Parameter Type Analysis

*No parameters found*

## 🎯 Method Dependency Analysis

*No method dependencies found*

## 🕸️ Visual Dependency Graph

```mermaid
graph TD
    GetWeatherForecastsQueryHandler[📦 GetWeatherForecastsQueryHandler]
    class GetWeatherForecastsQueryHandler publicClass
    IRequestHandlerGetWeatherForecastsQuery -.->|inherits| GetWeatherForecastsQueryHandler
    IEnumerableWeatherForecast -.->|inherits| GetWeatherForecastsQueryHandler

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
| **GetWeatherForecastsQueryHandler** | IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>> |

## 🔍 Detailed Structure

### 📁 CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts

#### 🏗️ GetWeatherForecastsQueryHandler 📦

**Line:** 5 | **Access:** public | **Extends:** IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>

**🔗 Dependencies:** IRequestHandler<GetWeatherForecastsQuery, IEnumerable<WeatherForecast>>

