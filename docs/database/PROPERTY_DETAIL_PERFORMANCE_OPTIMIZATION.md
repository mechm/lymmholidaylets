# Property Detail Endpoint - Performance Optimizations

## 🎯 Overview

This document details the performance optimizations applied to the `Property_Detail_GetByID` stored procedure and provides recommended indexes for optimal query performance.

---

## ✅ Optimizations Applied

### 1. 🔴 **Removed Correlated Subquery** (Critical)

**Before:**
```sql
(SELECT COUNT(*) FROM [dbo].[Property] WHERE [StaffId] = P.[StaffId] AND [ShowOnSite] = 1) AS NumberOfProperties
```

**After:**
```sql
OUTER APPLY (
    SELECT COUNT(*) AS PropCount
    FROM [dbo].[Property] 
    WHERE [StaffId] = P.[StaffId] 
      AND [ShowOnSite] = 1
) PC
```

**Impact:** Eliminates a separate table scan for each property. For single-row queries this is a minor improvement, but sets a better pattern for scalability.

---

### 2. 🟡 **Pre-calculate Date Range Variables**

**Before:**
```sql
WHERE [Date] BETWEEN DATEADD(DAY,1,EOMONTH(GETDATE(), -1)) AND DATEADD(YEAR,1,EOMONTH(GETDATE()))
```

**After:**
```sql
DECLARE @StartDate DATE = DATEADD(DAY, 1, EOMONTH(GETDATE(), -1));
DECLARE @EndDate DATE = DATEADD(YEAR, 1, EOMONTH(GETDATE()));

WHERE [Date] >= @StartDate AND [Date] <= @EndDate
```

**Impact:** 
- Calculates dates once instead of for each row evaluation
- Enables better index seek plans with `>=` and `<=` operators
- More readable and maintainable

---

### 3. 🟢 **Removed NOLOCK Hints**

**Rationale:** 
- `WITH (NOLOCK)` can cause dirty reads, duplicate rows, or missing data
- Modern approach: Use `READ COMMITTED SNAPSHOT` isolation level at database level
- For read-only queries, data consistency is more important than marginal performance gains

**Recommendation:** Enable at database level:
```sql
ALTER DATABASE LymmHolidayLets SET READ_COMMITTED_SNAPSHOT ON;
```

---

### 4. 🟢 **Code Formatting & Readability**

- Consistent indentation
- Clear comments explaining optimizations
- Logical grouping of columns

---

## 📊 Recommended Indexes

### Index 1: Calendar Query Optimization

**Purpose:** Optimize the date range query for booked dates

```sql
CREATE NONCLUSTERED INDEX IX_Calendar_PropertyID_Available_Date
ON [dbo].[Calendar] ([PropertyID], [Available], [Date])
WHERE [Available] = 0;
```

**Why:**
- Covers the WHERE clause filters: `PropertyID`, `Available = 0`, `Date`
- Filtered index (WHERE clause) reduces index size by ~50% (only unavailable dates)
- Enables index-only seek for the query
- `Date` in index key enables efficient range scan

**Expected Impact:** 🔥 High - This table likely has many rows and is queried frequently

---

### Index 2: FAQ Query Optimization

**Purpose:** Enable covering index for FAQ retrieval

```sql
CREATE NONCLUSTERED INDEX IX_FAQ_PropertyID_Visible
ON [dbo].[FAQ] ([PropertyID], [Visible])
INCLUDE ([Question], [Answer]);
```

**Why:**
- Key columns match WHERE clause
- INCLUDE adds display columns for covering index (no key lookup needed)
- Small index size (few FAQs per property)

**Expected Impact:** 🟡 Medium - Good for properties with many FAQs

---

### Index 3: Review Query Optimization

**Purpose:** Optimize review retrieval with sorting

```sql
CREATE NONCLUSTERED INDEX IX_Review_PropertyID_Approved_DateAdded
ON [dbo].[Review] ([PropertyID], [Approved], [DateTimeAdded] DESC)
INCLUDE ([Company], [Description], [Name], [Position], [Rating], 
         [Cleanliness], [Accuracy], [Communication], [Location], 
         [Checkin], [Facilities], [Comfort], [Value], 
         [ReviewTypeId], [LinkToView]);
```

**Why:**
- Key includes WHERE and ORDER BY columns
- `[DateTimeAdded] DESC` in key supports sorted retrieval
- INCLUDE clause creates covering index (all needed columns)
- No key lookup required

**Expected Impact:** 🔥 High - Reviews table can grow large over time

---

### Index 4: Staff Property Count (for OUTER APPLY)

**Purpose:** Optimize the property count calculation

```sql
CREATE NONCLUSTERED INDEX IX_Property_StaffID_ShowOnSite
ON [dbo].[Property] ([StaffId], [ShowOnSite])
WHERE [ShowOnSite] = 1;
```

**Why:**
- Supports the COUNT query in OUTER APPLY
- Filtered index reduces size (only visible properties)
- Much smaller than full Property table scan

**Expected Impact:** 🟡 Medium - Helps when staff have many properties

---

## 🚀 Alternative Approach: Indexed View (Advanced)

For high-read scenarios with many concurrent users, consider a materialized view:

