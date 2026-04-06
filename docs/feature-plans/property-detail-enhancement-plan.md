# Property Detail Enhancement - Implementation Plan

## Current State Analysis

### Existing Implementation
**Endpoint:** `GET /api/v1/property/detail/{id}`  
**Stored Procedure:** `Property_Detail_GetByID`  
**Data Access:** `DapperPropertyDataAdapter.GetPropertyDetailByIdAsync`  
**Domain Model:** `PropertyDetailAggregate` → mapped to `PropertyDetailResult` → `PropertyDetailResponse`

**Currently Returns:**
- ✅ Basic property info (DisplayAddress, Description)
- ✅ Guest capacity (adults, children, infants)
- ✅ Booked dates
- ✅ FAQs
- ✅ Reviews with aggregates
- ✅ Host information (name, location, properties, years experience, bio, image)
- ✅ Map data (Google Map + Street View coordinates)
- ✅ Social sharing links (Facebook, Twitter, LinkedIn, Email)

### Current Data Access Pattern (Dapper)
**From `DapperPropertyDataAdapter.cs` line 83-86:**
```csharp
var datesBooked = await result.ReadAsync<DateOnly>();
var faqs        = await result.ReadAsync<FAQ>();
var reviews     = await result.ReadAsync<Review>();
```

**Important:** Uses deferred execution - `ReadAsync<T>()` returns `IEnumerable<T>` which is **not materialized** until enumerated. The `PropertyDetailAggregate` constructor accepts `IEnumerable<T>` and stores references, which is efficient for memory.

### Missing from Live Site Comparison

After reviewing https://lymmholidaylets.com/property/detail/1, the following data is **missing** from the current API response:

#### 1. **Property Images** ❌ MISSING - NO DATA IN DATABASE
- Image paths, order, visibility, alt text
- **Database:** `PropertyImage` table exists BUT no data currently populated
- **Action Required:** Need to add image data for all properties before exposing in API

#### 2. **Bedroom Configuration** ❌ MISSING - NO SCHEMA
- Bedroom names/numbers ("Bedroom 2")
- Bed types (double bed, single bed, etc.)
- Number of beds per bedroom
- **Database:** No schema currently exists - need to create tables

#### 3. **Property Amenities** ❌ MISSING
- Amenities list ("Hi-speed Wifi", "TV", "Kitchenware", etc.)
- **Database:** `PropertyFeatureType` + `FeatureType` tables exist
- **Note:** Need to verify data exists in these tables

#### 4. **Room Counts** ⚠️ PARTIALLY MISSING
- ✅ Already in Property table: `Bedroom`, `Bathroom`, `ReceptionRoom`, `Kitchen`, `CarSpace`
- ❌ Not returned in API: These columns exist but aren't included in stored procedure

---

## Critical Data Status Review

**Before Implementation:**
1. ✅ Check if `FeatureType` and `PropertyFeatureType` have data
2. ❌ `PropertyImage` table is empty - images need to be added first
3. ❌ Bedroom configuration tables don't exist

**Recommendation:** Implement in phases based on data availability.

---

## Proposed Changes - REVISED

### Data Access Pattern Decision

**Question:** Should we use `.ToList()` or keep deferred execution?

**Current Pattern (line 83-86 in DapperPropertyDataAdapter):**
```csharp
var datesBooked = await result.ReadAsync<DateOnly>();    // IEnumerable<DateOnly>
var faqs        = await result.ReadAsync<FAQ>();          // IEnumerable<FAQ>
var reviews     = await result.ReadAsync<Review>();       // IEnumerable<Review>
```

**Analysis:**
- Dapper's `ReadAsync<T>()` already materializes the data into memory
- The returned `IEnumerable<T>` is **not** a lazy database query - it's a buffered result
- The `PropertyDetailAggregate` constructor accepts `IEnumerable<T>`
- The data is iterated during JSON serialization

**Decision:** **KEEP CURRENT PATTERN** - No `.ToList()` needed. The data is already materialized by Dapper's `ReadAsync`, and using `IEnumerable<T>` is more flexible for the domain model. Only convert to `List<T>` if you need indexing or Count operations.

---

### Implementation Phases (Based on Data Availability)

