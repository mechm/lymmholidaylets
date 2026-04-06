# Property Detail Enhancement - Implementation Plan (REVISED)

## Current State Analysis

### Existing Implementation
**Endpoint:** `GET /api/v1/property/detail/{id}`  
**Stored Procedure:** `Property_Detail_GetByID` (4 result sets)  
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

**Pattern Decision:** Keep current approach - `ReadAsync<T>()` returns `IEnumerable<T>` which is **already materialized** by Dapper. The returned enumerable is not a lazy database query but a buffered result in memory. Using `IEnumerable<T>` in the domain model is more flexible than `List<T>`. **No `.ToList()` needed.**

---

## Missing from Live Site Comparison

After reviewing https://lymmholidaylets.com/property/detail/1, the following data is **missing** from the current API response:

### 1. **Property Images** ❌ BLOCKED - NO DATA IN DATABASE
- Image paths, order, visibility, alt text
- **Database:** `PropertyImage` table exists BUT no data currently populated
- **Action Required:** Need to add image data for all properties before exposing in API
- **Impact:** Cannot implement until images are manually added to database

### 2. **Bedroom Configuration** ❌ MISSING - NO SCHEMA
- Bedroom names/numbers ("Bedroom 2")
- Bed types (double bed, single bed, etc.)
- Number of beds per bedroom
- **Database:** No schema currently exists - need to create `BedType` and `PropertyBedroom` tables
- **Impact:** Can implement schema now, but need manual data entry for all properties

### 3. **Property Amenities** ⚠️ PENDING - VERIFY DATA
- Amenities list ("Hi-speed Wifi", "TV", "Kitchenware", etc.)
- **Database:** `PropertyFeatureType` + `FeatureType` tables exist
- **Action Required:** Verify that data exists in these tables before exposing
- **Impact:** If data exists, can expose immediately

### 4. **Room Counts** ✅ DATA EXISTS
- ✅ Already in Property table: `Bedroom`, `Bathroom`, `ReceptionRoom`, `Kitchen`, `CarSpace`
- ❌ Not returned in API: These columns exist but aren't included in stored procedure
- **Impact:** Can add immediately - just extend stored procedure SELECT

---

## Critical Data Status Review

**Before Implementation:**
1. ✅ Check if `FeatureType` and `PropertyFeatureType` have data populated
2. ❌ `PropertyImage` table is empty - images need to be added first
3. ❌ Bedroom configuration tables don't exist yet

**Recommendation:** Implement in phases based on data availability.

---

## Implementation Phases (Based on Data Availability)

### ✅ Phase 1: Room Counts (IMMEDIATE - Data Exists)
Add room counts to existing stored procedure result set - no new tables needed.

**Changes Required:**
- Update `Property_Detail_GetByID.sql` to include room count columns
- Update `PropertyBooking` read model to include new properties
- Update `PropertyDetailResult` and `PropertyDetailResponse` models
- No new tables needed

**Estimated Effort:** 1-2 hours  
**Risk:** Low - data already exists

---

### ⚠️ Phase 2: Amenities (PENDING - Verify Data Exists)
Expose amenities if `PropertyFeatureType` and `FeatureType` data exists.

**Pre-Implementation Check:**
```sql
-- Run this to verify data exists
SELECT COUNT(*) FROM PropertyFeatureType;
SELECT COUNT(*) FROM FeatureType;
SELECT * FROM FeatureType;
```

**If Data Exists:**
- Update `Property_Detail_GetByID.sql` to add 5th result set for amenities
- Update `DapperPropertyDataAdapter.cs` to read amenity result set
- Update `PropertyDetailAggregate` to include amenities
- Update `PropertyDetailResult` to include `List<string> Amenities`

**Estimated Effort:** 2-3 hours  
**Risk:** Medium - depends on data quality

---

### 🔨 Phase 3: Bedroom Configuration (FUTURE - Requires Schema + Data)
Create schema + populate data for all properties.

**DACPAC Structure:**
```
db/lymmholidaylets.db/
├── dbo/
│   ├── Tables/
│   │   ├── BedType.sql              ← CREATE NEW TABLE
│   │   └── PropertyBedroom.sql      ← CREATE NEW TABLE
│   └── Stored Procedures/
│       └── Property_Detail_GetByID.sql  ← ADD 6th RESULT SET
├── Script.PostDeployment.sql        ← SEED BedType lookup + example data
└── LymmHolidayLets.Db.sqlproj      ← ADD NEW FILES
```

**Database Changes (DACPAC ONLY):**

#### 3.1 BedType Table
**File:** `db/lymmholidaylets.db/dbo/Tables/BedType.sql`

