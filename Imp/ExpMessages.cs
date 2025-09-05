using DynamicData;
using ExprodesC.Models;
using ExprodesC.Services;
using FluentAvalonia.UI.Controls;
using System.Linq;

namespace ExprodesC.Imp;

/// <inheritdoc/>
public class ExpMessages : IExpMessages
{
    private readonly SourceList<ExpMessage> _errors = new();
    private readonly SourceList<ExpMessage> _messages = new();

    /// <inheritdoc/>
    public IObservable<IChangeSet<ExpMessage>> Errors => _errors.Connect();
    /// <inheritdoc/>
    public IObservable<IChangeSet<ExpMessage>> Messages => _messages.Connect();

    /// <inheritdoc/>
    public void SendError(string caption, Exception err) => _errors.Add(
        new()
        {
            Caption = caption,
            Text = err.Message,
            Severity = InfoBarSeverity.Error,
            Error = err
        });

    /// <inheritdoc/>
    public void SendWarrning(string caption, string message) => _messages.Add(
        new ()
        {
            Caption = caption,
            Text = message,
            Severity = InfoBarSeverity.Warning,
        });

    /// <inheritdoc/>
    public void SendSuccess(string caption, string message) => _messages.Add(
        new()
        {
            Caption = caption,
            Text = message,
            Severity = InfoBarSeverity.Success,
        });

    /// <inheritdoc/>
    public void SendMessage(string caption, string message) => _messages.Add(new() { Caption = caption, Text = message });
    
    /// <inheritdoc/>
    public void SendMessages(IEnumerable<string> messages, string caption)
    {
        SendMessage(messages.Aggregate("", (n, next) => n + "./n" + next), caption);
    }

    /// <inheritdoc/>
    public void SendExpMessage(ExpMessage expMessage) => _messages.Add(expMessage);

}
