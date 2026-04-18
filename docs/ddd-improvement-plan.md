# DDD Improvement Plan — LymmHolidayLets

> **Session ID:** 36f532b6-bffb-47b6-9c17-80031ae6903b  
> **Status:** Planned — not yet implemented  
> To resume: open this file and ask Copilot to "resume the DDD improvement plan"

## Assessment Summary

The codebase has a **solid structural foundation** — Clean/Hexagonal architecture is correctly layered, the dependency flow is respected, repositories are pure persistence, and the `Booking` aggregate is a genuinely rich model with factory methods, private constructors, and encapsulated value objects (`StayPeriod`, `ContactInfo`).

**DDD Score: 5.5 / 10**

The main weaknesses are:
1. **Widespread anemic domain models** — most entities are data bags with public setters and no behavior
2. **Business logic in query classes** — rating calculations live in `PropertyQuery`, not in the domain
3. **Business logic in controllers** — `PageController` and `PropertyController` own caching + domain rules
4. **Missing value objects** — denormalized data (Review ratings, property occupancy) that should be encapsulated

Repositories and infrastructure are clean. Controllers are mostly thin — only 2 need work. The `CheckoutService` and `CalGenerator` are legitimate orchestration services.

---

## Identified Domain Concepts

| Concept | Current Location | DDD Type | Status |
|---|---|---|---|
| `Booking` | `Domain/Model/Booking/Entity/` | Aggregate Root | ✅ Rich |
| `StayPeriod` | `Domain/Model/Booking/ValueObject/` | Value Object | ✅ Immutable |
| `ContactInfo` | `Domain/Model/Booking/ValueObject/` | Value Object | ✅ Immutable |
| `DateRange` | `Domain/Model/Common/` | Value Object | ✅ Good |
| `GuestCount` | `Domain/Model/Common/` | Value Object | ✅ Good |
| `Calendar` | `Domain/Model/Calendar/Entity/` | Aggregate Root | ❌ Anemic |
| `Checkout` | `Domain/Model/Checkout/Entity/` | Aggregate Root | ❌ Anemic |
| `Review` | `Domain/Model/Review/Entity/` | Aggregate Root | ❌ Anemic + denormalized |
| `EmailEnquiry` | `Domain/Model/EmailEnquiry/Entity/` | Aggregate Root | ❌ Anemic |
| `Page` | `Domain/Model/Page/Entity/` | Aggregate Root | ⚠️ Minimal |
| `FAQ`, `ICal`, `Staff`, `Slideshow`, `SiteMap` | `Domain/Model/*/Entity/` | Aggregate Root | ❌ Anemic |
| `ReviewRatings` | _Does not exist_ | Value Object | 🔴 Missing |
| `PropertyOccupancy` | _Does not exist_ | Value Object | 🔴 Missing |
| `CalculateService` | `Application/Service/` | Domain Service | ✅ Well-placed |
| `CheckoutService` | `Application/Service/` | Application Service | ✅ Orchestration |
| `CalGenerator` | `Application/Service/` | Application Service | ✅ Orchestration |

---

## Todo List