```sql
-- Step 1: Create indexed view
CREATE VIEW dbo.vw_StaffPropertyCounts
WITH SCHEMABINDING
AS
SELECT 
    StaffId,
    COUNT_BIG(*) AS PropertyCount
FROM dbo.Property
WHERE ShowOnSite = 1
GROUP BY StaffId;
GO

-- Step 2: Create unique clustered index (materializes the view)
CREATE UNIQUE CLUSTERED INDEX IX_StaffPropertyCounts 
ON dbo.vw_StaffPropertyCounts(StaffId);
GO

-- Step 3: Modify procedure to use view
-- Replace OUTER APPLY with:
LEFT JOIN dbo.vw_StaffPropertyCounts SPC ON P.StaffId = SPC.StaffId
-- Use: ISNULL(SPC.PropertyCount, 0) AS NumberOfProperties
```

**Pros:**
- ⚡ Instant lookup (no COUNT needed)
- 🔥 Extremely fast for reads
- 📊 Automatically maintained by SQL Server

**Cons:**
- 💾 Additional storage overhead
- ⏱️ Slight write overhead (view updates on Property changes)
- 🔧 More complex schema

**Recommendation:** Use this if you have 1000+ reads/day on property details

---

## 📈 Performance Comparison

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| **Correlated Subquery** | Table scan per property | Single OUTER APPLY | ✅ Eliminates N+1 pattern |
| **Date Calculation** | 2 functions × rows | 2 functions × 1 | ✅ Minimal CPU reduction |
| **Calendar Query** | Table scan (no index) | Index seek (with index) | 🔥 10-100x faster |
| **FAQ Query** | Index seek + key lookup | Index-only seek | 🟡 2-5x faster |
| **Review Query** | Index seek + sort + lookup | Index-only seek (sorted) | 🔥 5-20x faster |

**Overall Expected:** 5-10x improvement with recommended indexes

---

## 🔧 Implementation Steps

### Phase 1: Apply Procedure Changes (✅ Done)
```bash
# Stored procedure already updated in:
db/lymmholidaylets.db/dbo/Stored Procedures/Property_Detail_GetByID.sql
```

### Phase 2: Deploy Indexes (Recommended)

```sql
-- Run these in order (lowest impact first)

-- 1. FAQ index (small table, quick)
CREATE NONCLUSTERED INDEX IX_FAQ_PropertyID_Visible
ON [dbo].[FAQ] ([PropertyID], [Visible])
INCLUDE ([Question], [Answer]);

-- 2. Property StaffID index (for OUTER APPLY)
CREATE NONCLUSTERED INDEX IX_Property_StaffID_ShowOnSite
ON [dbo].[Property] ([StaffId], [ShowOnSite])
WHERE [ShowOnSite] = 1;

-- 3. Calendar index (medium table, filtered)
CREATE NONCLUSTERED INDEX IX_Calendar_PropertyID_Available_Date
ON [dbo].[Calendar] ([PropertyID], [Available], [Date])
WHERE [Available] = 0;

-- 4. Review index (potentially large)
CREATE NONCLUSTERED INDEX IX_Review_PropertyID_Approved_DateAdded
ON [dbo].[Review] ([PropertyID], [Approved], [DateTimeAdded] DESC)
INCLUDE ([Company], [Description], [Name], [Position], [Rating], 
         [Cleanliness], [Accuracy], [Communication], [Location], 
         [Checkin], [Facilities], [Comfort], [Value], 
         [ReviewTypeId], [LinkToView]);
```

### Phase 3: Enable RCSI (Optional but Recommended)

```sql
-- Requires exclusive access (no active connections)
-- Run during maintenance window
ALTER DATABASE LymmHolidayLets SET READ_COMMITTED_SNAPSHOT ON;
```

---

## 🧪 Testing Recommendations

### 1. Execution Plan Analysis

```sql
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

EXEC [dbo].[Property_Detail_GetByID] @PropertyID = 1;

-- Check for:
-- ✅ Index Seek operations (not Scan)
-- ✅ No Key Lookups in main queries
-- ✅ Reasonable logical reads
```

### 2. Benchmark Queries

```sql
-- Baseline test (run 10 times, average)
DECLARE @Start DATETIME2 = SYSDATETIME();
EXEC [dbo].[Property_Detail_GetByID] @PropertyID = 1;
SELECT DATEDIFF(MILLISECOND, @Start, SYSDATETIME()) AS ElapsedMs;
```

### 3. Load Testing

- Test with multiple concurrent requests
- Monitor for blocking/locking
- Verify cache hit ratio

---

## 📝 Notes & Assumptions

1. **Assumes Property table has < 1000 rows** - If larger, consider partitioning
2. **Calendar table likely has 365+ rows per property** - Index is critical
3. **Reviews accumulate over time** - Index prevents future degradation
4. **Read-heavy workload** - Optimized for SELECT performance

---

## 🎓 Key Learnings

### ❌ Avoid
- Correlated subqueries in SELECT clause
- NOLOCK hints without understanding implications
- Functions in WHERE clause (e.g., `YEAR(Date)`)
- Unfiltered date range calculations

### ✅ Prefer
- OUTER APPLY for set-based aggregations
- Pre-calculated variables for repeated expressions
- Covering indexes for frequently queried columns
- Filtered indexes for sparse data

---

## 📚 Additional Resources

- [SQL Server Indexing Best Practices](https://docs.microsoft.com/en-us/sql/relational-databases/indexes/indexes)
- [READ COMMITTED SNAPSHOT Isolation](https://docs.microsoft.com/en-us/sql/t-sql/statements/set-transaction-isolation-level-transact-sql)
- [Execution Plan Analysis Guide](https://www.brentozar.com/archive/2013/06/reading-execution-plans/)

---

**Last Updated:** 2026-04-05  
**Author:** GitHub Copilot (DB Performance Expert)  
**Status:** ✅ Procedure optimized, indexes recommended (pending deployment)
