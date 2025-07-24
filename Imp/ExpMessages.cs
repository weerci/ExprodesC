using DynamicData;
using ExprodesC.Models;
using ExprodesC.Services;
using FluentAvalonia.UI.Controls;

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

    public void SendMessage(string message, string caption) => _messages.Add(new() { Caption = caption, Text = message });

    public void SendExpMessage(ExpMessage expMessage) => _messages.Add(expMessage);
}
