namespace Auth.Application.Policies
{
    public static class RequiredConsents
    {
        public static IReadOnlyList<RequiredConsent> Defaults() => new List<RequiredConsent>
        {
            new RequiredConsent("terms_of_use", "v1"),
            new RequiredConsent("privacy_policy", "v1"),
            new RequiredConsent("pilot_notice", "v1")
        };
    }

    public sealed record RequiredConsent(string DocumentType, string Version);
}
