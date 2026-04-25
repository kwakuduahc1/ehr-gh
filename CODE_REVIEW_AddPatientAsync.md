# Code Review: AddPatientAsync Method
## File: ShimsServer\Repositories\RegistrationRepository.cs (Lines 25-62)

---

## 🔴 CRITICAL ISSUES

### 1. **SQL Injection Vulnerability** (Lines 33-37)
**Severity:** 🔴 CRITICAL

```csharp
foreach (var s in dto.Schemes)
    sql += $"""
        INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate)
        VALUES (gen_random_uuid(), @PatientsID, '{s}', @CardID, @ExpiryDate);
    """;
```

**Problem:**
- Direct string interpolation `'{s}'` with Guid values embedded directly in SQL
- Violates parameterized query principles
- Although GUIDs have limited attack surface, this is poor practice and breaks consistency
- Creates unmaintainable SQL with hardcoded values mixed with parameters

**Impact:** Security risk, code quality issue

**Fix:**
```csharp
// Instead of dynamic SQL string building:
// Build a single INSERT ... VALUES statement with arrays or use separate queries
```

---

### 2. **String Concatenation for Dynamic SQL** (Lines 27-41)
**Severity:** 🔴 CRITICAL

```csharp
string sql = """...""";
foreach (var s in dto.Schemes)
    sql += $"""...""";  // String concatenation
sql += $"""...""";      // More concatenation
```

**Problems:**
- String concatenation for SQL construction is error-prone
- Multiple RETURNING clauses may cause issues
- Whitespace/formatting issues in concatenated strings
- Hard to debug and maintain

**Example of the generated SQL:**
```sql
INSERT INTO Patients (...) VALUES (...) RETURNING HospitalID;
INSERT INTO PatientSchemes (...) VALUES (gen_random_uuid(), @PatientsID, 'guid-value', @CardID, @ExpiryDate);
INSERT INTO PatientSchemes (...) VALUES (gen_random_uuid(), @PatientsID, 'guid-value', @CardID, @ExpiryDate);
INSERT INTO PatientAttendances(...) VALUES (...);
```

**Impact:** Unpredictable SQL generation

---

### 3. **Improper Handling of Multiple Result Sets** (Lines 45-60)
**Severity:** 🔴 CRITICAL

```csharp
using var res = await con.QueryMultipleAsync(sql, new { ... });
var _ = await res.ReadFirstOrDefaultAsync<string>();
```

**Problems:**
- `QueryMultipleAsync` executes multiple queries and returns a `GridReader`
- Only reads from the FIRST result set (Patients INSERT RETURNING)
- Ignores all subsequent result sets (PatientSchemes INSERTs, PatientAttendances INSERT)
- GridReader must read ALL result sets before disposal, or PostgreSQL will complain
- The discarded result (`var _ = ...`) suggests incomplete implementation

**Impact:** 
- Potential connection state issues
- Query results not validated
- Silent failures if inserts don't happen

---

### 4. **Multiple INSERT Statements with Mixed Parameters** (Lines 28-41)
**Severity:** 🔴 CRITICAL

```csharp
using var res = await con.QueryMultipleAsync(sql, new
{
    ids.PatientsID,
    dto.Surname,
    // ... more parameters
    dto.Schemes,        // ❌ Array passed but used as '{s}' in interpolation
    // ...
});
```

**Problem:**
- `dto.Schemes` is an array of GUIDs but:
  - Dapper will serialize it as a single parameter
  - Used individually in the loop with string interpolation
  - Creates inconsistent parameter binding

---

### 5. **No Error Handling** (Lines 43-62)
**Severity:** 🟡 HIGH

```csharp
using var con = await connection.ConnectionAsync(cancellationToken);
using var transaction = await con.BeginTransactionAsync(cancellationToken);
using var res = await con.QueryMultipleAsync(sql, new { ... });
var _ = await res.ReadFirstOrDefaultAsync<string>();
await transaction.CommitAsync(cancellationToken);
```

