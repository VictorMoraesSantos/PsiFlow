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
}
