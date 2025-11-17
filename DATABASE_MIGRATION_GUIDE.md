# ?? DATABASE MIGRATION - SCHEDULING TABLES

## Prerequisites

- Visual Studio or Visual Studio Code
- SQL Server or SQL Server Express
- Entity Framework Core CLI or Package Manager Console

---

## Step 1: Create Migration

### Option A: Package Manager Console (Visual Studio)

```powershell
# Open Package Manager Console
# Tools ? NuGet Package Manager ? Package Manager Console

# Ensure FIXIT.DAL is selected as default project
# Run:
Add-Migration "AddSchedulingTables"
```

### Option B: Command Line (dotnet CLI)

```bash
cd "E:\ITI\FIXIT\Api Project"
dotnet ef migrations add "AddSchedulingTables" -p FIXIT.DAL -s FIXIT.API
```

---

## Step 2: Review Migration

A file will be created in `FIXIT.DAL\Migrations\` with a timestamp like:
```
YYYYMMDDHHMMSS_AddSchedulingTables.cs
```

This file contains:
- Creating `CraftsManAvailabilities` table
- Creating `CraftsManTimeOffs` table
- Adding foreign key relationships
- Adding proper indexes

**Expected tables:**
```sql
CREATE TABLE CraftsManAvailabilities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CraftsManId INT NOT NULL,
    DayOfWeek INT NOT NULL,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (CraftsManId) REFERENCES CraftsMan(Id)
);

CREATE TABLE CraftsManTimeOffs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CraftsManId INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    Type INT NOT NULL,
    Reason NVARCHAR(MAX),
    IsApproved BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    FOREIGN KEY (CraftsManId) REFERENCES CraftsMan(Id)
);
```

---

## Step 3: Apply Migration

### Option A: Package Manager Console

```powershell
Update-Database
```

### Option B: Command Line

```bash
dotnet ef database update -p FIXIT.DAL -s FIXIT.API
```

### Option C: SQL Server Management Studio

1. Open SQL Server Management Studio
2. Connect to your database
3. Open a New Query window
4. Copy the migration script from the migration file
5. Execute the script

---

## Step 4: Verify Migration

### Check in SQL Server Management Studio

```sql
-- Check if tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('CraftsManAvailabilities', 'CraftsManTimeOffs')
ORDER BY TABLE_NAME;

-- Check columns in CraftsManAvailabilities
EXEC sp_columns 'CraftsManAvailabilities';

-- Check columns in CraftsManTimeOffs
EXEC sp_columns 'CraftsManTimeOffs';

-- Check foreign keys
SELECT 
    name AS ForeignKey,
    OBJECT_NAME(parent_object_id) AS TableName,
    OBJECT_NAME(referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys
WHERE OBJECT_NAME(parent_object_id) IN ('CraftsManAvailabilities', 'CraftsManTimeOffs');
```

### Check in Visual Studio

1. Open `SQL Server Object Explorer`
2. Navigate to your database
3. Expand `Tables`
4. Look for:
   - `CraftsManAvailabilities`
   - `CraftsManTimeOffs`

---

## Troubleshooting

### Issue: Migration Not Found

**Error:** "No DbContext named 'FixItDbContext' was found"

**Solution:**
```powershell
# Specify the context explicitly
Add-Migration "AddSchedulingTables" -Context FixItDbContext
```

---

### Issue: Foreign Key Constraint Violation

**Error:** "The INSERT, UPDATE, or DELETE statement conflicted with a FOREIGN KEY constraint"

**Solution:**
- Ensure CraftsMan IDs exist in the Clients table before inserting
- Check your test data

---

### Issue: Column Type Mismatch

**Error:** "Conversion failed when converting date and/or time"

**Solution:**
- Ensure DateTime values are in ISO 8601 format
- Use `DateTime.UtcNow` for current dates

---

### Issue: Unable to Find Migration

**Error:** "No migrations found"

**Solution:**
```powershell
# List all migrations
Get-Migration

# If no migrations exist, check your project structure
# Migrations folder should be at: FIXIT.DAL\Migrations\
```

---

## Rollback (If Needed)

### Rollback Last Migration

```powershell
# Package Manager Console
Update-Database -Migration <PreviousMigrationName>

# Or use dotnet CLI
dotnet ef database update <PreviousMigrationName> -p FIXIT.DAL -s FIXIT.API
```

### Remove Migration

```powershell
# Only if migration not yet applied
Remove-Migration

# Or use dotnet CLI
dotnet ef migrations remove -p FIXIT.DAL
```

---

## Sample Data (Optional)

After migration, you can insert test data:

```sql
-- Insert test availability
INSERT INTO CraftsManAvailabilities 
(CraftsManId, DayOfWeek, StartTime, EndTime, IsAvailable, CreatedAt, UpdatedAt)
VALUES 
(1, 1, '09:00:00', '17:00:00', 1, GETUTCDATE(), GETUTCDATE()),
(1, 2, '09:00:00', '17:00:00', 1, GETUTCDATE(), GETUTCDATE()),
(1, 3, '09:00:00', '17:00:00', 1, GETUTCDATE(), GETUTCDATE()),
(1, 4, '09:00:00', '17:00:00', 1, GETUTCDATE(), GETUTCDATE()),
(1, 5, '09:00:00', '17:00:00', 1, GETUTCDATE(), GETUTCDATE());

-- Insert test time off
INSERT INTO CraftsManTimeOffs 
(CraftsManId, StartDate, EndDate, Type, Reason, IsApproved, CreatedAt)
VALUES 
(1, '2025-12-20 00:00:00', '2025-12-27 23:59:59', 0, 'Holiday vacation', 1, GETUTCDATE());
```

---

## Verification Checklist

? Migration file created in `FIXIT.DAL\Migrations\`
? Both tables created in database
? Foreign keys properly configured
? Indexes created for performance
? Audit columns (CreatedAt, UpdatedAt) present
? Test data can be inserted successfully
? API endpoints respond correctly

---

## Next Steps

1. ? Run the migration
2. ? Verify tables in database
3. ? Insert test data (optional)
4. ? Test API endpoints using Postman or Swagger
5. ? Integrate with ServiceRequest scheduling checks
6. ? Deploy to staging/production

---

## Backup Before Migration

```sql
-- Create backup of current database
BACKUP DATABASE [FIXIT_DB] 
TO DISK = 'C:\Backups\FIXIT_DB_Before_Scheduling.bak'
WITH INIT, STATS = 10;
```

---

## Monitoring

After migration, monitor:
- Query performance on large craftsman lists
- Index usage statistics
- Foreign key constraint violations

```sql
-- Check index usage
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    s.user_updates,
    s.user_seeks,
    s.user_scans
FROM sys.indexes i
LEFT JOIN sys.dm_db_index_usage_stats s ON i.object_id = s.object_id AND i.index_id = s.index_id
WHERE OBJECT_NAME(i.object_id) IN ('CraftsManAvailabilities', 'CraftsManTimeOffs')
ORDER BY OBJECT_NAME(i.object_id);
```

---

**Migration Created:** 2025-11-15
**Status:** ? Ready to Deploy
**Rollback Plan:** Available (see Rollback section above)