#### Phase 1: Room Counts (IMMEDIATE - Data Exists)
Add room counts to existing result set - no new tables needed.

#### Phase 2: Amenities (PENDING - Verify Data Exists)
Expose amenities if `PropertyFeatureType` data exists.

#### Phase 3: Bedroom Configuration (FUTURE - Requires Schema + Data)
Create schema + populate data for all properties.

#### Phase 4: Images (BLOCKED - Requires Image Data First)
Cannot implement until images are added to `PropertyImage` table.

---

### DACPAC Project Structure

**All database changes MUST be in the dacpac project:**

```
db/lymmholidaylets.db/
├── dbo/
│   ├── Tables/
│   │   ├── BedType.sql              ← NEW TABLE
│   │   ├── PropertyBedroom.sql      ← NEW TABLE
│   │   └── PropertyImage.sql        ← ALTER (add AltText)
│   └── Stored Procedures/
│       └── Property_Detail_GetByID.sql  ← UPDATE
├── Script.PostDeployment.sql        ← SEED DATA
└── LymmHolidayLets.Db.sqlproj      ← ADD FILES
```

**NO adhoc SQL queries** - everything goes through dacpac deployment.

---

### Phase 1: Add Missing Database Fields

#### 1.1 Bedroom Configuration Table (NEW)
Create table to store bedroom configurations:

```sql
CREATE TABLE [dbo].[PropertyBedroom] (
    [ID] SMALLINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [PropertyId] TINYINT NOT NULL,
    [BedroomNumber] TINYINT NOT NULL,
    [BedroomName] VARCHAR(100) NULL,  -- e.g. "Master Bedroom", "Bedroom 2"
    [BedTypeId] TINYINT NOT NULL,     -- FK to BedType
    [NumberOfBeds] TINYINT NOT NULL DEFAULT 1,
    [SequenceOrder] TINYINT NOT NULL DEFAULT 1,
    [ShowOnSite] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [FK_PropertyBedroom_Property] FOREIGN KEY ([PropertyId]) 
        REFERENCES [dbo].[Property] ([ID])
);

CREATE TABLE [dbo].[BedType] (
    [ID] TINYINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Description] VARCHAR(50) NOT NULL,  -- "Single Bed", "Double Bed", "King Bed", etc.
    [IconPath] VARCHAR(255) NULL,
    CONSTRAINT [UQ_BedType_Description] UNIQUE ([Description])
);
```

#### 1.2 Image Alt Text (MODIFY EXISTING)
Add alt text column to PropertyImage:

```sql
ALTER TABLE [dbo].[PropertyImage]
ADD [AltText] VARCHAR(255) NULL;
```

### Phase 2: Update Stored Procedure

Modify `Property_Detail_GetByID` to return 5 result sets (currently returns 4):

**Result Set 1:** Property basic info + room counts
```sql
SELECT 
    P.[ID], 
    P.[DisplayAddress],
    P.[Description] AS PageDescription,
    -- Guest capacity (existing)
    P.[MinimumNumberOfAdult], 
    P.[MaximumNumberOfAdult], 
    P.[MaximumNumberOfGuests], 
    P.[MaximumNumberOfChildren], 
    P.[MaximumNumberOfInfants],
    -- Room counts (ADD THESE - already in table)
    P.[Bedroom] AS NumberOfBedrooms,
    P.[Bathroom] AS NumberOfBathrooms,
    P.[ReceptionRoom] AS NumberOfReceptionRooms,
    P.[Kitchen] AS NumberOfKitchens,
    P.[CarSpace] AS NumberOfCarSpaces,
    -- Host info (existing)
    S.[Name] AS HostName,
    ...
```

**Result Set 2:** Booked dates (existing - no changes)

**Result Set 3:** FAQs (existing - no changes)

**Result Set 4:** Reviews (existing - no changes)

**Result Set 5:** Property Images (NEW)
```sql
-- Property images ordered by sequence
SELECT 
    [ImagePath],
    [AltText],
    [SequenceOrder],
    [ShowOnSite]
FROM [dbo].[PropertyImage]
WHERE [PropertyId] = @PropertyID 
  AND [ShowOnSite] = 1
ORDER BY [SequenceOrder];
```

