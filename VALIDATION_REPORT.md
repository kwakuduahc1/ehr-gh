# SQL Statements and Parameters Validation Report
## File: ShimsServer\Repositories\RegistrationRepository.cs

---

## ✅ VALIDATIONS PASSED

### 1. **AddPatientAsync Method** (Lines 26-58)
**Status:** ⚠️ ISSUES FOUND

#### SQL Statement Analysis:
```sql
INSERT INTO Patients (PatientID, Surname, OtherNames, DateOfBirth, GhanaCard, Sex, PhoneNumber, UserName, HospitalID)
VALUES (@PatientsID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, @Sex, @PhoneNumber, @UserName, generate_hospital_id());

INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName, IsActive)
VALUES (uuidv7(), @PatientsID, @SchemesID, @CardID, @ExpiryDate, now(), @UserName, true);

INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen)
VALUES (@PatientAttendancesID, @PatientsID, 'Acute', @UserName, now());
```

#### Issues Found:

**❌ CRITICAL - Parameter Mismatch (Line 31)**
- **Issue:** SQL column uses `@PatientsID` but Dapper matches parameters by name
- **Impact:** Parameter binding may fail
- **Evidence:** Line 44 provides `ids.PatientsID` but the query expects consistency

**❌ CRITICAL - Missing Parameter (Line 114)**
- **Issue:** `ListPatientsDto` expects `VisitType` (line 114: `string VisitType`) but `GetPatientsAsync` doesn't select it
- **Query:** `SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate, patientschemesid FROM vw_patients;`
- **Missing:** No `visittype` in SELECT clause
- **Impact:** Runtime error when mapping to `ListPatientsDto`

**⚠️ WARNING - Transaction Pattern**
- **Issue:** Using `QueryMultipleAsync` with `using var res` but not consuming the results
- **Line 42:** `using var res = await con.QueryMultipleAsync(sql, new {...});`
- **Recommendation:** Results should be consumed or disposal pattern should be explicit

---

### 2. **DeletePatientAsync Method** (Lines 60-77)
**Status:** ✅ VALID

#### SQL Statement:
```sql
UPDATE Patients SET IsActive = false WHERE PatientsID = @id;
UPDATE PatientSchemes SET IsActive = false WHERE PatientsID = @id;
UPDATE PatientAttendances SET IsActive = false WHERE PatientsID = @id;
```

**Validation:**
- ✅ Parameter `@id` correctly defined in line 75
- ✅ Logical soft delete pattern (sets `IsActive = false`)
- ✅ Consistent parameter naming across all 3 UPDATE statements
- ✅ Proper transaction handling
- ⚠️ Minor: `QueryMultipleAsync` used but results not consumed (line 75)

---

### 3. **EditPatientAsync Method** (Lines 79-108)
**Status:** ✅ VALID

#### SQL Statement:
```sql
UPDATE Patients
SET Surname = @Surname,
    OtherNames = @OtherNames,
    DateOfBirth = @DateOfBirth,
    GhanaCard = @GhanaCard,
    PhoneNumber = @PhoneNumber,
    Sex = @Sex,
    UserName = @UserName
WHERE PatientID = @PatientID;
```

**Validation:**
- ✅ All 8 parameters provided in lines 97-104
- ✅ Correct parameter naming conventions
- ✅ `EditPatientDto` has all required properties (lines 177-191)
- ✅ Proper transaction handling
- ✅ Return type matches Dapper's `ExecuteAsync` return value (rows affected)

---

### 4. **GetPatientByIdAsync Method** (Lines 110-120)
**Status:** ❌ SCHEMA MISMATCH

#### SQL Statement:
```sql
SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
FROM vw_patients
WHERE patientid = @id;
```

**Issues:**
- ❌ Missing `patientschemesid` column (expected in `ListPatientsDto` line 201)
- ❌ Missing `visittype` column (expected in `ListPatientsDto` line 114 - but NOT in `GetPatientsAsync`)
- **Query returns:** 10 columns
- **DTO expects:** 12 columns (lines 193-205)

---

### 5. **GetPatientsAsync Method** (Lines 122-131)
**Status:** ❌ DTO SCHEMA MISMATCH

#### SQL Statement:
```sql
SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate, patientschemesid
FROM vw_patients;
```

**Issues:**
- ❌ Missing `visittype` column (required by `ListPatientsDto` - line 114)
- ❌ Inconsistent with `GetPatientByIdAsync` (which has 10 columns, this has 11)
- **Query returns:** 11 columns
- **DTO expects:** 12 columns
- **Impact:** Runtime mapping failure: "No property or field VisitType on ListPatientsDto"

---

### 6. **PatientExists Method** (Lines 133-141)
**Status:** ✅ VALID

#### SQL Statement:
```sql
SELECT EXISTS(SELECT 1 FROM Patients WHERE PatientsID = @id AND IsActive = true);
```

