using System.Collections.Concurrent;
using PracticeBookingSystem.Services.Interfaces;
namespace PracticeBookingSystem.Services;

public record EmailMessage(DateTime SentAt, string To, string Subject, string Body);
public class InMemoryEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<EmailMessage> _messages = new();
    public IReadOnlyCollection<EmailMessage> Messages => _messages.ToArray();
    public Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _messages.Enqueue(new EmailMessage(DateTime.Now, to, subject, body));
        return Task.CompletedTask;
    }
}
