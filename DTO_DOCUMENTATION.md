# Data Transfer Objects (DTOs) Documentation

## Overview
This document provides comprehensive documentation of all Data Transfer Objects (DTOs) and View Models (VMs) used throughout the SHIMS application. DTOs are used for transferring data between the API endpoints and clients, while VMs (View Models) are used for form submission and data binding in the application.

---

## Authentication & User Management

### 1. **LoginVm**
**Namespace:** `ShimsServer.Models.AuthVm`  
**Type:** Record  
**Purpose:** Used for user login requests

```csharp
public record LoginVm(
    [Required, StringLength(20)] string Email, 
    [Required, StringLength(15, MinimumLength = 6)] string Password);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `Email` | `string` | Required, Max 20 chars | User email address |
| `Password` | `string` | Required, 6-15 chars | User password |

**Usage:** POST `/auth/login`

---

### 2. **RegisterVm**
**Namespace:** `ShimsServer.Models.AuthVm`  
**Type:** Class  
**Purpose:** Used for user registration (users creating their own accounts)

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `Email` | `string` | Required, 5-70 chars | Email address |
| `Password` | `string` | Required, 6-30 chars | Account password |
| `ConfirmPassword` | `string` | Required, 6-30 chars, Must match Password | Password confirmation |
| `FullName` | `string` | Required, 6-75 chars | User's full name |
| `PhoneNumber` | `string` | Required, 10 chars | Phone number |
| `Title` | `string` | Required, 2-20 chars | User's title/position |

**Methods:**
- `Transform()` - Converts RegisterVm to ApplicationUser entity

**Usage:** POST `/Account/Register`

---

### 3. **GuestsVM**
**Namespace:** `ShimsServer.Models.AuthVm`  
**Type:** Class  
**Purpose:** Used for guest user registration (created by administrators)

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `UserName` | `string` | Required, 6-20 chars | Username for guest account |
| `Title` | `string` | Required, 2-10 chars | Guest's title/position |
| `Password` | `string` | Required, 6-15 chars | Account password |
| `ConfirmPassword` | `string` | Required, 6-15 chars, Must match Password | Password confirmation |
| `FullName` | `string` | Required, 5-50 chars | Guest's full name |
| `PhoneNumber` | `string` | Required, 10 chars | Phone number |
| `Institution` | `string?` | Optional, 3-150 chars | Guest's institution (if applicable) |

**Methods:**
- `Transform()` - Converts GuestsVM to ApplicationUser entity

**Usage:** POST `/auth/register-guest` (Admin only)

---

### 4. **UsersDto**
**Namespace:** `ShimsServer.Models.AuthVm`  
**Type:** Record  
**Purpose:** Data transfer object for user information (single role)

```csharp
public record UsersDto(Guid ID, string UserName, string FullName, string Role);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ID` | `Guid` | User's unique identifier |
| `UserName` | `string` | User's username/email |
| `FullName` | `string` | User's full name |
| `Role` | `string` | User's primary role |

**Usage:** GET `/users/{id}`, User listings

---

### 5. **UsersDtoVw**
**Namespace:** `ShimsServer.Models.AuthVm`  
**Type:** Record  
**Purpose:** Data transfer object for user information with multiple roles

```csharp
public record UsersDtoVw(Guid ID, string UserName, string FullName, string[] Roles);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ID` | `Guid` | User's unique identifier |
| `UserName` | `string` | User's username/email |
| `FullName` | `string` | User's full name |
| `Roles` | `string[]` | Array of user's roles |

**Usage:** GET `/users`, Comprehensive user listings with all assigned roles

---

## Insurance Schemes

### 6. **SchemesDTO**
**Namespace:** `ShimsServer.EndPoints`  
**Type:** Record  
**Purpose:** Data transfer object for insurance scheme information

```csharp
public record SchemesDTO(Guid SchemesID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemesID` | `Guid` | Unique identifier for the scheme |
| `SchemeName` | `string` | Name of the insurance scheme (e.g., "NHIS", "Private") |
| `Coverage` | `string` | Coverage type: "Full", "Relative", or "Fixed" |
| `MaxPayable` | `decimal` | Maximum amount the scheme will pay |
| `Recovery` | `decimal` | Recovery percentage (1-100) |

**Usage:** GET `/schemes`, GET `/schemes/{id}`

---

### 7. **AddSchemeDto**
**Namespace:** `ShimsServer.EndPoints`  
**Type:** Record  
**Purpose:** Data transfer object for creating new insurance schemes

```csharp
public record AddSchemeDto(string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeName` | `string` | Name of the new scheme |
| `Coverage` | `string` | Coverage type: "Full", "Relative", or "Fixed" |
| `MaxPayable` | `decimal` | Maximum payable amount |
| `Recovery` | `decimal` | Recovery percentage |

**Validation:**
- Scheme name must be unique (409 Conflict if duplicate)
- Coverage must be one of: "Full", "Relative", "Fixed"
- MaxPayable must be >= 0
- Recovery must be between 1-100

**Usage:** POST `/schemes`

---

### 8. **UpdateSchemeDto**
**Namespace:** `ShimsServer.EndPoints`  
**Type:** Record  
**Purpose:** Data transfer object for updating insurance schemes

```csharp
public record UpdateSchemeDto(Guid ID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ID` | `Guid` | Scheme identifier to update |
| `SchemeName` | `string` | Updated scheme name |
| `Coverage` | `string` | Updated coverage type |
| `MaxPayable` | `decimal` | Updated maximum payable amount |
| `Recovery` | `decimal` | Updated recovery percentage |

**Validation:**
- Scheme must exist (404 Not Found if doesn't exist)
- New name must be unique unless it's the same scheme (409 Conflict if duplicate with another scheme)
- Recovery must be between 1-100

**Usage:** PUT `/schemes/{id}`

---

## Consulting Room

### 9. **PendingVm**
**Namespace:** `ShimsServer.Models.ConsultingRoom`  
**Type:** Class  
**Purpose:** View model for pending patient consultations

**Properties:**
| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `FullName` | `string` | Yes | Patient's full name |
| `OPDNumber` | `string` | Yes | Out-patient department number |
| `Gender` | `string` | Yes | Patient's gender (M/F) |
| `Age` | `int` | Yes | Patient's age in years |
| `PatientsID` | `Guid` | No | Reference to patient record |
| `Systolic` | `double?` | No | Systolic blood pressure reading |
| `Diastolic` | `double?` | No | Diastolic blood pressure reading |
| `Weight` | `double?` | No | Patient's weight in kg |
| `Pulse` | `double?` | No | Patient's pulse rate (bpm) |
| `Temperature` | `double?` | No | Patient's body temperature (°C) |
| `Respiration` | `double?` | No | Patient's respiration rate (breaths/min) |
| `History` | `string?` | No | Patient's medical history notes |

**Usage:** Tracking pending consultations, vital signs entry

---

## Scheme Pricing (Drugs, Labs, Services)

### 9. **SchemeDrugDTO**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme drug pricing information

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeDrugsID` | `Guid` | Unique identifier for scheme drug pricing |
| `SchemesID` | `Guid` | Insurance scheme reference |
| `DrugsID` | `Guid` | Drug reference |
| `SchemeName` | `string` | Scheme name for display |
| `DrugName` | `string` | Drug name for display |
| `Price` | `decimal` | Drug price under this scheme |
| `DateSet` | `DateTime` | Date pricing was set |

**Usage:** GET `/schemes/{schemeId}/drugs`, GET `/schemes/{schemeId}/drugs/{id}`

---

### 10. **AddSchemeDrugDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for creating scheme drug pricing

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `SchemesID` | `Guid` | Required | Scheme identifier |
| `DrugsID` | `Guid` | Required | Drug identifier |
| `Price` | `decimal` | Required, >= 0.5 | Drug price |

**Usage:** POST `/schemes/{schemeId}/drugs`

---

### 11. **UpdateSchemeDrugDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for updating scheme drug pricing

**Usage:** PUT `/schemes/{schemeId}/drugs/{id}`

---

### 12. **SchemeDrugAvailabilityDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme drugs with availability status

**Usage:** GET `/schemes/{schemeId}/drugs/availability`

---

### 13. **SchemeLabDTO**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme lab pricing information

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeLabsID` | `Guid` | Unique identifier for scheme lab pricing |
| `SchemesID` | `Guid` | Insurance scheme reference |
| `LabsGroupID` | `Guid` | Lab group reference |
| `SchemeName` | `string` | Scheme name for display |
| `LabGroupName` | `string` | Lab group name for display |
| `Price` | `decimal` | Lab test price under this scheme |
| `DateSet` | `DateTime` | Date pricing was set |

**Usage:** GET `/schemes/{schemeId}/labs`, GET `/schemes/{schemeId}/labs/{id}`

---

### 14. **AddSchemeLabDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for creating scheme lab pricing

**Usage:** POST `/schemes/{schemeId}/labs`

---

### 15. **UpdateSchemeLabDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for updating scheme lab pricing

**Usage:** PUT `/schemes/{schemeId}/labs/{id}`

---

### 16. **SchemeLabAvailabilityDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme labs with available tests

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeLabsID` | `Guid` | Scheme lab ID |
| `SchemeName` | `string` | Scheme name |
| `LabGroupName` | `string` | Lab group name |
| `Price` | `decimal` | Lab test price |
| `AvailableTests` | `int` | Number of available test parameters |

**Usage:** GET `/schemes/{schemeId}/labs/availability`

---

### 17. **SchemeServiceDTO**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme service pricing information

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeServicesID` | `Guid` | Unique identifier for scheme service pricing |
| `SchemesID` | `Guid` | Insurance scheme reference |
| `ServicesID` | `Guid` | Service reference |
| `SchemeName` | `string` | Scheme name for display |
| `ServiceName` | `string` | Service name for display |
| `Price` | `decimal` | Service price under this scheme |
| `DateSet` | `DateTime` | Date pricing was set |

**Usage:** GET `/schemes/{schemeId}/services`, GET `/schemes/{schemeId}/services/{id}`

---

### 18. **AddSchemeServiceDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for creating scheme service pricing

**Usage:** POST `/schemes/{schemeId}/services`

---

### 19. **UpdateSchemeServiceDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for updating scheme service pricing

**Usage:** PUT `/schemes/{schemeId}/services/{id}`

---

### 20. **SchemeServiceAvailabilityDto**
**Namespace:** `ShimsServer.Models.Schemes`  
**Type:** Record  
**Purpose:** Data transfer object for scheme services with coverage status

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `SchemeServicesID` | `Guid` | Scheme service ID |
| `SchemeName` | `string` | Scheme name |
| `ServiceName` | `string` | Service name |
| `ServiceGroup` | `string` | Service category/group |
| `Price` | `decimal` | Service price |
| `IsCovered` | `bool` | Coverage status |

**Usage:** GET `/schemes/{schemeId}/services/coverage`

---

## Services Management

### 10. **ServicesDTO**
**Namespace:** `ShimsServer.Models.Services`  
**Type:** Record  
**Purpose:** Data transfer object for service information

```csharp
public record ServicesDTO(int ServicesID, string Service, string ServiceGroup);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ServicesID` | `int` | Unique identifier for the service |
| `Service` | `string` | Service name (e.g., "Consultation", "Surgery") |
| `ServiceGroup` | `string` | Service grouping/category |

**Usage:** GET `/services`, GET `/services/{id}`

---

### 11. **AddServiceDto**
**Namespace:** `ShimsServer.Models.Services`  
**Type:** Record  
**Purpose:** Data transfer object for creating new services

```csharp
public record AddServiceDto(
    [Required, StringLength(100)] string Service,
    [Required, StringLength(50)] string ServiceGroup);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `Service` | `string` | Required, Max 100 chars | Service name |
| `ServiceGroup` | `string` | Required, Max 50 chars | Service category |

**Usage:** POST `/services`

---

### 12. **UpdateServiceDto**
**Namespace:** `ShimsServer.Models.Services`  
**Type:** Record  
**Purpose:** Data transfer object for updating services

```csharp
public record UpdateServiceDto(
    int ServicesID,
    [Required, StringLength(100)] string Service,
    [Required, StringLength(50)] string ServiceGroup);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `ServicesID` | `int` | Required | Service identifier to update |
| `Service` | `string` | Required, Max 100 chars | Updated service name |
| `ServiceGroup` | `string` | Required, Max 50 chars | Updated service category |

**Usage:** PUT `/services/{id}`

---

### 13. **ServiceWithCoverageDto**
**Namespace:** `ShimsServer.Models.Services`  
**Type:** Record  
**Purpose:** Data transfer object for service with scheme coverage information

```csharp
public record ServiceWithCoverageDto(
    int ServicesID,
    string Service,
    string ServiceGroup,
    decimal? Price,
    string[] SchemesCovered);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ServicesID` | `int` | Service identifier |
| `Service` | `string` | Service name |
| `ServiceGroup` | `string` | Service category |
| `Price` | `decimal?` | Service cost (if applicable) |
| `SchemesCovered` | `string[]` | Array of scheme names that cover this service |

**Usage:** GET `/services/{id}/coverage`, Service coverage lookup

---

## Drugs Management

### 14. **DrugDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug information

```csharp
public record DrugDTO(Guid DrugsID, string Drug, string? Tags, string? Description, DateTime DateAdded);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsID` | `Guid` | Unique identifier for the drug |
| `Drug` | `string` | Drug name/description |
| `Tags` | `string?` | Searchable tags for the drug |
| `Description` | `string?` | Detailed drug description |
| `DateAdded` | `DateTime` | Date drug was added to system |

**Usage:** GET `/drugs`, GET `/drugs/{id}`

---

### 15. **AddDrugDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for creating new drugs

```csharp
public record AddDrugDto(
    [Required, StringLength(150)] string Drug,
    [StringLength(100)] string? Description,
    string? Tags);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `Drug` | `string` | Required, Max 150 chars | Drug name |
| `Description` | `string?` | Optional, Max 100 chars | Drug description |
| `Tags` | `string?` | Optional | Comma-separated tags for searching |

**Usage:** POST `/drugs`

---

### 16. **UpdateDrugDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for updating drugs

```csharp
public record UpdateDrugDto(
    Guid DrugsID,
    [Required, StringLength(150)] string Drug,
    [StringLength(100)] string? Description,
    string? Tags);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugsID` | `Guid` | Required | Drug identifier to update |
| `Drug` | `string` | Required, Max 150 chars | Updated drug name |
| `Description` | `string?` | Optional, Max 100 chars | Updated drug description |
| `Tags` | `string?` | Optional | Updated search tags |

**Usage:** PUT `/drugs/{id}`

---

### 17. **DrugStockDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug stock information

```csharp
public record DrugStockDTO(Guid DrugsStockID, Guid DrugsID, string DrugName, short Quantity, DateTime TranDate);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsStockID` | `Guid` | Unique identifier for stock entry |
| `DrugsID` | `Guid` | Reference to drug |
| `DrugName` | `string` | Drug name for display |
| `Quantity` | `short` | Stock quantity available |
| `TranDate` | `DateTime` | Transaction/update date |

**Usage:** GET `/drugs/{id}/stock`, Stock inventory lookup

---

### 18. **AddDrugStockDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for adding drug stock

```csharp
public record AddDrugStockDto(
    [Required] Guid DrugsID,
    [Required, Range(1, short.MaxValue)] short Quantity);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugsID` | `Guid` | Required | Drug identifier |
| `Quantity` | `short` | Required, > 0 | Stock quantity to add |

**Usage:** POST `/drugs/{id}/stock`

---

### 19. **UpdateDrugStockDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for updating drug stock

```csharp
public record UpdateDrugStockDto(
    Guid DrugsStockID,
    [Required, Range(1, short.MaxValue)] short Quantity);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugsStockID` | `Guid` | Required | Stock entry identifier |
| `Quantity` | `short` | Required, > 0 | Updated stock quantity |

**Usage:** PUT `/drugs/stock/{id}`

---

### 20. **DrugWithStockDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug with current stock information

```csharp
public record DrugWithStockDto(
    Guid DrugsID,
    string Drug,
    string? Description,
    short CurrentStock,
    int LowStockThreshold);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsID` | `Guid` | Drug identifier |
| `Drug` | `string` | Drug name |
| `Description` | `string?` | Drug description |
| `CurrentStock` | `short` | Current available quantity |
| `LowStockThreshold` | `int` | Quantity threshold for alerts |

**Usage:** GET `/drugs/inventory`, Inventory management dashboard

---

### 21. **DrugRequestDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug request information

```csharp
public record DrugRequestDTO(
    Guid DrugsRequestsID,
    Guid PatientsAttendancesID,
    Guid SchemeDrugsID,
    string DrugName,
    byte Frequency,
    byte Days,
    byte QuantityRequested,
    DateTime DateRequested,
    bool IsPaid,
    bool IsDispensed);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsRequestsID` | `Guid` | Unique request identifier |
| `PatientsAttendancesID` | `Guid` | Patient attendance reference |
| `SchemeDrugsID` | `Guid` | Scheme drug reference |
| `DrugName` | `string` | Name of requested drug |
| `Frequency` | `byte` | Times per day (1-6) |
| `Days` | `byte` | Duration in days (1-200) |
| `QuantityRequested` | `byte` | Quantity requested (0-200) |
| `DateRequested` | `DateTime` | Request date |
| `IsPaid` | `bool` | Payment status |
| `IsDispensed` | `bool` | Dispensing status |

**Usage:** GET `/drugs/requests`, GET `/drugs/requests/{id}`

---

### 22. **AddDrugRequestDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for creating drug requests

```csharp
public record AddDrugRequestDto(
    [Required] Guid PatientsAttendancesID,
    [Required] Guid SchemeDrugsID,
    [Required, Range(1, 6)] byte Frequency,
    [Required, Range(1, 200)] byte Days,
    [Required, Range(0, 200)] byte QuantityRequested);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientsAttendancesID` | `Guid` | Required | Patient attendance reference |
| `SchemeDrugsID` | `Guid` | Required | Scheme drug reference |
| `Frequency` | `byte` | Required, 1-6 | Times per day |
| `Days` | `byte` | Required, 1-200 | Treatment duration |
| `QuantityRequested` | `byte` | Required, 0-200 | Quantity needed |

**Usage:** POST `/drugs/requests`

---

### 23. **DrugRequestSummaryDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug request summary with patient information

```csharp
public record DrugRequestSummaryDto(
    Guid DrugsRequestsID,
    string PatientName,
    string DrugName,
    byte Frequency,
    byte Days,
    byte QuantityRequested,
    DateTime DateRequested,
    bool IsPaid,
    bool IsDispensed);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsRequestsID` | `Guid` | Request identifier |
| `PatientName` | `string` | Patient name for display |
| `DrugName` | `string` | Drug name for display |
| `Frequency` | `byte` | Times per day |
| `Days` | `byte` | Treatment duration |
| `QuantityRequested` | `byte` | Requested quantity |
| `DateRequested` | `DateTime` | Request date |
| `IsPaid` | `bool` | Payment status |
| `IsDispensed` | `bool` | Dispensing status |

**Usage:** GET `/drugs/requests/summary`, Patient drug request tracking

---

### 24. **DispensingCalculationDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for dispensing calculations

```csharp
public record DispensingCalculationDTO(
    Guid DrugsRequestsID,
    byte Quantity,
    DateTime DateDone,
    string UserName,
    string? Notes);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsRequestsID` | `Guid` | Reference to drug request |
| `Quantity` | `byte` | Calculated quantity to dispense |
| `DateDone` | `DateTime` | Calculation date |
| `UserName` | `string` | Pharmacist/technician name |
| `Notes` | `string?` | Additional notes |

**Usage:** GET `/drugs/dispensing-calculations/{id}`

---

### 25. **AddDispensingCalculationDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for creating dispensing calculations

```csharp
public record AddDispensingCalculationDto(
    [Required] Guid DrugsRequestsID,
    [Required, Range(0, 100)] byte Quantity,
    [StringLength(150, MinimumLength = 10)] string? Notes);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugsRequestsID` | `Guid` | Required | Drug request reference |
| `Quantity` | `byte` | Required, 0-100 | Calculated dispensing quantity |
| `Notes` | `string?` | Optional, 10-150 chars | Clinical notes |

**Usage:** POST `/drugs/dispensing-calculations`

---

### 26. **UpdateDispensingCalculationDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for updating dispensing calculations

```csharp
public record UpdateDispensingCalculationDto(
    Guid DrugsRequestsID,
    [Required, Range(0, 100)] byte Quantity,
    [StringLength(150, MinimumLength = 10)] string? Notes);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugsRequestsID` | `Guid` | Required | Drug request reference |
| `Quantity` | `byte` | Required, 0-100 | Updated quantity |
| `Notes` | `string?` | Optional, 10-150 chars | Updated notes |

**Usage:** PUT `/drugs/dispensing-calculations/{id}`

---

### 27. **DrugPaymentDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug payment information

```csharp
public record DrugPaymentDTO(
    Guid DispensingCaculationsID,
    string Receipt,
    byte QuantityPaid,
    decimal Amount,
    DateTime? DatePaid,
    Guid? PaymentTypesID,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DispensingCaculationsID` | `Guid` | Reference to dispensing calculation |
| `Receipt` | `string` | Payment receipt number |
| `QuantityPaid` | `byte` | Quantity paid for |
| `Amount` | `decimal` | Payment amount |
| `DatePaid` | `DateTime?` | Payment date |
| `PaymentTypesID` | `Guid?` | Payment method reference |
| `UserName` | `string` | Cashier/operator name |

**Usage:** GET `/drugs/payments`, GET `/drugs/payments/{id}`

---

### 28. **AddDrugPaymentDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for creating drug payments

```csharp
public record AddDrugPaymentDto(
    [Required] Guid DispensingCaculationsID,
    [Required, StringLength(20, MinimumLength = 8)] string Receipt,
    [Required, Range(1, 255)] byte QuantityPaid,
    [Required, Range(0, double.MaxValue)] decimal Amount,
    Guid? PaymentTypesID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DispensingCaculationsID` | `Guid` | Required | Dispensing calculation reference |
| `Receipt` | `string` | Required, 8-20 chars | Receipt/invoice number |
| `QuantityPaid` | `byte` | Required, 1-255 | Quantity paid |
| `Amount` | `decimal` | Required, >= 0 | Payment amount |
| `PaymentTypesID` | `Guid?` | Optional | Payment method ID |

**Usage:** POST `/drugs/payments`

---

### 29. **UpdateDrugPaymentDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for updating drug payments

```csharp
public record UpdateDrugPaymentDto(
    Guid DispensingCaculationsID,
    [Required, StringLength(20, MinimumLength = 8)] string Receipt,
    [Required, Range(1, 255)] byte QuantityPaid,
    [Required, Range(0, double.MaxValue)] decimal Amount,
    Guid? PaymentTypesID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DispensingCaculationsID` | `Guid` | Required | Dispensing calculation reference |
| `Receipt` | `string` | Required, 8-20 chars | Updated receipt number |
| `QuantityPaid` | `byte` | Required, 1-255 | Updated quantity |
| `Amount` | `decimal` | Required, >= 0 | Updated amount |
| `PaymentTypesID` | `Guid?` | Optional | Updated payment method |

**Usage:** PUT `/drugs/payments/{id}`

---

### 30. **DrugPaymentDetailedDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for drug payment with dispensing details

```csharp
public record DrugPaymentDetailedDto(
    Guid DispensingCaculationsID,
    string Receipt,
    byte QuantityPaid,
    decimal Amount,
    DateTime? DatePaid,
    byte QuantityDispensed,
    DateTime DateDispensed,
    string DispensingUserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DispensingCaculationsID` | `Guid` | Dispensing calculation reference |
| `Receipt` | `string` | Receipt number |
| `QuantityPaid` | `byte` | Quantity paid |
| `Amount` | `decimal` | Payment amount |
| `DatePaid` | `DateTime?` | Payment date |
| `QuantityDispensed` | `byte` | Actual quantity dispensed |
| `DateDispensed` | `DateTime` | Dispensing date |
| `DispensingUserName` | `string` | Pharmacist who dispensed |

**Usage:** GET `/drugs/payments/{id}/details`, Payment and dispensing summary

---

### 31. **DispensingDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for dispensing information

```csharp
public record DispensingDTO(
    Guid DrugPaymentsID,
    DateTime DateDispensed,
    byte QuantityDispensed,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugPaymentsID` | `Guid` | Reference to drug payment |
| `DateDispensed` | `DateTime` | Dispensing date |
| `QuantityDispensed` | `byte` | Quantity dispensed |
| `UserName` | `string` | Pharmacist/technician name |

**Usage:** GET `/drugs/dispensing/{id}`

---

### 32. **AddDispensingDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for creating dispensing records

```csharp
public record AddDispensingDto(
    [Required] Guid DrugPaymentsID,
    [Required, Range(1, 100)] byte QuantityDispensed);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugPaymentsID` | `Guid` | Required | Drug payment reference |
| `QuantityDispensed` | `byte` | Required, 1-100 | Quantity to dispense |

**Usage:** POST `/drugs/dispensing`

---

### 33. **UpdateDispensingDto**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for updating dispensing records

```csharp
public record UpdateDispensingDto(
    Guid DrugPaymentsID,
    [Required, Range(1, 100)] byte QuantityDispensed);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `DrugPaymentsID` | `Guid` | Required | Drug payment reference |
| `QuantityDispensed` | `byte` | Required, 1-100 | Updated quantity |

**Usage:** PUT `/drugs/dispensing/{id}`

---

### 34. **DrugWorkflowDTO**
**Namespace:** `ShimsServer.Models.Drugs`  
**Type:** Record  
**Purpose:** Data transfer object for complete drug workflow (request to dispensing)

```csharp
public record DrugWorkflowDTO(
    Guid DrugsRequestsID,
    string DrugName,
    byte QuantityRequested,
    byte QuantityCalculated,
    byte QuantityDispensed,
    decimal PaymentAmount,
    string Status);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `DrugsRequestsID` | `Guid` | Drug request identifier |
| `DrugName` | `string` | Drug name |
| `QuantityRequested` | `byte` | Original requested quantity |
| `QuantityCalculated` | `byte` | Calculated dispensing quantity |
| `QuantityDispensed` | `byte` | Actually dispensed quantity |
| `PaymentAmount` | `decimal` | Payment amount |
| `Status` | `string` | Current status: Requested, Calculated, Paid, Dispensed |

**Usage:** GET `/drugs/requests/{id}/workflow`, Drug request tracking from request to dispensing

---

## Laboratory Management

### 35. **LabRequestDTO**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab request information

```csharp
public record LabRequestDTO(
    Guid LabRequestsID,
    Guid PatientsAttendancesID,
    Guid SchemeLabsID,
    string PatientName,
    string LabGroupName,
    DateTime DateRequested,
    string UserName,
    bool IsPaid);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabRequestsID` | `Guid` | Unique identifier for lab request |
| `PatientsAttendancesID` | `Guid` | Patient attendance reference |
| `SchemeLabsID` | `Guid` | Scheme lab reference |
| `PatientName` | `string` | Patient name for display |
| `LabGroupName` | `string` | Lab group name (e.g., Hematology) |
| `DateRequested` | `DateTime` | Request date |
| `UserName` | `string` | Requesting doctor/staff |
| `IsPaid` | `bool` | Payment status |

**Usage:** GET `/labs/requests`, GET `/labs/requests/{id}`

---

### 36. **AddLabRequestDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for creating lab requests

```csharp
public record AddLabRequestDto(
    [Required] Guid PatientsAttendancesID,
    [Required] Guid SchemeLabsID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientsAttendancesID` | `Guid` | Required | Patient attendance reference |
| `SchemeLabsID` | `Guid` | Required | Scheme lab reference |

**Usage:** POST `/labs/requests`

---

### 37. **LabRequestSummaryDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab request summary with patient information

```csharp
public record LabRequestSummaryDto(
    Guid LabRequestsID,
    string PatientName,
    string LabGroupName,
    DateTime DateRequested,
    string UserName,
    bool IsPaid);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabRequestsID` | `Guid` | Lab request identifier |
| `PatientName` | `string` | Patient name for display |
| `LabGroupName` | `string` | Lab group name |
| `DateRequested` | `DateTime` | Request date |
| `UserName` | `string` | Requesting user |
| `IsPaid` | `bool` | Payment status |

**Usage:** GET `/labs/requests/summary`, Lab request tracking

---

### 38. **LabPaymentDTO**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab payment information

```csharp
public record LabPaymentDTO(
    Guid LabRequestsID,
    string Receipt,
    decimal Amount,
    DateTime? DatePaid,
    Guid? PaymentTypesID,
    string? PaymentReceiver,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabRequestsID` | `Guid` | Reference to lab request |
| `Receipt` | `string` | Payment receipt number |
| `Amount` | `decimal` | Payment amount |
| `DatePaid` | `DateTime?` | Payment date |
| `PaymentTypesID` | `Guid?` | Payment method reference |
| `PaymentReceiver` | `string?` | Person who received payment |
| `UserName` | `string` | User who processed payment |

**Usage:** GET `/labs/payments`, GET `/labs/payments/{id}`

---

### 39. **AddLabPaymentDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for creating lab payments

```csharp
public record AddLabPaymentDto(
    [Required] Guid LabRequestsID,
    [Required, StringLength(20, MinimumLength = 8)] string Receipt,
    [Required, Range(0.0, double.MaxValue)] decimal Amount,
    Guid? PaymentTypesID,
    [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabRequestsID` | `Guid` | Required | Lab request reference |
| `Receipt` | `string` | Required, 8-20 chars | Receipt/invoice number |
| `Amount` | `decimal` | Required, >= 0 | Payment amount |
| `PaymentTypesID` | `Guid?` | Optional | Payment method ID |
| `PaymentReceiver` | `string?` | Optional, 10-75 chars | Payment receiver name |

**Usage:** POST `/labs/payments`

---

### 40. **UpdateLabPaymentDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for updating lab payments

```csharp
public record UpdateLabPaymentDto(
    Guid LabRequestsID,
    [Required, StringLength(20, MinimumLength = 8)] string Receipt,
    [Required, Range(0.0, double.MaxValue)] decimal Amount,
    Guid? PaymentTypesID,
    [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabRequestsID` | `Guid` | Required | Lab request reference |
| `Receipt` | `string` | Required, 8-20 chars | Updated receipt number |
| `Amount` | `decimal` | Required, >= 0 | Updated amount |
| `PaymentTypesID` | `Guid?` | Optional | Updated payment method |
| `PaymentReceiver` | `string?` | Optional, 10-75 chars | Updated receiver name |

**Usage:** PUT `/labs/payments/{id}`

---

### 41. **LabPaymentDetailedDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab payment with request details

```csharp
public record LabPaymentDetailedDto(
    Guid LabRequestsID,
    string PatientName,
    string LabGroupName,
    string Receipt,
    decimal Amount,
    DateTime? DatePaid,
    string UserName,
    DateTime DateRequested);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabRequestsID` | `Guid` | Lab request identifier |
| `PatientName` | `string` | Patient name for display |
| `LabGroupName` | `string` | Lab group that will perform tests |
| `Receipt` | `string` | Payment receipt |
| `Amount` | `decimal` | Payment amount |
| `DatePaid` | `DateTime?` | Payment date |
| `UserName` | `string` | User who processed |
| `DateRequested` | `DateTime` | Original request date |

**Usage:** GET `/labs/payments/{id}/details`, Lab payment with request details

---

### 42. **LabGroupDTO**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab group information

```csharp
public record LabGroupDTO(Guid LabGroupsID, string LabGroup, string? LabDescription);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabGroupsID` | `Guid` | Unique identifier for lab group |
| `LabGroup` | `string` | Lab group name (e.g., "Hematology", "Chemistry") |
| `LabDescription` | `string?` | Lab group description |

**Usage:** GET `/labs/groups`, GET `/labs/groups/{id}`

---

### 43. **AddLabGroupDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for creating new lab groups

```csharp
public record AddLabGroupDto(
    [Required, StringLength(50, MinimumLength = 3)] string LabGroup,
    [StringLength(100, MinimumLength = 2)] string? LabDescription);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabGroup` | `string` | Required, 3-50 chars | Lab group name |
| `LabDescription` | `string?` | Optional, 2-100 chars | Lab group description |

**Usage:** POST `/labs/groups`

---

### 44. **UpdateLabGroupDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for updating lab groups

```csharp
public record UpdateLabGroupDto(
    Guid LabGroupsID,
    [Required, StringLength(50, MinimumLength = 3)] string LabGroup,
    [StringLength(100, MinimumLength = 2)] string? LabDescription);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabGroupsID` | `Guid` | Required | Lab group identifier to update |
| `LabGroup` | `string` | Required, 3-50 chars | Updated lab group name |
| `LabDescription` | `string?` | Optional, 2-100 chars | Updated description |

**Usage:** PUT `/labs/groups/{id}`

---

### 45. **LabParameterDTO**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab parameter information

```csharp
public record LabParameterDTO(Guid LabParametersID, string LabParameter, short Order, Guid LabGroupsID, string LabGroupName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabParametersID` | `Guid` | Unique identifier for parameter |
| `LabParameter` | `string` | Parameter name (e.g., "WBC", "RBC", "Hemoglobin") |
| `Order` | `short` | Display order (0-100) |
| `LabGroupsID` | `Guid` | Parent lab group identifier |
| `LabGroupName` | `string` | Parent lab group name for display |

**Usage:** GET `/labs/parameters`, GET `/labs/groups/{id}/parameters`

---

### 46. **AddLabParameterDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for creating lab parameters

```csharp
public record AddLabParameterDto(
    [Required, StringLength(50, MinimumLength = 3)] string LabParameter,
    [Required, Range(0, 100)] short Order,
    [Required] int LabGroupsID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabParameter` | `string` | Required, 3-50 chars | Parameter name |
| `Order` | `short` | Required, 0-100 | Display order |
| `LabGroupsID` | `int` | Required | Parent lab group ID |

**Usage:** POST `/labs/parameters`

---

### 47. **UpdateLabParameterDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for updating lab parameters

```csharp
public record UpdateLabParameterDto(
    Guid LabParametersID,
    [Required, StringLength(50, MinimumLength = 3)] string LabParameter,
    [Required, Range(0, 100)] short Order);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabParametersID` | `Guid` | Required | Parameter identifier to update |
| `LabParameter` | `string` | Required, 3-50 chars | Updated parameter name |
| `Order` | `short` | Required, 0-100 | Updated display order |

**Usage:** PUT `/labs/parameters/{id}`

---

### 48. **LabResultDTO**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab results

```csharp
public record LabResultDTO(
    Guid LabPaymentID,
    Guid LabParametersID,
    string LabParameterName,
    string Result,
    string? Notes,
    DateTime DateTested,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabPaymentID` | `Guid` | Reference to lab payment |
| `LabParametersID` | `Guid` | Lab parameter tested |
| `LabParameterName` | `string` | Parameter name for display |
| `Result` | `string` | Test result value |
| `Notes` | `string?` | Additional clinical notes |
| `DateTested` | `DateTime` | Date test was performed |
| `UserName` | `string` | Technician/user who performed test |

**Usage:** GET `/labs/results`, GET `/labs/payment/{id}/results`

---

### 49. **AddLabResultDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for adding lab results

```csharp
public record AddLabResultDto(
    [Required] Guid LabPaymentID,
    [Required] Guid LabParametersID,
    [Required, StringLength(50, MinimumLength = 1)] string Result,
    [StringLength(500, MinimumLength = 2)] string? Notes);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `LabPaymentID` | `Guid` | Required | Lab payment reference |
| `LabParametersID` | `Guid` | Required | Lab parameter tested |
| `Result` | `string` | Required, 1-50 chars | Test result |
| `Notes` | `string?` | Optional, 2-500 chars | Clinical notes |

**Usage:** POST `/labs/results`

---

### 50. **LabResultWithPaymentDto**
**Namespace:** `ShimsServer.Models.Labs`  
**Type:** Record  
**Purpose:** Data transfer object for lab results with payment information

```csharp
public record LabResultWithPaymentDto(
    Guid LabPaymentID,
    string LabGroupName,
    LabResultDTO[] Results,
    decimal TotalCost,
    DateTime DateCreated);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `LabPaymentID` | `Guid` | Lab payment identifier |
| `LabGroupName` | `string` | Lab group that performed tests |
| `Results` | `LabResultDTO[]` | Array of individual test results |
| `TotalCost` | `decimal` | Total cost of all tests |
| `DateCreated` | `DateTime` | Date payment/tests were created |

**Usage:** GET `/labs/results/{id}/summary`, Comprehensive lab results with billing

---

## Patient Records Management

### 51. **PatientDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for patient information (inferred from context)

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `PatientsID` | `Guid` | Unique patient identifier |
| `HospitalID` | `string` | Hospital assigned patient ID |
| `Surname` | `string` | Patient's surname |
| `OtherNames` | `string` | Patient's other names |
| `DateOfBirth` | `DateTime` | Date of birth |
| `Sex` | `string` | Gender (Male/Female) |
| `PhoneNumber` | `string` | Contact phone number |
| `UserName` | `string` | Staff member who created record |

**Usage:** GET `/patients`, GET `/patients/{id}`

---

### 52. **AddPatientDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for creating new patient records

```csharp
public record AddPatientDto(
    [Required] string HospitalID,
    [Required, StringLength(30, MinimumLength = 3)] string Surname,
    [Required, StringLength(50, MinimumLength = 3)] string OtherNames,
    [Required] DateTime DateOfBirth,
    [Required, StringLength(6, MinimumLength = 4)] [AllowedValues("Male", "Female")] string Sex,
    [Required, DataType(DataType.PhoneNumber)] string PhoneNumber);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `HospitalID` | `string` | Required | Hospital assigned patient ID |
| `Surname` | `string` | Required, 3-30 chars | Patient's surname |
| `OtherNames` | `string` | Required, 3-50 chars | Patient's other names |
| `DateOfBirth` | `DateTime` | Required | Date of birth |
| `Sex` | `string` | Required, Male/Female | Patient's gender |
| `PhoneNumber` | `string` | Required, Phone format | Contact number |

**Usage:** POST `/patients`

---

### 53. **EditPatientDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for updating patient records

```csharp
public record EditPatientDto(
    [Required] Guid PatientID,
    [Required] string HospitalID,
    [Required, StringLength(30, MinimumLength = 3)] string Surname,
    [Required, StringLength(50, MinimumLength = 3)] string OtherNames,
    [Required] DateTime DateOfBirth,
    [Required, StringLength(6, MinimumLength = 4)] [AllowedValues("Male", "Female")] string Sex,
    [Required, DataType(DataType.PhoneNumber)] string PhoneNumber);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientID` | `Guid` | Required | Patient identifier to update |
| `HospitalID` | `string` | Required | Updated hospital ID |
| `Surname` | `string` | Required, 3-30 chars | Updated surname |
| `OtherNames` | `string` | Required, 3-50 chars | Updated other names |
| `DateOfBirth` | `DateTime` | Required | Updated date of birth |
| `Sex` | `string` | Required, Male/Female | Updated gender |
| `PhoneNumber` | `string` | Required, Phone format | Updated phone number |

**Usage:** PUT `/patients/{id}`

---

### 54. **AttendanceDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for creating patient attendance records

```csharp
public record AttendanceDto(
    [Required] Guid PatientSchemesID,
    [StringLength(15)] [DefaultValue("Acute")] [AllowedValues(["Acute", "Review", "Follow-up"])] string VisitType);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientSchemesID` | `Guid` | Required | Patient scheme reference |
| `VisitType` | `string` | Required, Acute/Review/Follow-up | Type of visit |

**Allowed Values:**
- `Acute` - New/emergency visit
- `Review` - Scheduled follow-up
- `Follow-up` - Continuation of previous visit

**Usage:** POST `/attendance`

---

### 55. **PatientSchemeDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for patient insurance scheme information (inferred)

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `PatientSchemesID` | `Guid` | Patient scheme identifier |
| `PatientsID` | `Guid` | Patient reference |
| `SchemesID` | `Guid` | Insurance scheme reference |
| `CardID` | `string` | Insurance card number |
| `IsActive` | `bool` | Active status (default: true) |
| `ExpiryDate` | `DateTime` | Card expiry date |
| `LastUpdateDate` | `DateTime` | Last update timestamp |

**Usage:** GET `/patients/{id}/schemes`

---

### 56. **AddPatientInsuranceDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for adding insurance scheme to patient

```csharp
public record AddPatientInsuranceDto(
    [Required] Guid PatientsID,
    [Required] Guid SchemesID,
    [DefaultValue(true)] bool IsActive,
    [Required, StringLength(30, MinimumLength = 10)] string CardID,
    [Required] DateTime ExpiryDate);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientsID` | `Guid` | Required | Patient identifier |
| `SchemesID` | `Guid` | Required | Insurance scheme identifier |
| `IsActive` | `bool` | Default: true | Activation status |
| `CardID` | `string` | Required, 10-30 chars | Insurance card number |
| `ExpiryDate` | `DateTime` | Required | Card expiration date |

**Usage:** POST `/patients/{id}/insurance`

---

### 57. **EditPatientInsuranceDto**
**Namespace:** `ShimsServer.Models.Records`  
**Type:** Record  
**Purpose:** Data transfer object for updating patient insurance scheme

```csharp
public record EditPatientInsuranceDto(
    [Required] Guid PatientInsurancesID,
    [Required] Guid PatientsID,
    [Required] Guid InsurancesID,
    [DefaultValue(true)] bool Status,
    [Required, StringLength(30, MinimumLength = 10)] string CardID,
    [Required] DateTime ExpiryDate);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientInsurancesID` | `Guid` | Required | Patient insurance record to update |
| `PatientsID` | `Guid` | Required | Patient identifier |
| `InsurancesID` | `Guid` | Required | Insurance scheme identifier |
| `Status` | `bool` | Default: true | Updated status |
| `CardID` | `string` | Required, 10-30 chars | Updated card number |
| `ExpiryDate` | `DateTime` | Required | Updated expiry date |

**Usage:** PUT `/patients/{id}/insurance/{insuranceId}`

---

## Wards Management

### 51. **WardsDTO**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for ward information

```csharp
public record WardsDTO(
    Guid WardsID,
    string Ward,
    string WardTags,
    short Capacity);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `WardsID` | `Guid` | Unique identifier for the ward |
| `Ward` | `string` | Ward name (e.g., "Pediatrics", "Cardiology") |
| `WardTags` | `string` | Ward abbreviation/tag (e.g., "PED", "CARD") |
| `Capacity` | `short` | Number of beds in the ward (5-50) |

**Usage:** GET `/wards`, GET `/wards/{id}`

---

### 52. **AddWardsDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for creating new wards

```csharp
public record AddWardsDto(
    [Required, StringLength(50, MinimumLength = 5)] string Ward,
    [Required, StringLength(10, MinimumLength = 4)] string WardTags,
    [Required, Range(5, 50)] short Capacity);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `Ward` | `string` | Required, 5-50 chars | Ward name |
| `WardTags` | `string` | Required, 4-10 chars | Ward tag/abbreviation |
| `Capacity` | `short` | Required, 5-50 | Number of beds |

**Validation:**
- Ward name must be unique
- Capacity between 5 and 50 beds

**Usage:** POST `/wards`

---

### 53. **UpdateWardsDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for updating ward information

```csharp
public record UpdateWardsDto(
    Guid WardsID,
    [Required, StringLength(50, MinimumLength = 5)] string Ward,
    [Required, StringLength(10, MinimumLength = 4)] string WardTags,
    [Required, Range(5, 50)] short Capacity);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `WardsID` | `Guid` | Required | Ward identifier to update |
| `Ward` | `string` | Required, 5-50 chars | Updated ward name |
| `WardTags` | `string` | Required, 4-10 chars | Updated ward tag |
| `Capacity` | `short` | Required, 5-50 | Updated bed capacity |

**Usage:** PUT `/wards/{id}`

---

### 54. **WardOccupancyDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for ward with current occupancy information

```csharp
public record WardOccupancyDto(
    Guid WardsID,
    string Ward,
    string WardTags,
    short Capacity,
    int CurrentOccupancy,
    int AvailableBeds);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `WardsID` | `Guid` | Ward identifier |
| `Ward` | `string` | Ward name |
| `WardTags` | `string` | Ward tag |
| `Capacity` | `short` | Total ward capacity |
| `CurrentOccupancy` | `int` | Currently admitted patients |
| `AvailableBeds` | `int` | Number of available beds |

**Usage:** GET `/wards/{id}/occupancy`, Ward capacity and occupancy status

---

### 55. **WardAdmissionDTO**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for ward admission information

```csharp
public record WardAdmissionDTO(
    Guid WardAdmissionsID,
    Guid PatientsAttendancesID,
    Guid WardsID,
    string PatientName,
    string WardName,
    DateTime DateAdmitted,
    DateTime? DateDischarged,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `WardAdmissionsID` | `Guid` | Unique admission record ID |
| `PatientsAttendancesID` | `Guid` | Patient attendance reference |
| `WardsID` | `Guid` | Ward reference |
| `PatientName` | `string` | Patient name for display |
| `WardName` | `string` | Ward name for display |
| `DateAdmitted` | `DateTime` | Admission date |
| `DateDischarged` | `DateTime?` | Discharge date (if applicable) |
| `UserName` | `string` | Staff member who admitted patient |

**Usage:** GET `/ward-admissions`, GET `/ward-admissions/{id}`

---

### 56. **AddWardAdmissionDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for creating ward admissions

```csharp
public record AddWardAdmissionDto(
    [Required] Guid PatientsAttendancesID,
    [Required] Guid WardsID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientsAttendancesID` | `Guid` | Required | Patient attendance reference |
| `WardsID` | `Guid` | Required | Ward to admit patient to |

**Usage:** POST `/ward-admissions`

---

### 57. **UpdateWardAdmissionDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for updating ward admission (transfer)

```csharp
public record UpdateWardAdmissionDto(
    Guid WardAdmissionsID,
    [Required] Guid PatientsAttendancesID,
    [Required] Guid WardsID);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `WardAdmissionsID` | `Guid` | Required | Admission record to update |
| `PatientsAttendancesID` | `Guid` | Required | Patient attendance reference |
| `WardsID` | `Guid` | Required | New ward for transfer |

**Usage:** PUT `/ward-admissions/{id}` (for ward transfers)

---

### 58. **DischargePatientDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for discharging patient from ward

```csharp
public record DischargePatientDto(
    Guid WardAdmissionsID);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `WardAdmissionsID` | `Guid` | Admission record to discharge |

**Usage:** POST `/ward-admissions/{id}/discharge`

---

### 59. **WardAdmissionSummaryDto**
**Namespace:** `ShimsServer.Models.Wards`  
**Type:** Record  
**Purpose:** Data transfer object for ward admission summary with all details

```csharp
public record WardAdmissionSummaryDto(
    Guid WardAdmissionsID,
    string PatientName,
    string OPDNumber,
    string WardName,
    string WardTag,
    DateTime DateAdmitted,
    DateTime? DateDischarged,
    int DaysAdmitted,
    string Status);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `WardAdmissionsID` | `Guid` | Admission record ID |
| `PatientName` | `string` | Patient name for display |
| `OPDNumber` | `string` | Patient OPD number |
| `WardName` | `string` | Ward name |
| `WardTag` | `string` | Ward abbreviation |
| `DateAdmitted` | `DateTime` | Admission date |
| `DateDischarged` | `DateTime?` | Discharge date (if applicable) |
| `DaysAdmitted` | `int` | Number of days admitted |
| `Status` | `string` | Current status: Admitted or Discharged |

**Usage:** GET `/ward-admissions/{id}/summary`, Complete admission details for reports

---

## Out-Patient Department (OPD)

### 58. **VitalsDTO**
**Namespace:** `ShimsServer.Models.OPD`  
**Type:** Record  
**Purpose:** Data transfer object for vital signs information

```csharp
public record VitalsDTO(
    Guid VitalsID,
    Guid PatientsAttendancesID,
    DateTime DateSeen,
    double Temperature,
    double Weight,
    double? Pulse,
    double? Systol,
    double? Diastol,
    double? Respiration,
    double? SPO2,
    string Complaints,
    string? Notes,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `VitalsID` | `Guid` | Unique identifier for vital signs record |
| `PatientsAttendancesID` | `Guid` | Patient attendance reference |
| `DateSeen` | `DateTime` | Date vitals were recorded |
| `Temperature` | `double` | Body temperature (°C, 36-45) |
| `Weight` | `double` | Patient weight (kg, 1.8-250) |
| `Pulse` | `double?` | Heart rate (bpm, 20-250) |
| `Systol` | `double?` | Systolic blood pressure (mmHg, 20-250) |
| `Diastol` | `double?` | Diastolic blood pressure (mmHg, 20-250) |
| `Respiration` | `double?` | Respiration rate (breaths/min, 12-60) |
| `SPO2` | `double?` | Oxygen saturation (%, 50-110) |
| `Complaints` | `string` | Patient complaints (3-200 chars) |
| `Notes` | `string?` | Additional clinical notes (5-200 chars) |
| `UserName` | `string` | Staff member who recorded vitals |

**Usage:** GET `/opd/vitals`, GET `/opd/vitals/{id}`

---

### 52. **AddVitalsDto**
**Namespace:** `ShimsServer.Models.OPD`  
**Type:** Record  
**Purpose:** Data transfer object for creating vital signs records

```csharp
public record AddVitalsDto(
    [Required] Guid PatientsAttendancesID,
    [Required, Range(36, 45.0)] double Temperature,
    [Required, Range(1.8, 250)] double Weight,
    [Range(20, 250)] double? Pulse,
    [Range(20, 250)] double? Systol,
    [Range(20, 250)] double? Diastol,
    [Range(12, 60)] double? Respiration,
    [Range(50.0, 110.0)] double? SPO2,
    [Required, StringLength(200, MinimumLength = 3)] string Complaints,
    [StringLength(200, MinimumLength = 5)] string? Notes);
```

**Properties:**
| Property | Type | Validation | Description |
|----------|------|-----------|-------------|
| `PatientsAttendancesID` | `Guid` | Required | Patient attendance reference |
| `Temperature` | `double` | Required, 36-45°C | Body temperature |
| `Weight` | `double` | Required, 1.8-250 kg | Patient weight |
| `Pulse` | `double?` | Optional, 20-250 bpm | Heart rate |
| `Systol` | `double?` | Optional, 20-250 mmHg | Systolic BP |
| `Diastol` | `double?` | Optional, 20-250 mmHg | Diastolic BP |
| `Respiration` | `double?` | Optional, 12-60 breaths/min | Respiration rate |
| `SPO2` | `double?` | Optional, 50-110% | Oxygen saturation |
| `Complaints` | `string` | Required, 3-200 chars | Patient complaints |
| `Notes` | `string?` | Optional, 5-200 chars | Clinical notes |

**Usage:** POST `/opd/vitals`

---

### 53. **VitalsSummaryDto**
**Namespace:** `ShimsServer.Models.OPD`  
**Type:** Record  
**Purpose:** Data transfer object for vital signs with patient information

```csharp
public record VitalsSummaryDto(
    Guid VitalsID,
    string PatientName,
    string OPDNumber,
    DateTime DateSeen,
    double Temperature,
    double Weight,
    double? Pulse,
    double? Systol,
    double? Diastol,
    double? Respiration,
    double? SPO2,
    string Complaints,
    string UserName);
```

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `VitalsID` | `Guid` | Vital signs record identifier |
| `PatientName` | `string` | Patient name for display |
| `OPDNumber` | `string` | OPD patient number |
| `DateSeen` | `DateTime` | Date vitals were recorded |
| `Temperature` | `double` | Body temperature |
| `Weight` | `double` | Patient weight |
| `Pulse` | `double?` | Heart rate |
| `Systol` | `double?` | Systolic blood pressure |
| `Diastol` | `double?` | Diastolic blood pressure |
| `Respiration` | `double?` | Respiration rate |
| `SPO2` | `double?` | Oxygen saturation |
| `Complaints` | `string` | Patient complaints |
| `UserName` | `string` | Recording staff member |

**Usage:** GET `/opd/vitals/summary`, Vital signs with patient details for display/reports

---

## Summary

| DTO/VM | Type | Purpose | Endpoint(s) |
|--------|------|---------|-----------|
| LoginVm | Record | User login | POST `/auth/login` |
| RegisterVm | Class | User self-registration | POST `/Account/Register` |
| GuestsVM | Class | Guest user creation | POST `/auth/register-guest` |
| UsersDto | Record | Single user info | GET `/users/{id}` |
| UsersDtoVw | Record | User with multiple roles | GET `/users` |
| SchemesDTO | Record | Scheme details | GET `/schemes`, GET `/schemes/{id}` |
| AddSchemeDto | Record | Create scheme | POST `/schemes` |
| UpdateSchemeDto | Record | Update scheme | PUT `/schemes/{id}` |
| PendingVm | Class | Pending consultation | Consultation endpoints |
| ServicesDTO | Record | Service details | GET `/services`, GET `/services/{id}` |
| AddServiceDto | Record | Create service | POST `/services` |
| UpdateServiceDto | Record | Update service | PUT `/services/{id}` |
| ServiceWithCoverageDto | Record | Service coverage | GET `/services/{id}/coverage` |
| DrugDTO | Record | Drug details | GET `/drugs`, GET `/drugs/{id}` |
| AddDrugDto | Record | Create drug | POST `/drugs` |
| UpdateDrugDto | Record | Update drug | PUT `/drugs/{id}` |
| DrugStockDTO | Record | Drug stock info | GET `/drugs/{id}/stock` |
| AddDrugStockDto | Record | Add stock | POST `/drugs/{id}/stock` |
| UpdateDrugStockDto | Record | Update stock | PUT `/drugs/stock/{id}` |
| DrugWithStockDto | Record | Drug with inventory | GET `/drugs/inventory` |
| DrugRequestDTO | Record | Drug request details | GET `/drugs/requests` |
| AddDrugRequestDto | Record | Create drug request | POST `/drugs/requests` |
| DrugRequestSummaryDto | Record | Drug request summary | GET `/drugs/requests/summary` |
| DispensingCalculationDTO | Record | Dispensing calc details | GET `/drugs/dispensing-calculations/{id}` |
| AddDispensingCalculationDto | Record | Create dispensing calc | POST `/drugs/dispensing-calculations` |
| UpdateDispensingCalculationDto | Record | Update dispensing calc | PUT `/drugs/dispensing-calculations/{id}` |
| DrugPaymentDTO | Record | Drug payment details | GET `/drugs/payments` |
| AddDrugPaymentDto | Record | Create payment | POST `/drugs/payments` |
| UpdateDrugPaymentDto | Record | Update payment | PUT `/drugs/payments/{id}` |
| DrugPaymentDetailedDto | Record | Payment with dispensing | GET `/drugs/payments/{id}/details` |
| DispensingDTO | Record | Dispensing details | GET `/drugs/dispensing/{id}` |
| AddDispensingDto | Record | Create dispensing | POST `/drugs/dispensing` |
| UpdateDispensingDto | Record | Update dispensing | PUT `/drugs/dispensing/{id}` |
| DrugWorkflowDTO | Record | Complete drug workflow | GET `/drugs/requests/{id}/workflow` |
| LabRequestDTO | Record | Lab request details | GET `/labs/requests` |
| AddLabRequestDto | Record | Create lab request | POST `/labs/requests` |
| LabRequestSummaryDto | Record | Lab request summary | GET `/labs/requests/summary` |
| LabPaymentDTO | Record | Lab payment details | GET `/labs/payments` |
| AddLabPaymentDto | Record | Create lab payment | POST `/labs/payments` |
| UpdateLabPaymentDto | Record | Update lab payment | PUT `/labs/payments/{id}` |
| LabPaymentDetailedDto | Record | Payment with request | GET `/labs/payments/{id}/details` |
| LabGroupDTO | Record | Lab group details | GET `/labs/groups`, GET `/labs/groups/{id}` |
| AddLabGroupDto | Record | Create lab group | POST `/labs/groups` |
| UpdateLabGroupDto | Record | Update lab group | PUT `/labs/groups/{id}` |
| LabParameterDTO | Record | Lab parameter details | GET `/labs/parameters` |
| AddLabParameterDto | Record | Create lab parameter | POST `/labs/parameters` |
| UpdateLabParameterDto | Record | Update lab parameter | PUT `/labs/parameters/{id}` |
| LabResultDTO | Record | Lab result details | GET `/labs/results` |
| AddLabResultDto | Record | Add lab result | POST `/labs/results` |
| LabResultWithPaymentDto | Record | Lab results summary | GET `/labs/results/{id}/summary` |
| PatientDto | Record | Patient details | GET `/patients`, GET `/patients/{id}` |
| AddPatientDto | Record | Create patient | POST `/patients` |
| EditPatientDto | Record | Update patient | PUT `/patients/{id}` |
| AttendanceDto | Record | Create attendance | POST `/attendance` |
| PatientSchemeDto | Record | Patient insurance | GET `/patients/{id}/schemes` |
| AddPatientInsuranceDto | Record | Add patient insurance | POST `/patients/{id}/insurance` |
| EditPatientInsuranceDto | Record | Update patient insurance | PUT `/patients/{id}/insurance/{id}` |
| WardsDTO | Record | Ward details | GET `/wards`, GET `/wards/{id}` |
| AddWardsDto | Record | Create ward | POST `/wards` |
| UpdateWardsDto | Record | Update ward | PUT `/wards/{id}` |
| WardOccupancyDto | Record | Ward with occupancy | GET `/wards/{id}/occupancy` |
| WardAdmissionDTO | Record | Admission details | GET `/ward-admissions` |
| AddWardAdmissionDto | Record | Create admission | POST `/ward-admissions` |
| UpdateWardAdmissionDto | Record | Transfer ward | PUT `/ward-admissions/{id}` |
| DischargePatientDto | Record | Discharge patient | POST `/ward-admissions/{id}/discharge` |
| WardAdmissionSummaryDto | Record | Admission summary | GET `/ward-admissions/{id}/summary` |
| VitalsDTO | Record | Vital signs details | GET `/opd/vitals`, GET `/opd/vitals/{id}` |
| AddVitalsDto | Record | Create vitals record | POST `/opd/vitals` |
| VitalsSummaryDto | Record | Vitals with patient info | GET `/opd/vitals/summary` |

---

## Newly Generated DTOs

The following DTOs have been generated to support the Services, Drugs, and Labs modules:

### Location: `ShimsServer\Models\Schemes\SchemePricingDtos.cs`

**Scheme Drug Pricing:**
- `SchemeDrugDTO` - Scheme drug pricing info
- `AddSchemeDrugDto` - Create scheme drug pricing
- `UpdateSchemeDrugDto` - Update scheme drug pricing
- `SchemeDrugAvailabilityDto` - Scheme drugs availability

**Scheme Lab Pricing:**
- `SchemeLabDTO` - Scheme lab pricing info
- `AddSchemeLabDto` - Create scheme lab pricing
- `UpdateSchemeLabDto` - Update scheme lab pricing
- `SchemeLabAvailabilityDto` - Scheme labs availability

**Scheme Service Pricing:**
- `SchemeServiceDTO` - Scheme service pricing info
- `AddSchemeServiceDto` - Create scheme service pricing
- `UpdateSchemeServiceDto` - Update scheme service pricing
- `SchemeServiceAvailabilityDto` - Scheme services coverage

### Location: `ShimsServer\Models\Services\ServicesDtos.cs`
- `ServicesDTO`
- `AddServiceDto`
- `UpdateServiceDto`
- `ServiceWithCoverageDto`

### Location: `ShimsServer\Models\Drugs\DrugDtos.cs`
**Basic Drug Management:**
- `DrugDTO`
- `AddDrugDto`
- `UpdateDrugDto`

**Drug Stock Management:**
- `DrugStockDTO`
- `AddDrugStockDto`
- `UpdateDrugStockDto`
- `DrugWithStockDto`

**Drug Request Workflow:**
- `DrugRequestDTO`
- `AddDrugRequestDto`
- `DrugRequestSummaryDto`

**Dispensing Calculations:**
- `DispensingCalculationDTO`
- `AddDispensingCalculationDto`
- `UpdateDispensingCalculationDto`

**Drug Payments:**
- `DrugPaymentDTO`
- `AddDrugPaymentDto`
- `UpdateDrugPaymentDto`
- `DrugPaymentDetailedDto`

**Dispensing Records:**
- `DispensingDTO`
- `AddDispensingDto`
- `UpdateDispensingDto`

**Complete Workflow:**
- `DrugWorkflowDTO` - Tracks complete request to dispensing process

### Location: `ShimsServer\Models\Labs\LabDtos.cs`

**Lab Requests:**
- `LabRequestDTO`
- `AddLabRequestDto`
- `LabRequestSummaryDto`

**Lab Payments:**
- `LabPaymentDTO`
- `AddLabPaymentDto`
- `UpdateLabPaymentDto`
- `LabPaymentDetailedDto`

**Lab Groups:**
- `LabGroupDTO`
- `AddLabGroupDto`
- `UpdateLabGroupDto`

**Lab Parameters:**
- `LabParameterDTO`
- `AddLabParameterDto`
- `UpdateLabParameterDto`

**Lab Results:**
- `LabResultDTO`
- `AddLabResultDto`
- `LabResultWithPaymentDto`

### Location: `ShimsServer\Models\Records\Patients.cs`

**Patient Management:**
- `PatientDto` - Patient information
- `AddPatientDto` - Create new patient
- `EditPatientDto` - Update patient

### Location: `ShimsServer\Models\Records\PatientSchemes.cs`

**Patient Insurance:**
- `AddPatientInsuranceDto` - Add insurance to patient
- `EditPatientInsuranceDto` - Update patient insurance

### Location: `ShimsServer\Models\Records\PatientAttendance.cs`

**Patient Attendance:**
- `AttendanceDto` - Create patient visit record

### Location: `ShimsServer\Models\Wards\WardsDtos.cs`

**Wards Management:**
- `WardsDTO` - Ward information
- `AddWardsDto` - Create new ward
- `UpdateWardsDto` - Update ward
- `WardOccupancyDto` - Ward with occupancy info

**Ward Admissions:**
- `WardAdmissionDTO` - Admission details
- `AddWardAdmissionDto` - Create admission
- `UpdateWardAdmissionDto` - Transfer to different ward
- `DischargePatientDto` - Discharge from ward
- `WardAdmissionSummaryDto` - Complete admission summary

### Location: `ShimsServer\Models\OPD\OPDDtos.cs`

**Vital Signs:**
- `VitalsDTO` - View vital signs
- `AddVitalsDto` - Record new vitals
- `VitalsSummaryDto` - Vitals with patient information

## Drug Workflow Overview

The DTOs follow a complete workflow pattern for drug management:

1. **Request Stage** - `DrugRequestDTO`, `AddDrugRequestDto`
   - Patient/Doctor requests a drug with frequency and duration

2. **Calculation Stage** - `DispensingCalculationDTO`, `AddDispensingCalculationDto`
   - Pharmacist calculates the exact quantity to dispense

3. **Payment Stage** - `DrugPaymentDTO`, `AddDrugPaymentDto`
   - Patient/Insurance pays for the calculated quantity

4. **Dispensing Stage** - `DispensingDTO`, `AddDispensingDto`
   - Pharmacist dispenses the drug

5. **Tracking Stage** - `DrugWorkflowDTO`
   - Complete view of the entire workflow for reporting and auditing

## Lab Workflow Overview

The DTOs follow a complete workflow pattern for laboratory testing:

1. **Request Stage** - `LabRequestDTO`, `AddLabRequestDto`
   - Doctor/Provider requests lab tests for a patient

2. **Payment Stage** - `LabPaymentDTO`, `AddLabPaymentDto`
   - Patient/Insurance pays for the requested tests

3. **Test Parameters Stage** - `LabParameterDTO`, `AddLabParameterDto`
   - Lab group organizes parameters within test categories

4. **Results Stage** - `LabResultDTO`, `AddLabResultDto`
   - Lab technician enters individual test results

5. **Summary Stage** - `LabResultWithPaymentDto`
   - Complete view of all results with associated payment information

6. **Tracking Stage** - `LabPaymentDetailedDto`
   - Payment details linked with request information for audit trail

## OPD (Out-Patient Department) Workflow Overview

The DTOs follow a workflow pattern for vital signs recording:

1. **Assessment Stage** - `AddVitalsDto`
   - Nurse/Staff records patient vital signs at OPD visit

2. **Viewing Stage** - `VitalsDTO`
   - Doctor/Provider views recorded vital signs

3. **Summary Stage** - `VitalsSummaryDto`
   - Complete vital signs with patient information for clinical decision-making

## Patient Records Workflow Overview

The DTOs follow a workflow pattern for patient management:

1. **Registration Stage** - `AddPatientDto`
   - New patient registration with demographics

2. **Profile Stage** - `PatientDto`, `EditPatientDto`
   - View and update patient information

3. **Insurance Stage** - `AddPatientInsuranceDto`, `EditPatientInsuranceDto`
   - Link insurance schemes to patient records

4. **Attendance Stage** - `AttendanceDto`
   - Record each patient visit with visit type

5. **Vitals Stage** - `AddVitalsDto`, `VitalsDTO`
   - Record vital signs during attendance

6. **Scheme Stage** - `PatientSchemeDto`
   - View active insurance coverage

## Wards Management Workflow Overview

The DTOs follow a workflow pattern for inpatient ward management:

1. **Ward Setup Stage** - `AddWardsDto`, `WardsDTO`
   - Create and manage ward master data (Pediatrics, Cardiology, etc.)

2. **Occupancy Tracking Stage** - `WardOccupancyDto`
   - Monitor ward capacity and available beds

3. **Admission Stage** - `AddWardAdmissionDto`, `WardAdmissionDTO`
   - Admit patient to appropriate ward

4. **Transfer Stage** - `UpdateWardAdmissionDto`
   - Transfer patient between wards

5. **Discharge Stage** - `DischargePatientDto`
   - Discharge patient from ward

6. **Summary Stage** - `WardAdmissionSummaryDto`
   - Complete admission record with length of stay for reports and billing

## Notes

- All DTOs use nullable reference types (`#nullable enable`)
- Records are immutable by default and optimized for data transfer
- View Models (VMs) are typically used for form submission and client-side binding
- DTOs are used for API responses and should not expose sensitive data
- Validation is performed both client-side and server-side
- All datetime operations use `CancellationToken` for graceful shutdown support
- Generated DTOs include XML documentation comments for IntelliSense support
- Each module (Services, Drugs, Labs, OPD) has dedicated DTO classes for CRUD operations
- Drug workflow DTOs include status tracking for complete audit trail
- All payment-related DTOs include receipt/transaction numbers for accountability
- OPD Vitals DTOs include comprehensive validation ranges for medical accuracy
- Vital signs ranges are based on standard clinical parameters:
  - Temperature: 36-45°C (normal body temperature range)
  - Weight: 1.8-250 kg (realistic human weight range)
  - Pulse: 20-250 bpm (clinically viable range)
  - Blood Pressure: 20-250 mmHg (systolic/diastolic)
  - Respiration: 12-60 breaths/min (normal breathing range)
  - SPO2: 50-110% (oxygen saturation percentage)
