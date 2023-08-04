namespace Application.Commons
{
    public class AppConfiguration
    {
        public string DatabaseConnection { get; set; } = null!;
        public string JWTSecretKey { get; set; } = null!;
        public EmailSetting EmailSetting { get; set; } = new EmailSetting();
        public SerilogSetting Serilog { get; set; } = new SerilogSetting();
        public AuthenticationSetting Authentication { get; set; } = new AuthenticationSetting();
    }

    public class EmailSetting
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }

    public class SerilogSetting
    {
        public string MinimumLevel { get; set; } = null!;
        public string[] Using { get; set; } = null!;
        public object[] WriteTo { get; set; } = null!;
        public object[] Enrich { get; set; } = null!;
        public object Properties { get; set; } = null!;
    }

    public class AuthenticationSetting
    {
        public GoogleSetting Google { get; set; } = new GoogleSetting();
    }

    public class GoogleSetting
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }

}
