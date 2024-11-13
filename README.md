Certainly! Here’s a detailed project explanation in **Markdown** format, suitable for interviews or documentation.

---

# Patient Registration Service

The **Patient Registration Service** is a RESTful API developed to manage patient registration and information in a healthcare environment, specifically for a cancer clinic. It provides endpoints for creating, reading, updating, and deleting patient records while enforcing business rules, such as assigning physicians based on diagnosis.

## Project Overview

- **Platform**: .NET Core with Entity Framework Core
- **Database**: SQLite for persistence
- **API Documentation**: Swagger for easy endpoint testing and documentation

---

## Project Structure

### 1. Models

The model classes define the structure and relationships of data within the application.

#### `Patient` Model

The `Patient` model represents a patient with demographic and medical information.

```csharp
public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] [StringLength(100, MinimumLength = 2)] public string Name { get; set; }
    [Required] public string MedicalRecordNumber { get; set; }
    [Range(0, 120)] public int Age { get; set; }
    [Required] public string Gender { get; set; }
    [MinLength(1)] public List<string> Contacts { get; set; } = new List<string>();
    public Diagnosis? AdmittingDiagnosis { get; set; }
    public string? AttendingPhysician { get; set; }
    public string? Department { get; set; }
}
```

#### `Diagnosis` Enum

Defines values for the patient’s admitting diagnosis:

```csharp
public enum Diagnosis
{
    BreastCancer,
    LungCancer,
    ProstateCancer,
    Unspecified
}
```

### 2. DTOs (Data Transfer Objects)

DTOs are used to validate and restrict data received from external clients.

#### `CreatePatientDto`

```csharp
public class CreatePatientDto
{
    [Required] public string Name { get; set; }
    [Required] public string MedicalRecordNumber { get; set; }
    [Range(0, 120)] public int Age { get; set; }
    [Required] public string Gender { get; set; }
    [MinLength(1)] public List<string> Contacts { get; set; }
    public Diagnosis? AdmittingDiagnosis { get; set; }
}
```

### 3. Database Context

`DataContext` configures EF Core for managing database interactions:

```csharp
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.MedicalRecordNumber).HasMaxLength(50);
            entity.Property(p => p.Gender).HasMaxLength(20);
            entity.Property(p => p.AttendingPhysician).HasMaxLength(100);
            entity.Property(p => p.Department).HasMaxLength(50);
        });
    }
}
```

### 4. Controller

The `PatientsController` defines API endpoints for managing patient data.

#### Key Endpoints

- **POST /api/patients** - Registers a new patient
- **GET /api/patients** - Retrieves all patients
- **GET /api/patients/{id}** - Retrieves a single patient by ID
- **PUT /api/patients/{id}** - Updates a patient’s demographics
- **DELETE /api/patients/{id}** - Deletes a patient with restrictions

#### Example: Registering a Patient

```csharp
[HttpPost]
public IActionResult RegisterPatient([FromBody] CreatePatientDto patientDto)
{
    if (!ModelState.IsValid) { return BadRequest(ModelState); }

    var patient = new Patient
    {
        Name = patientDto.Name,
        MedicalRecordNumber = patientDto.MedicalRecordNumber,
        Age = patientDto.Age,
        Gender = patientDto.Gender,
        Contacts = patientDto.Contacts,
        AdmittingDiagnosis = patientDto.AdmittingDiagnosis
    };

    if (patient.AdmittingDiagnosis.HasValue)
    {
        (patient.AttendingPhysician, patient.Department) = AssignPhysicianAndDepartment(patient.AdmittingDiagnosis.Value);
    }

    _repository.AddPatient(patient);
    return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
}
```

### 5. Repository Pattern

The repository pattern abstracts data access, ensuring cleaner controller code.

#### `PatientRepository`