**Result Set 6:** Bedroom Configuration (NEW)
```sql
-- Bedroom configuration
SELECT 
    PB.[BedroomNumber],
    PB.[BedroomName],
    BT.[Description] AS BedType,
    BT.[IconPath] AS BedTypeIcon,
    PB.[NumberOfBeds]
FROM [dbo].[PropertyBedroom] PB
INNER JOIN [dbo].[BedType] BT ON PB.[BedTypeId] = BT.[ID]
WHERE PB.[PropertyId] = @PropertyID 
  AND PB.[ShowOnSite] = 1
ORDER BY PB.[SequenceOrder];
```

**Result Set 7:** Amenities (NEW)
```sql
-- Amenities/features
SELECT 
    FT.[Description] AS AmenityName
FROM [dbo].[PropertyFeatureType] PFT
INNER JOIN [dbo].[FeatureType] FT ON PFT.[FeatureTypeId] = FT.[ID]
WHERE PFT.[PropertyId] = @PropertyID 
  AND PFT.[ShowOnSite] = 1
ORDER BY FT.[Description];
```

### Phase 3: Update Application Models

#### 3.1 PropertyDetailResult.cs
Add new properties:

```csharp
public sealed class PropertyDetailResult
{
    // Existing properties...
    
    // ── Room counts (NEW) ─────────────────────────────────────────────────
    public byte NumberOfBedrooms { get; init; }
    public double NumberOfBathrooms { get; init; }  // Can be fractional (e.g., 1.5)
    public byte NumberOfReceptionRooms { get; init; }
    public byte NumberOfKitchens { get; init; }
    public byte NumberOfCarSpaces { get; init; }
    
    // ── Images (NEW) ──────────────────────────────────────────────────────
    public IReadOnlyList<PropertyImageResult> Images { get; init; } = [];
    
    // ── Bedroom configuration (NEW) ───────────────────────────────────────
    public IReadOnlyList<PropertyBedroomResult> Bedrooms { get; init; } = [];
    
    // ── Amenities (NEW) ───────────────────────────────────────────────────
    public IReadOnlyList<string> Amenities { get; init; } = [];
    
    // Existing: DatesBooked, FAQs, ReviewAggregate, Host, Map...
}

public sealed class PropertyImageResult
{
    public required string ImagePath { get; init; }
    public string? AltText { get; init; }
    public byte SequenceOrder { get; init; }
}

public sealed class PropertyBedroomResult
{
    public byte BedroomNumber { get; init; }
    public string? BedroomName { get; init; }
    public required string BedType { get; init; }
    public string? BedTypeIcon { get; init; }
    public byte NumberOfBeds { get; init; }
}
```

### Phase 4: Update Infrastructure (Data Access)

#### 4.1 PropertyQuery.cs
Update `GetPropertyDetailByIdAsync` to map the new result sets from Dapper:

```csharp
public async Task<PropertyDetailResult?> GetPropertyDetailByIdAsync(byte propertyId)
{
    using var multi = await connection.QueryMultipleAsync(
        "Property_Detail_GetByID",
        new { PropertyID = propertyId },
        commandType: CommandType.StoredProcedure);

    var property = await multi.ReadFirstOrDefaultAsync<PropertyDetailDto>();
    if (property is null) return null;

    var datesBooked = (await multi.ReadAsync<DateOnly>()).ToList();
    var faqs = (await multi.ReadAsync<PropertyFaqDto>()).ToList();
    var reviews = (await multi.ReadAsync<ReviewDto>()).ToList();
    var images = (await multi.ReadAsync<PropertyImageDto>()).ToList();      // NEW
    var bedrooms = (await multi.ReadAsync<PropertyBedroomDto>()).ToList();  // NEW
    var amenities = (await multi.ReadAsync<string>()).ToList();            // NEW

    return MapToResult(property, datesBooked, faqs, reviews, images, bedrooms, amenities);
}
```

### Phase 5: Update API Response

No changes needed to `PropertyDetailResponse.cs` - it wraps `PropertyDetailResult` which will automatically include the new data.

### Phase 6: Testing

#### 6.1 Unit Tests
- Update `PropertyQueryTests` to verify new fields are mapped
- Add tests for bedroom configuration parsing
- Add tests for image ordering

