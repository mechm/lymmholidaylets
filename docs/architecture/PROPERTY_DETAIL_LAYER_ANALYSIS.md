# Property Detail Endpoint - Architectural Layer Analysis

## 🏗️ Layer Separation & Responsibilities

### Overview

The social sharing links (`FacebookShareLink`, `TwitterShareLink`, etc.) and `PropertyUrl` are **presentation-layer concerns** that depend on the HTTP request context. They have been correctly placed in the **API layer** rather than polluting lower layers with UI-specific logic.

---

## ✅ Correct Layer Placement

### 1. **Domain Layer** (`LymmHolidayLets.Domain`)

**Purpose:** Pure business entities and domain logic  
**Dependencies:** None (bottom layer)

**Property-related classes:**
- `PropertyBooking.cs` - Booking capacity data
- `PropertyHost.cs` - Host read model (new)
- `PropertyMap.cs` - Map coordinates read model (new)
- `FAQ.cs` - Frequently asked questions
- `Review.cs` - Guest reviews

**✅ Correctly contains:** Business data with no HTTP/UI concerns  
**❌ Does NOT contain:** Sharing links, URLs, computed properties

---

### 2. **Application Layer** (`LymmHolidayLets.Application.Model`)

**Purpose:** Business data transfer objects for orchestration  
**Dependencies:** Domain only

**Property-related classes:**
- `PropertyDetailResult.cs` - Complete property detail DTO
  - `PropertyHostResult` - Host information
  - `PropertyMapResult` - Map coordinates
  - `PropertyFaqResult` - FAQ items
  - `PropertyReviewAggregateResult` - Aggregated reviews

**✅ Correctly contains:** Business data needed by the application  
**❌ Does NOT contain:** HTTP context, URLs, presentation logic

**Why PropertyUrl is NOT here:**
- PropertyUrl depends on `HttpContext.Request` (scheme, host, path)
- Different environments = different URLs (localhost vs. production)
- Not business data, but presentation metadata

---

### 3. **Infrastructure Layer** (`LymmHolidayLets.Infrastructure`)

**Purpose:** Data access implementation  
**Dependencies:** Domain, Application.Model

**Property-related classes:**
- `DapperPropertyDataAdapter.cs` - Executes stored procedures
- `PropertyQuery.cs` - Maps from domain to application models

**✅ Correctly contains:** Database queries, mapping logic  
**❌ Does NOT contain:** HTTP concerns, presentation logic

---

### 4. **API Layer** (`LymmHolidayLets.Api`)

**Purpose:** REST API controllers and presentation models  
**Dependencies:** All other layers

**Property-related classes:**
- `PropertyController.cs` - REST endpoint
- **`PropertyDetailResponse.cs`** ✨ **NEW** - Wrapper with social links
- **`SocialShareLinkGenerator.cs`** ✨ **NEW** - Computes links from HTTP context

**✅ Correctly contains:**
- HTTP request/response handling
- PropertyUrl computation (from HttpContext)
- Social sharing link generation
- Presentation wrappers

---

## 📦 Data Flow

```
HTTP Request
     ↓
PropertyController
     ↓
PropertyQuery (Application)
     ↓
DapperPropertyDataAdapter (Infrastructure)
     ↓
Property_Detail_GetByID (Database)
     ↓
PropertyDetailAggregate (Domain)
     ↓
PropertyDetailResult (Application)
     ↓
SocialShareLinkGenerator (API) ← Uses HttpContext
     ↓
PropertyDetailResponse (API)
     ↓
ApiResponse<PropertyDetailResponse>
     ↓
HTTP Response
```

---

## 🎯 Why This Architecture is Correct

### Problem: Social Sharing Links Depend on HTTP Context

```csharp
// WRONG - This would violate layer separation:
public class PropertyDetailResult
{
    public string FacebookShareLink { get; init; } // ❌ Depends on HttpContext!
}
```

**Why wrong:**
- `PropertyDetailResult` is in `Application.Model` (business layer)
- `HttpContext` is an ASP.NET Core concept (presentation layer)
- Business logic shouldn't know about HTTP requests
- Can't unit test without mocking HTTP infrastructure
- Violates Single Responsibility Principle

### Solution: Presentation Wrapper Pattern