```sql
CREATE TABLE [dbo].[BedType] (
    [ID] TINYINT IDENTITY(1,1) NOT NULL,
    [Description] VARCHAR(50) NOT NULL,
    [IconPath] VARCHAR(255) NULL,
    [Created] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_BedType] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_BedType_Description] UNIQUE NONCLUSTERED ([Description] ASC)
);
```

#### 3.2 PropertyBedroom Table
**File:** `db/lymmholidaylets.db/dbo/Tables/PropertyBedroom.sql`

```sql
CREATE TABLE [dbo].[PropertyBedroom] (
    [ID] SMALLINT IDENTITY(1,1) NOT NULL,
    [PropertyId] TINYINT NOT NULL,
    [BedroomNumber] TINYINT NOT NULL,
    [BedroomName] VARCHAR(100) NULL,
    [BedTypeId] TINYINT NOT NULL,
    [NumberOfBeds] TINYINT NOT NULL DEFAULT 1,
    [SequenceOrder] TINYINT NOT NULL DEFAULT 1,
    [ShowOnSite] BIT NOT NULL DEFAULT 1,
    [Created] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    [Updated] DATETIME2(0) NULL,
    CONSTRAINT [PK_PropertyBedroom] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyBedroom_Property] FOREIGN KEY ([PropertyId]) 
        REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [FK_PropertyBedroom_BedType] FOREIGN KEY ([BedTypeId]) 
        REFERENCES [dbo].[BedType] ([ID])
);

GO

CREATE NONCLUSTERED INDEX [IX_PropertyBedroom_PropertyId] 
    ON [dbo].[PropertyBedroom] ([PropertyId]) 
    INCLUDE ([ShowOnSite], [SequenceOrder]);
```

#### 3.3 Seed Data
**File:** `db/lymmholidaylets.db/Script.PostDeployment.sql` (append to end)

```sql
-- =============================================
-- Seed BedType lookup data
-- =============================================
SET IDENTITY_INSERT [dbo].[BedType] ON;
GO

MERGE INTO [dbo].[BedType] AS [Target]
USING (VALUES
  (1, N'Single Bed', N'/images/Bedroom-Icon-Single.svg'),
  (2, N'Double Bed', N'/images/Bedroom-Icon-Double.svg'),
  (3, N'King Bed', N'/images/Bedroom-Icon-King.svg'),
  (4, N'Queen Bed', N'/images/Bedroom-Icon-Queen.svg'),
  (5, N'Sofa Bed', N'/images/Bedroom-Icon-Sofa.svg'),
  (6, N'Bunk Bed', N'/images/Bedroom-Icon-Bunk.svg')
) AS [Source] ([ID], [Description], [IconPath])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
    NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR 
    NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL OR
    NULLIF([Source].[IconPath], [Target].[IconPath]) IS NOT NULL OR 
    NULLIF([Target].[IconPath], [Source].[IconPath]) IS NOT NULL
) THEN UPDATE SET
  [Description] = [Source].[Description],
  [IconPath] = [Source].[IconPath]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID], [Description], [IconPath])
 VALUES([Source].[ID], [Source].[Description], [Source].[IconPath])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[BedType] OFF;
GO
```

#### 3.4 Update Stored Procedure
**File:** `db/lymmholidaylets.db/dbo/Stored Procedures/Property_Detail_GetByID.sql`

Add 6th result set:
```sql
-- Result Set 6: Bedroom configuration
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

#### 3.5 Application Changes
**Files to update:**
- `Domain/ReadModel/Property/PropertyDetailAggregate.cs` - add `IEnumerable<PropertyBedroom> Bedrooms`
- `Application.Model/Property/PropertyDetailResult.cs` - add `List<PropertyBedroomResult> Bedrooms`
- `Infrastructure/DataAdapter/DapperPropertyDataAdapter.cs` - read 6th result set
- Add new class: `Domain/ReadModel/Property/PropertyBedroom.cs`
- Add new class: `Application.Model/Property/PropertyBedroomResult.cs`

**Estimated Effort:** 4-6 hours (schema + code + testing)  
**Risk:** Medium - requires manual data entry for all properties after deployment

---

### 🚫 Phase 4: Images (BLOCKED - Requires Image Data First)
Cannot implement until images are added to `PropertyImage` table.

**Database:** `PropertyImage` table exists with columns:
- `ImagePath` (VARCHAR)
- `SequenceOrder` (TINYINT)
- `ShowOnSite` (BIT)
- `Optimised` (BIT)
- ⚠️ **Missing:** `AltText` column (needs to be added)

**DACPAC Changes Required:**

#### 4.1 Alter PropertyImage Table
**File:** `db/lymmholidaylets.db/dbo/Tables/PropertyImage.sql` (modify existing file)

Add column to existing CREATE TABLE statement:
```sql
[AltText] VARCHAR(255) NULL,
```

#### 4.2 Update Stored Procedure
**File:** `db/lymmholidaylets.db/dbo/Stored Procedures/Property_Detail_GetByID.sql`

Add 7th result set (or 5th if bedrooms not implemented yet):
```sql
-- Result Set: Property images
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

