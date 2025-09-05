using DynamicData;
using ExprodesC.Models;

namespace ExprodesC.Services;

/// <summary>
/// Предоставляет доступ к функционалу отправки/получения сообщений и ошибок
/// </summary>
public interface IExpMessages
{
    /// <summary>
    /// Список возникших ошибок
    /// </summary>
    public IObservable<IChangeSet<ExpMessage>> Errors { get; }

    /// <summary>
    /// Список отправленных сообщений
    /// </summary>
    public IObservable<IChangeSet<ExpMessage>> Messages { get; }

    /// <summary>
    /// Отправление ошибки
    /// </summary>
    /// <param name="caption">Заголовок об ошибке</param>
    /// <param name="err">Ошибка</param>
    public void SendError(string caption, Exception err);

    /// <summary>
    /// Отправление сообщения
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="caption">Заголовок сообщения</param>
    public void SendMessage(string message, string caption);

    /// <summary>
    /// Отправление предупреждения
    /// </summary>
    /// <param name="message">Предупреждение</param>
    /// <param name="caption">Заголовок предупреждения</param>
    public void SendWarrning(string message, string caption);

    /// <summary>
    /// Отправление сообщения об удачном завершении задачи
    /// </summary>
    /// <param name="message">Сообщение об удачном завершении</param>
    /// <param name="caption">Заголовок сообщения</param>
    public void SendSuccess(string message, string caption);

    /// <summary>
    /// Отправление нескольких сообщений (объединяются в строку с разделителями /n)
    /// </summary>
    /// <param name="messages">Сообщениия</param>
    /// <param name="caption">Заголовок сообщения</param>
    public void SendMessages(IEnumerable<string> messages, string caption);

    /// <summary>
    /// Отправления произвольного сообщения
    /// </summary>
    /// <param name="expMessage">Произвольное сообщение</param>
    public void SendExpMessage(ExpMessage expMessage);

}