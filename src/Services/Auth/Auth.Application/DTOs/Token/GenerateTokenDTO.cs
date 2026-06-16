using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Application.DTOs.Token
{
    public record GenerateTokenDTO(
        int UserId,
        string Email,
        bool EmailVerified,
        int TenantId,
        string Role,
        IEnumerable<string> Roles,
        IEnumerable<string> Permissions);
}