| # | ID | Title | Status |
|---|---|---|---|
| 1 | `ddd-vo-review-ratings` | Extract `ReviewRatings` value object | ⬜ Pending |
| 2 | `ddd-vo-property-occupancy` | Extract `PropertyOccupancy` value object | ⬜ Pending |
| 3 | `ddd-calendar-behavior` | Add behavior to `Calendar` entity | ⬜ Pending |
| 4 | `ddd-review-behavior` | Add `Approve()` behavior to `Review` entity | ⬜ Pending |
| 5 | `ddd-checkout-entity-behavior` | Add `IsExpired()` to `Checkout` entity | ⬜ Pending |
| 6 | `ddd-property-query-extract` | Extract rating calculation out of `PropertyQuery` | ⬜ Pending (needs #1) |
| 7 | `ddd-page-controller-cache` | Move `PageController` cache + visibility logic to service | ⬜ Pending |
| 8 | `ddd-property-controller-cache` | Move `PropertyController` cache + invalidation to service | ⬜ Pending |
| 9 | `ddd-email-recaptcha` | Move ReCAPTCHA validation out of `EmailController` | ⬜ Pending |
| 10 | `ddd-tests-update` | Update unit tests to cover new domain behavior | ⬜ Pending (needs #1–6) |

---

## Phase 1 — Value Object Extraction (Domain)

**Goal:** Replace raw denormalized data with encapsulated, immutable value objects.

### 1a. `ReviewRatings` Value Object
**Problem:** `Review` has 8 individually nullable `byte` rating fields (`Rating`, `Accuracy`, `Cleanliness`, `Communication`, `Checkin`, `Location`, `Facilities`, `Comfort`, `Value`). These are a coherent concept leaking as flat columns.

**File:** `Domain/Model/Review/ValueObject/ReviewRatings.cs`

```csharp
public sealed record ReviewRatings(
    byte Overall,
    byte? Accuracy,
    byte? Cleanliness,
    byte? Communication,
    byte? CheckIn,
    byte? Location,
    byte? Facilities,
    byte? Comfort,
    byte? Value)
{
    public static PropertyRatingSummaryResult? ComputeSummary(IEnumerable<ReviewRatings> ratings)
    {
        var list = ratings.ToList();
        if (list.Count == 0) return null;

        return new PropertyRatingSummaryResult
        {
            Rating        = list.Average(r => r.Overall),
            Accuracy      = NullableAverage(list, r => r.Accuracy),
            Cleanliness   = NullableAverage(list, r => r.Cleanliness),
            Communication = NullableAverage(list, r => r.Communication),
            CheckInExperience = NullableAverage(list, r => r.CheckIn),
            Value         = NullableAverage(list, r => r.Value),
            Location      = NullableAverage(list, r => r.Location),
            Facilities    = NullableAverage(list, r => r.Facilities),
            Comfort       = NullableAverage(list, r => r.Comfort),
        };
    }

    private static double? NullableAverage(IList<ReviewRatings> ratings, Func<ReviewRatings, byte?> selector)
    {
        var values = ratings.Select(selector).Where(v => v.HasValue).Select(v => (double)v!.Value).ToList();
        return values.Count > 0 ? values.Average() : null;
    }
}
```

Update `Review` entity to use `Ratings` property of type `ReviewRatings`.

### 1b. `PropertyOccupancy` Value Object
**Problem:** `PropertyBooking` read model has 6 guest capacity fields scattered as flat properties.

**File:** `Domain/Model/Property/ValueObject/PropertyOccupancy.cs`

```csharp
public sealed record PropertyOccupancy(
    byte MinimumAdults,
    byte MaximumGuests,
    byte MaximumAdults,
    byte? MaximumChildren,
    byte? MaximumInfants);
```

---

## Phase 2 — Domain Behavior (Entities)

**Goal:** Move business rules from services/controllers into the entities that own them.

### 2a. `Calendar` — Add availability behavior

```csharp
// In Calendar.cs
public bool IsAvailable() => Available && !Booked;

public Calendar BlockForBooking(int bookingId) =>
    new(ID, PropertyID, Date, Price, MinimumStay, MaximumStay,
        available: false, booked: true, bookingId);
```

### 2b. `Review` — Approval behavior

```csharp
// In Review.cs — replace public Approved setter with:
public bool Approved { get; private set; }
public DateTime? DateTimeApproved { get; private set; }

public void Approve()
{
    if (Approved) throw new ClientSideException("Review is already approved.");
    Approved = true;
    DateTimeApproved = DateTime.UtcNow;
}
```

### 2c. `Checkout` — Expiry behavior

```csharp
// In Checkout.cs
public bool IsExpired(TimeSpan sessionTtl) =>
    DateTime.UtcNow - Created > sessionTtl;
```

---

## Phase 3 — Application Layer Cleanup

**Goal:** Remove domain/mapping logic from query classes.

### 3a. Extract rating calculation from `PropertyQuery`

`PropertyQuery.GetPropertyDetailByIdAsync()` contains ~50 lines of rating average calculation and deep object mapping.

- Move `NullableAverage` + `PropertyRatingSummaryResult` construction → `ReviewRatings.ComputeSummary()`
- Extract full property detail mapping → `PropertyDetailMapper` in `Application/Mapping/`
- `PropertyQuery` becomes a thin delegator again

**File:** `Application/Mapping/PropertyDetailMapper.cs`

```csharp
public static class PropertyDetailMapper
{
    public static PropertyDetailResult Map(PropertyDetailAggregate aggregate) { ... }
}
```

---

## Phase 4 — Controller Cleanup

**Goal:** Eliminate business logic from controllers.

### 4a. `PageController` → `IPageQueryService`

**Problem:** `PageController.Detail()` contains cache management + `if (!page.Visible) return NotFound()`.

**Create:** `Application/Interface/Service/IPageQueryService.cs` + `Application/Service/PageQueryService.cs`

```csharp
public interface IPageQueryService
{
    Task<PageDetail?> GetVisiblePageByAliasAsync(string alias);
}
```

The service owns `IMemoryCache` interaction and visibility check. Controller becomes:
```csharp
var page = await pageQueryService.GetVisiblePageByAliasAsync(alias);
if (page is null) return NotFound();
return Ok(ApiResponse<PageDetail>.SuccessResult(page));
```

### 4b. `PropertyController` → `IPropertyDetailQueryService`

**Problem:** Calendar timestamp comparison for cache invalidation in controller.

**Create:** `Application/Interface/Service/IPropertyDetailQueryService.cs` + implementation

```csharp
public interface IPropertyDetailQueryService
{
    Task<PropertyDetailResult?> GetPropertyDetailAsync(byte propertyId);
}
```

### 4c. `EmailController` — ReCAPTCHA validation

Move `recaptchaValidationService.ValidateAsync()` into `EmailEnquiryService.ProcessEnquiryAsync()`.  
Throw `ClientSideException("Security verification failed.")` on failure.  
Controller should not know about reCAPTCHA.

---

## Phase 5 — Tests

New test cases in `LymmHolidayLets.UnitTests`:

| Class | Scenarios |
|---|---|
| `ReviewRatings.ComputeSummary()` | Correct averages; handles null dimensions; null for empty list |
| `Calendar.IsAvailable()` | True when Available=true, Booked=false; false for all other combos |
| `Calendar.BlockForBooking()` | Returns new Calendar with Booked=true, correct BookingID |
| `Review.Approve()` | Sets Approved + timestamp; throws `ClientSideException` on double-approve |
| `Checkout.IsExpired()` | False within TTL; true after TTL |
| `PropertyDetailMapper.Map()` | Maps all fields; handles null host; handles empty reviews |
| `PageQueryService` | Cache hit; cache miss fetches; invisible page returns null |
| `PropertyDetailQueryService` | Changed calendar timestamp evicts and re-fetches |

---

## What NOT to Change

- **Repositories** — clean, pure persistence
- **`Booking` aggregate** — already rich
- **`CheckoutService`** — legitimate orchestration
- **`CalGenerator`** — correct scope
- **`CalculateService`** — well-placed domain service
- **EF models** (`CalendarEF`, `PageEF`, `PropertyEF`) — anemia is correct for persistence models
- **`StripeWebHookController`, `ReviewController`, `CalendarController`** — thin and clean
