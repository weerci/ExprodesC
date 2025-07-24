using Avalonia;
using Avalonia.ReactiveUI;

namespace ExprodesC;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        RxApp.DefaultExceptionHandler = Observer.Create<Exception>(GlobalErrorHandler);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    }

    public static void GlobalErrorHandler(Exception exception)
    {
        Init.ExpMessages.SendError("Unhandled error!", exception);
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();

}