#### 6.2 Integration Tests
- Update `PropertyControllerTests.Detail_ValidId_ReturnsExpectedStructure`
- Verify new fields are present in API response JSON

---

## Database Migration Steps

### Step 1: Create BedType lookup table
```sql
CREATE TABLE [dbo].[BedType] (...);

INSERT INTO [dbo].[BedType] ([Description], [IconPath]) VALUES
    ('Single Bed', '/images/Bedroom-Icon-Single.svg'),
    ('Double Bed', '/images/Bedroom-Icon-Double.svg'),
    ('King Bed', '/images/Bedroom-Icon-King.svg'),
    ('Queen Bed', '/images/Bedroom-Icon-Queen.svg'),
    ('Sofa Bed', '/images/Bedroom-Icon-Sofa.svg'),
    ('Bunk Bed', '/images/Bedroom-Icon-Bunk.svg');
```

### Step 2: Create PropertyBedroom table
```sql
CREATE TABLE [dbo].[PropertyBedroom] (...);
```

### Step 3: Add AltText to PropertyImage
```sql
ALTER TABLE [dbo].[PropertyImage] ADD [AltText] VARCHAR(255) NULL;
```

### Step 4: Seed bedroom data (example for Property 1)
```sql
-- Property 1 has 2 double bedrooms based on live site
INSERT INTO [dbo].[PropertyBedroom] 
    ([PropertyId], [BedroomNumber], [BedroomName], [BedTypeId], [NumberOfBeds], [SequenceOrder])
VALUES
    (1, 1, 'Bedroom 1', 2, 1, 1),  -- BedTypeId=2 is "Double Bed"
    (1, 2, 'Bedroom 2', 2, 1, 2);
```

---

## Implementation Checklist

### Database (dacpac project)
- [ ] Create `BedType` table + seed data
- [ ] Create `PropertyBedroom` table
- [ ] Alter `PropertyImage` to add `AltText`
- [ ] Update `Property_Detail_GetByID` stored procedure

### Domain Models
- [ ] Add `PropertyImageResult` class
- [ ] Add `PropertyBedroomResult` class
- [ ] Update `PropertyDetailResult` with new properties

### Infrastructure
- [ ] Update `PropertyQuery.GetPropertyDetailByIdAsync` to read new result sets
- [ ] Add DTOs for images, bedrooms
- [ ] Update mapping logic

### API
- [ ] No changes needed (wraps PropertyDetailResult)

### Tests
- [ ] Update `PropertyQueryTests`
- [ ] Update `PropertyControllerTests`
- [ ] Add bedroom configuration tests
- [ ] Add image ordering tests

---

## Assumptions & Limitations

### Assumptions
1. **Images:** All properties already have images in `PropertyImage` table
2. **Bedroom Data:** Will need to be manually seeded for existing properties
3. **Amenities:** Already exist in `FeatureType` table - just need to expose them
4. **Backwards Compatibility:** New fields are additive - existing API consumers won't break

### Limitations
1. **Bed Configuration:** Requires manual data entry for existing properties
2. **Alt Text:** Currently not populated - will be NULL until manually added
3. **Icon Paths:** Bed type icons need to exist in `/images/` directory

### Data Quality Checks Needed
- Verify all properties have at least one bedroom entry
- Verify all properties have at least one image
- Verify amenities are properly tagged

---

## Timeline Estimate

| Phase | Description | Estimated Time |
|-------|-------------|----------------|
| 1 | Database schema changes | 1 hour |
| 2 | Stored procedure update | 1 hour |
| 3 | Application models | 1 hour |
| 4 | Infrastructure/data access | 2 hours |
| 5 | Testing | 2 hours |
| 6 | Data seeding (3 properties) | 1 hour |
| **Total** | | **8 hours** |

---

## Rollout Plan

1. **Development:** Implement all changes, test with seeded data for properties 1-3
2. **Staging:** Deploy to staging, verify API responses match live site
3. **Data Migration:** Seed bedroom/amenity data for all live properties
4. **Production:** Deploy + monitor API response times (caching should handle load)

---

## Cache Invalidation Note

Current cache key: `property-detail-{id}` with 10-minute TTL.  
After DB updates, manually clear cache or wait for natural expiration.

```csharp
cache.Remove($"property-detail-{propertyId}");
```