```csharp
// CORRECT - Business data in Application layer:
public class PropertyDetailResult
{
    public byte PropertyId { get; init; }
    public string? DisplayAddress { get; init; }
    // ... business data only
}

// CORRECT - Presentation wrapper in API layer:
public class PropertyDetailResponse
{
    public required PropertyDetailResult PropertyDetail { get; init; } // Business data
    public required string PropertyUrl { get; init; }                   // Computed from HTTP
    public required string FacebookShareLink { get; init; }             // Computed from HTTP
    // ...
}
```

**Benefits:**
1. ✅ Clean separation of concerns
2. ✅ Business layer is HTTP-agnostic (testable without ASP.NET)
3. ✅ Presentation logic isolated in API layer
4. ✅ PropertyDetailResult can be reused in different contexts (GraphQL, gRPC, etc.)
5. ✅ Easy to change social sharing URLs without touching business logic

---

## 🔧 Implementation Pattern

### Service: `ISocialShareLinkGenerator`

```csharp
public interface ISocialShareLinkGenerator
{
    SocialShareLinks GenerateLinks(byte propertyId, string? displayAddress);
}
```

**Responsibilities:**
- Access `IHttpContextAccessor` to get request details
- Generate URLs based on current scheme/host/path
- URL-encode parameters for social media platforms
- Return structured link data

**Registered as:** `Transient` (stateless, safe to create per request)

### Controller Usage

```csharp
public async Task<IActionResult> Detail(byte id)
{
    // 1. Get business data (cached)
    var detail = await propertyQuery.GetPropertyDetailByIdAsync(id);
    
    // 2. Generate presentation properties (HTTP context)
    var shareLinks = socialShareLinkGenerator.GenerateLinks(id, detail.DisplayAddress);
    
    // 3. Wrap business data with presentation metadata
    var response = new PropertyDetailResponse
    {
        PropertyDetail = detail,        // ← Application layer
        PropertyUrl = shareLinks.Prop ertyUrl,  // ← API layer
        FacebookShareLink = shareLinks.FacebookShareLink,
        // ...
    };
    
    return Ok(ApiResponse<PropertyDetailResponse>.SuccessResult(response));
}
```

---

## 📊 Comparison: Wrong vs. Right Approach

| Aspect | ❌ Wrong (Links in Application) | ✅ Right (Links in API) |
|--------|--------------------------------|------------------------|
| **Layer violation** | Yes - Application depends on HTTP | No - Clean separation |
| **Testability** | Hard - Need to mock HttpContext | Easy - Mock interface |
| **Reusability** | Low - Tied to ASP.NET | High - Business model portable |
| **SRP compliance** | No - Mixed business + presentation | Yes - Single responsibility |
| **Caching** | Hard - URLs vary per request | Easy - Cache business data only |
| **Maintainability** | Low - Changes affect multiple layers | High - Changes isolated |

---

## 🧪 Testing Benefits

### Business Logic (Application Layer)
```csharp
// Easy to test - no HTTP concerns
[Fact]
public async Task GetPropertyDetailByIdAsync_MapsHostInformation()
{
    var aggregate = new PropertyDetailAggregate(...);
    var result = await propertyQuery.GetPropertyDetailByIdAsync(1);
    
    result.Host.Name.Should().Be("John Doe");
    // No need to mock HttpContext!
}
```

### Presentation Logic (API Layer)
```csharp
// Mock the link generator - simple interface
[Fact]
public async Task Detail_ReturnsSocialShareLinks()
{
    _socialShareLinkGenerator
        .Setup(g => g.GenerateLinks(1, "Test Property"))
        .Returns(new SocialShareLinks { ... });
    
    var result = await controller.Detail(1);
    // Test presentation concerns independently
}
```

---

## 🎓 Key Takeaways

### ✅ Do:
1. Keep business data in Application.Model
2. Keep HTTP/presentation concerns in API layer
3. Use wrappers/decorators to add presentation metadata
4. Inject services for context-dependent operations

### ❌ Don't:
1. Reference `IHttpContextAccessor` in Application or Domain
2. Compute URLs in business logic
3. Mix presentation and business concerns in DTOs
4. Cache presentation data that varies per request

---

## 📚 Related Patterns

- **Wrapper Pattern** - PropertyDetailResponse wraps PropertyDetailResult
- **Decorator Pattern** - Adding presentation metadata to business data
- **Dependency Inversion** - ISocialShareLinkGenerator abstraction
- **Single Responsibility** - Each layer has one reason to change

---

**Last Updated:** 2026-04-05  
**Status:** ✅ Architecture validated, all tests passing  
**Layers:** Correctly separated per clean architecture principles