**Problems:**
- No try-catch blocks
- Transaction not explicitly rolled back on error
- If any insert fails, the exception propagates but transaction might not be cleaned up properly
- No logging of errors

---

### 6. **Missing LastUpdateDate in PatientSchemes** (Line 35)
**Severity:** 🟡 HIGH

```sql
INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate)
VALUES (gen_random_uuid(), @PatientsID, '{s}', @CardID, @ExpiryDate);
```

**Problem:**
- Original schema (RegistrationRepository v1) had `LastUpdateDate` and `UserName` fields
- Current version missing these columns
- Inconsistent with `ListPatientsDto` which may expect these fields

**Should include:**
```sql
INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName, IsActive)
VALUES (gen_random_uuid(), @PatientsID, @SchemesID, now(), @UserName, true);
```

---

## 🟡 HIGH PRIORITY ISSUES

### 7. **Hardcoded VisitType** (Line 39)
**Severity:** 🟡 MEDIUM

```sql
INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen)
VALUES (@PatientAttendancesID, @PatientsID, 'Acute', @UserName, now());
```

**Problem:**
- VisitType hardcoded to 'Acute'
- Should be either configurable or derived from DTO
- No validation that this value is allowed

**Consider:**
- Add `VisitType` parameter to `AddPatientDto`
- Or validate it's a known enum value

---

### 8. **Missing IsActive Flag** (Line 35)
**Severity:** 🟡 MEDIUM

```sql
INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate)
```

**Problem:**
- PatientSchemes should have `IsActive` column (used in delete operations)
- New inserts should default to `true` or be explicitly set

---

### 9. **gen_random_uuid() vs Guid.CreateVersion7()** (Line 36)
**Severity:** 🟢 MINOR

```csharp
// In C#, already have:
ids.PatientAttendancesID,  // Provided as parameter

// But using DB function:
gen_random_uuid()  // For PatientSchemesID
```

**Observation:**
- Inconsistent: using database function for one ID, C# for another
- With .NET 10, should use `Guid.CreateVersion7()` consistently
- Database function adds unnecessary overhead

---

## ✅ WHAT'S CORRECT

- ✅ Using transactions (though not properly error-handled)
- ✅ Using `generate_hospital_id()` function
- ✅ Parameterized queries (mostly - except the Schemes loop)
- ✅ CancellationToken support
- ✅ Async/await pattern

---

## RECOMMENDED FIXES

### **Option 1: Rewrite with Separate Queries (Recommended for .NET 10)**

```csharp
public async Task AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, CancellationToken cancellationToken = default)
{
    try
    {
        using var con = await connection.ConnectionAsync(cancellationToken);
        using var transaction = await con.BeginTransactionAsync(cancellationToken);

        // Insert Patient
        const string patientSql = """
            INSERT INTO Patients (PatientID, Surname, OtherNames, DateOfBirth, GhanaCard, Sex, PhoneNumber, UserName, HospitalID)
            VALUES (@PatientsID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, @Sex, @PhoneNumber, @UserName, generate_hospital_id())
            RETURNING HospitalID;
            """;

        var hospitalId = await con.ExecuteScalarAsync<string>(patientSql, new
        {
            ids.PatientsID,
            dto.Surname,
            dto.OtherNames,
            dto.DateOfBirth,
            dto.GhanaCard,
            dto.Sex,
            dto.PhoneNumber,
            ids.UserName
        }, transaction);

        // Insert Patient Schemes
        const string schemeSql = """
            INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName, IsActive)
            VALUES (@PatientSchemesID, @PatientsID, @SchemesID, @CardID, @ExpiryDate, NOW(), @UserName, true);
            """;

        foreach (var schemeId in dto.Schemes)
        {
            await con.ExecuteAsync(schemeSql, new
            {
                PatientSchemesID = Guid.CreateVersion7(),
                ids.PatientsID,
                SchemesID = schemeId,
                dto.CardID,
                dto.ExpiryDate,
                ids.UserName
            }, transaction);
        }

        // Insert Patient Attendance
        const string attendanceSql = """
            INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen)
            VALUES (@PatientAttendancesID, @PatientsID, @VisitType, @UserName, NOW());
            """;

        await con.ExecuteAsync(attendanceSql, new
        {
            ids.PatientAttendancesID,
            ids.PatientsID,
            VisitType = "Acute",
            ids.UserName
        }, transaction);

        await transaction.CommitAsync(cancellationToken);
    }
    catch (Exception ex)
    {
        // Log error appropriately
        throw;
    }
}
```

