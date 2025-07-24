

using FluentAvalonia.UI.Controls;

namespace ExprodesC.Models;


public record ExpMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Dt { get; } = DateTime.Now;
    public string Caption { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InfoBarSeverity Severity { get; set; } = InfoBarSeverity.Informational;
    public Exception? Error { get; set; }
}




