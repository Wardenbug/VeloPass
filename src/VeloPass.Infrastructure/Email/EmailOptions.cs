namespace VeloPass.Infrastructure.Email;

public class EmailOptions
{
    public const string SectionName = "Email";
    
    public string ApiKey { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName { get; init; } = "VeloPass";
}