### **Option 2: Use Multi-row INSERT with UNNEST (PostgreSQL Optimized)**

```csharp
public async Task AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, CancellationToken cancellationToken = default)
{
    try
    {
        using var con = await connection.ConnectionAsync(cancellationToken);
        using var transaction = await con.BeginTransactionAsync(cancellationToken);

        // Generate IDs for all schemes at once
        var schemesWithIds = dto.Schemes
            .Select(s => new { PatientSchemesID = Guid.CreateVersion7(), SchemesID = s })
            .ToList();

        const string sql = """
            INSERT INTO Patients (PatientID, Surname, OtherNames, DateOfBirth, GhanaCard, Sex, PhoneNumber, UserName, HospitalID)
            VALUES (@PatientsID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, @Sex, @PhoneNumber, @UserName, generate_hospital_id())
            RETURNING HospitalID;

            INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName, IsActive)
            SELECT @PatientSchemesID, @PatientsID, SchemesID, @CardID, @ExpiryDate, NOW(), @UserName, true
            FROM UNNEST(@SchemesArray) AS schemes(SchemesID);

            INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen)
            VALUES (@PatientAttendancesID, @PatientsID, 'Acute', @UserName, NOW());
            """;

        using var reader = await con.QueryMultipleAsync(sql, new
        {
            ids.PatientsID,
            dto.Surname,
            dto.OtherNames,
            dto.DateOfBirth,
            dto.GhanaCard,
            dto.Sex,
            dto.PhoneNumber,
            ids.UserName,
            PatientSchemesID = Guid.CreateVersion7(),
            dto.CardID,
            dto.ExpiryDate,
            SchemesArray = dto.Schemes,
            ids.PatientAttendancesID
        }, transaction);

        // Consume all result sets
        var hospitalId = await reader.ReadFirstOrDefaultAsync<string>();
        await reader.ReadFirstOrDefaultAsync();  // Schemes result
        await reader.ReadFirstOrDefaultAsync();  // Attendance result

        await transaction.CommitAsync(cancellationToken);
    }
    catch (Exception ex)
    {
        // Log and rethrow
        throw;
    }
}
```

---

## SUMMARY TABLE

| Issue | Severity | Category | Status |
|-------|----------|----------|--------|
| SQL Injection (GUIDs in interpolation) | 🔴 CRITICAL | Security | Not Fixed |
| String concatenation for SQL | 🔴 CRITICAL | Code Quality | Not Fixed |
| Improper GridReader handling | 🔴 CRITICAL | Logic | Not Fixed |
| Mixed parameter binding | 🔴 CRITICAL | Logic | Not Fixed |
| No error handling | 🟡 HIGH | Reliability | Not Fixed |
| Missing LastUpdateDate | 🟡 HIGH | Data Integrity | Not Fixed |
| Hardcoded VisitType | 🟡 MEDIUM | Flexibility | Not Fixed |
| Missing IsActive flag | 🟡 MEDIUM | Data Integrity | Not Fixed |
| UUID generation inconsistency | 🟢 MINOR | Consistency | Not Fixed |

---

## NEXT STEPS

1. **Immediate:** Fix SQL injection and GridReader handling
2. **Important:** Add error handling and logging
3. **Refactor:** Use Option 1 or Option 2 approach above
4. **Test:** Add unit tests for various scheme counts (0, 1, many)
5. **Validate:** Ensure all columns have proper values

