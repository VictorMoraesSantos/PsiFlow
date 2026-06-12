using Auth.Domain.Errors;
using Core.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.ValueObjects
{
    public record MfaChallengeId
    {
        public int Value { get; }
        public MfaChallengeId(int value)
        {
            if (value < 0)
                throw new DomainException(MfaChallengeErrors.InvalidId);
        }

        public override string ToString() => Value.ToString();
        public static implicit operator int(MfaChallengeId id) => new MfaChallengeId(id);
    }
}
