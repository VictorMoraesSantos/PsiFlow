using BuildingBlocks.Results;
using Microsoft.Extensions.Logging.Abstractions;
using Patients.Application.DTOs.Patient;
using Patients.Application.Services;
using Xunit;

namespace PsiFlow.Patients.Tests.Unit;

public sealed class PatientServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenPatientIsMinorWithoutResponsibleLegal_ShouldFail()
    {
        var service = new PatientService(new InMemoryPatientRepository(), NullLogger<PatientService>.Instance);
        var dto = new CreatePatientDTO(
            TenantId: 1,
            FullName: "Paciente Menor",
            Email: "menor@example.com",
            Phone: "11999999999",
            BirthDate: DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-12)),
            EmergencyContactName: null,
            EmergencyContactPhone: null,
            Address: null,
            DocumentNumber: null);

        var result = await service.CreateAsync(dto, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task GetByIdAndTenantAsync_WhenPatientBelongsToAnotherTenant_ShouldReturnForbidden()
    {
        var patient = new global::Patients.Domain.Entities.Patient
        {
            TenantId = 1,
            FullName = "Maria Silva",
            Email = "maria@example.com",
            Phone = "11999999999"
        };

        typeof(Core.Domain.Aggregates.BaseEntity<int>)
            .GetProperty(nameof(Core.Domain.Aggregates.BaseEntity<int>.Id), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!
            .SetValue(patient, 10);

        var service = new PatientService(new InMemoryPatientRepository(patient), NullLogger<PatientService>.Instance);

        var result = await service.GetByIdAndTenantAsync(10, tenantId: 2, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Forbidden, result.Error!.Type);
    }
}
