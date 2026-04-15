namespace VeloPass.Application.Abstractions;

public interface IEmailSender
{
    Task SendTemplatedAsync(
        string toEmail,
        string templateId,
        object templateData,
        CancellationToken cancellationToken = default);
}