using ExprodesC.Services;
using Func.Services;

namespace ExprodesC.ViewModels;

public class BaseVM : ReactiveObject, IDisposable, ICanSave
{
    protected IExpMessages Log => Init.ExpMessages;

    [Reactive] public bool CanSave { get; set; }

    #region Dispose

    private bool disposed = false;

    // реализация интерфейса IDisposable.
    public void Dispose()
    {
        // освобождаем неуправляемые ресурсы
        Dispose(true);
        // подавляем финализацию
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;
        if (disposing)
        {
            // Освобождаем управляемые ресурсы
        }
        // освобождаем неуправляемые объекты
        disposed = true;
    }

    // Деструктор
    ~BaseVM()
    {
        Dispose(false);
    }

    #endregion

}
