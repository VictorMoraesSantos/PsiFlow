namespace Auth.Infrastructure.Persistence.Seeds
{
    public class AuthSeedOptions
    {
        public bool SeedAdmin { get; set; } = true;
        public bool SeedPsychologist { get; set; } = true;
        public bool SeedPatient { get; set; } = true;
    }
}