#### 4.3 Application Changes
**Files to update:**
- `Domain/ReadModel/Property/PropertyDetailAggregate.cs` - add `IEnumerable<PropertyImage> Images`
- `Application.Model/Property/PropertyDetailResult.cs` - add `List<PropertyImageResult> Images`
- `Infrastructure/DataAdapter/DapperPropertyDataAdapter.cs` - read images result set
- Add new class: `Domain/ReadModel/Property/PropertyImage.cs`
- Add new class: `Application.Model/Property/PropertyImageResult.cs`

**Estimated Effort:** 3-4 hours (after images are populated)  
**Risk:** High - **BLOCKED** until images are manually added to database

---

## DACPAC Deployment Process

**All database changes MUST go through the dacpac project:**

1. Create/modify `.sql` files in `db/lymmholidaylets.db/dbo/Tables/` or `dbo/Stored Procedures/`
2. Add new files to `LymmHolidayLets.Db.sqlproj` in `<ItemGroup>`:
   ```xml
   <Build Include="dbo\Tables\BedType.sql" />
   <Build Include="dbo\Tables\PropertyBedroom.sql" />
   ```
3. Add seed data to `Script.PostDeployment.sql` using MERGE statements
4. Build dacpac: `dotnet build db/lymmholidaylets.db/LymmHolidayLets.Db.sqlproj`
5. Deploy to local: Use publish profile `LymmHolidayLets.Db.Local.publish.xml`

**NO adhoc SQL queries** - everything goes through dacpac deployment.

---

## Recommended Implementation Order

### ✅ Implement Now (Phase 1 + Phase 2)
1. **Room Counts** - data exists, low risk
2. **Amenities** - verify data exists first, then expose

### 🔨 Implement Next (Phase 3)
3. **Bedroom Configuration** - create schema + seed data, requires manual data entry after deployment

### 🚫 Defer Until Later (Phase 4)
4. **Images** - blocked until someone populates `PropertyImage` table with actual image data

---

## Pre-Implementation Checklist

Before starting any phase, verify:
- [ ] Check if `FeatureType` and `PropertyFeatureType` have data: `SELECT COUNT(*) FROM FeatureType`
- [ ] Check if `PropertyImage` has data: `SELECT COUNT(*) FROM PropertyImage`
- [ ] Review current `Property_Detail_GetByID.sql` to understand existing result sets
- [ ] Review `PropertyDetailAggregate.cs` constructor signature
- [ ] Identify mapping code in `DapperPropertyDataAdapter.GetPropertyDetailByIdAsync`
- [ ] Run all tests before making changes: `dotnet test lymmholidaylets.slnx`

---

## Testing Strategy

For each phase:
1. **Unit Tests:**
   - Update `DapperPropertyDataAdapterTests` to test new result set mappings
   - Add test cases for null/empty collections

2. **Integration Tests:**
   - Update `PropertyControllerTests` to verify new fields in response
   - Test with real database (if data exists)

3. **Manual Testing:**
   - Call `/api/v1/property/detail/1` and verify response shape
   - Check GraphQL endpoint if exposed there too

---

## Questions to Resolve Before Implementation

1. ✅ **Data Access Pattern:** Keep current `IEnumerable<T>` pattern (no `.ToList()`)
2. ⚠️ **Amenities Data:** Does `PropertyFeatureType` + `FeatureType` have data? → Need to verify
3. ❌ **Images Data:** `PropertyImage` is empty - who will populate?
4. ❌ **Bedroom Data:** Who will populate `PropertyBedroom` after schema is created?
5. ✅ **DACPAC:** All changes must be in dacpac project - no adhoc SQL

---

## Summary

**Immediate Actions:**
- ✅ Implement Phase 1 (Room Counts) - low risk, data exists
- ⚠️ Verify amenities data exists, then implement Phase 2
- 🔨 Create bedroom schema (Phase 3), but requires manual data entry
- 🚫 Defer images (Phase 4) until data is populated

**Key Principle:** All database changes via dacpac, follow existing Dapper patterns, verify data exists before exposing new fields.