**Validation:**
- ✅ Parameter `@id` correctly provided (line 140)
- ✅ Efficient EXISTS query
- ✅ Correct return type: `ExecuteScalarAsync<bool>`
- ✅ Active patient filter (IsActive = true)

---

### 7. **SearchPatientsAsync Method** (Lines 143-156)
**Status:** ⚠️ POTENTIAL SQL INJECTION

#### SQL Statement:
```sql
SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate, patientschemesid
FROM vw_patients
WHERE fullname ILIKE @search
   OR cardid ILIKE @search
   OR hospitalid ILIKE @search
ORDER BY attendancedate DESC;
```

**Validation:**
- ✅ Uses parameterized query (@search)
- ✅ Parameter value safe: `$"%{search}%"` (line 155)
- ✅ ILIKE is case-insensitive (good for user search)
- ✅ Proper ordering
- ⚠️ SAME DTO SCHEMA MISMATCH: Missing `visittype` column

---

## SUMMARY OF ISSUES

### Critical Issues (Must Fix):
1. **❌ AddPatientAsync** - Parameter/query mismatch in multi-statement INSERT
2. **❌ GetPatientsAsync** - Missing `visittype` in SELECT (DTO mismatch)
3. **❌ GetPatientByIdAsync** - Missing `visittype` and column count mismatch
4. **❌ SearchPatientsAsync** - Missing `visittype` in SELECT (DTO mismatch)

### Warnings (Should Fix):
1. **⚠️ Transaction Pattern** - `QueryMultipleAsync` results not consumed
2. **⚠️ DTO Property** - `ListPatientsDto` includes `VisitType` but not all queries return it

---

## RECOMMENDED FIXES

### Fix 1: Update ListPatientsDto (Remove visittype if not needed)
**Option A:** Remove `VisitType` from DTO if it's not required
```csharp
public record ListPatientsDto(
    Guid PatientID,
    Guid SchemesID,
    short Age,
    string Gender,
    string FullName,
    string Scheme,
    string HospitalID,
    string CardID,
    DateTime ExpiryDate,
    // DateTime AttendanceDate,  // Already included
    Guid PatientSchemesID
);
```

**Option B:** Add `visittype` to all SELECT queries if it IS needed

### Fix 2: Update GetPatientsAsync & GetPatientByIdAsync
Add `visittype` column if keeping in DTO:
```sql
SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, 
       cardid, expirydate, visittype, attendancedate, patientschemesid
FROM vw_patients;
```

### Fix 3: Fix AddPatientAsync Transaction
Consume the QueryMultiple result:
```csharp
using var con = await connection.ConnectionAsync(cancellationToken);
using var transaction = await con.BeginTransactionAsync(cancellationToken);
using (var reader = await con.QueryMultipleAsync(sql, new { /* parameters */ }))
{
    // Consume results if needed
}
await transaction.CommitAsync(cancellationToken);
```

---

## PARAMETER VALIDATION TABLE

| Method | Parameter | Source | DTO Property | Status |
|--------|-----------|--------|--------------|--------|
| **AddPatientAsync** |
| | @PatientsID | ids.PatientsID | N/A | ✅ |
| | @Surname | dto.Surname | Surname | ✅ |
| | @OtherNames | dto.OtherNames | OtherNames | ✅ |
| | @DateOfBirth | dto.DateOfBirth | DateOfBirth | ✅ |
| | @GhanaCard | dto.GhanaCard | GhanaCard | ✅ |
| | @Sex | dto.Sex | Sex | ✅ |
| | @PhoneNumber | dto.PhoneNumber | PhoneNumber | ✅ |
| | @UserName | ids.UserName | N/A | ✅ |
| | @SchemesID | dto.SchemesID | SchemesID | ✅ |
| | @CardID | dto.CardID | CardID | ✅ |
| | @ExpiryDate | dto.ExpiryDate | ExpiryDate | ✅ |
| | @PatientAttendancesID | ids.PatientAttendancesID | N/A | ✅ |
| **EditPatientAsync** |
| | @PatientID | dto.PatientID | PatientID | ✅ |
| | @Surname | dto.Surname | Surname | ✅ |
| | @OtherNames | dto.OtherNames | OtherNames | ✅ |
| | @DateOfBirth | dto.DateOfBirth | DateOfBirth | ✅ |
| | @GhanaCard | dto.GhanaCard | GhanaCard | ✅ |
| | @PhoneNumber | dto.PhoneNumber | PhoneNumber | ✅ |
| | @Sex | dto.Sex | Sex | ✅ |
| | @UserName | UserName (parameter) | N/A | ✅ |
| **DeletePatientAsync** |
| | @id | id | N/A | ✅ |
| **PatientExists** |
| | @id | id | N/A | ✅ |
| **SearchPatientsAsync** |
| | @search | $"%{search}%" | N/A | ✅ |