```csharp
public class PatientRepository : IPatientRepository
{
    private readonly DataContext _context;
    public PatientRepository(DataContext context) { _context = context; }

    public IEnumerable<Patient> GetAllPatients() => _context.Patients.ToList();
    public Patient GetPatient(Guid id) => _context.Patients.Find(id);
    public void AddPatient(Patient patient) { _context.Patients.Add(patient); _context.SaveChanges(); }
    public void UpdatePatient(Patient patient) { _context.Patients.Update(patient); _context.SaveChanges(); }
    public void RemovePatient(Guid id)
    {
        var patient = GetPatient(id);
        if (patient != null) { _context.Patients.Remove(patient); _context.SaveChanges(); }
    }
}
```

### 6. Program Configuration

The `Program.cs` file configures services, middleware, and database options.

#### Key Configurations

- **Use SQLite** for data persistence
- **JsonStringEnumConverter** to serialize enums as strings
- **Swagger** for easy API documentation and testing

```csharp
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

---

## CRUD Operations

### Create a New Patient

**Endpoint**: `POST /api/patients`

**Example Payload**:

```json
{
    "name": "Alice Johnson",
    "medicalRecordNumber": "MRN12345",
    "age": 45,
    "gender": "Female",
    "contacts": ["alice.johnson@example.com", "+1234567890"],
    "admittingDiagnosis": "BreastCancer"
}
```

**Expected Response**:

```json
{
    "id": "generated-uuid",
    "name": "Alice Johnson",
    "medicalRecordNumber": "MRN12345",
    "age": 45,
    "gender": "Female",
    "contacts": ["alice.johnson@example.com", "+1234567890"],
    "admittingDiagnosis": "BreastCancer",
    "attendingPhysician": "Dr. Susan Jones",
    "department": "Department J"
}
```

### Retrieve All Patients

**Endpoint**: `GET /api/patients`

**Response**: Returns a list of all patients in the database.

### Retrieve a Single Patient by ID

**Endpoint**: `GET /api/patients/{id}`

**Response**: Returns the details of the specified patient.

### Update Patient Information

**Endpoint**: `PUT /api/patients/{id}`

**Example Payload**:

```json
{
    "name": "Alice Johnson Updated",
    "medicalRecordNumber": "MRN67890",
    "age": 46,
    "gender": "Female",
    "contacts": ["alice.johnson.updated@example.com", "+1234567891"]
}
```

### Delete a Patient

**Endpoint**: `DELETE /api/patients/{id}`

**Response**: A successful deletion returns a `204 No Content` status. Deletion is restricted if the patient has a significant diagnosis.

---

## Business Rules and Validations

1. **Physician and Department Assignment**:
   - Based on `AdmittingDiagnosis`, `AttendingPhysician` and `Department` are auto-assigned.
   - **BreastCancer** and **LungCancer** patients are assigned to **Dr. Susan Jones** in **Department J**.
   - All other patients go to **Dr. Ben Smith** in **Department S**.

2. **Deletion Rules**:
   - Patients with a significant diagnosis (e.g., not `Unspecified`) cannot be deleted.

3. **Field Validations**:
   - Required fields (`Name`, `MedicalRecordNumber`, `Gender`) are enforced using the `[Required]` attribute.
   - Enum properties are restricted to defined values.

---

## Deployment Considerations

- **SQLite** is used for its simplicity and cross-platform support. However, for production, **SQL Server** or **PostgreSQL** might be better choices.
- **Swagger** enables easy documentation and testing of endpoints, facilitating development and debugging.

---

## Summary

The **Patient Registration Service** is a well-structured RESTful API designed to manage patient data efficiently in a clinical setting. By adhering to business rules and implementing validation and error handling, this API serves as a reliable foundation for patient management tasks.

This project demonstrates:
- Clean architecture with the repository pattern
- Separation of concerns using DTOs and models
- Detailed validation and business logic enforcement

This setup ensures maintainability, scalability, and a secure foundation for handling sensitive patient data in healthcare environments.
