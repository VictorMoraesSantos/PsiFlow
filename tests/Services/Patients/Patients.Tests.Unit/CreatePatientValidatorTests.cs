using Xunit;

namespace PsiFlow.Patients.Tests.Unit;

public sealed class CreatePatientValidatorTests
{
    [Fact]
    public void CreatePatientDto_Should_Carry_Required_Data()
    {
        var tenantId = 1;
        var dto = new global::Patients.Application.DTOs.Patient.CreatePatientDTO(
            tenantId,
            "Maria Silva",
            "maria@example.com",
            "+5511999999999",
            DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30)),
            "12345678900",
            "Main Street",
            "Initial notes");

        Assert.Equal(tenantId, dto.TenantId);
        Assert.Equal("Maria Silva", dto.FullName);
    }
}
