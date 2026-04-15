using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using VeloPass.Application.Abstractions;

namespace VeloPass.Infrastructure.Email;

internal sealed class EmailSender(IOptions<EmailOptions> options): IEmailSender
{
    private readonly EmailOptions _options = options.Value;
    
    public async Task SendTemplatedAsync(
        string toEmail, 
        string templateId, 
        object templateData,
        CancellationToken cancellationToken = default)
    {
        var client = new SendGridClient(_options.ApiKey);
        var from = new EmailAddress(_options.FromEmail, _options.FromName);
        var message = MailHelper.CreateSingleTemplateEmail(from, new EmailAddress(toEmail), templateId, templateData);
        
        var response = await client.SendEmailAsync(message, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
        }
    }
}