using ExprodesC.Imp;
using ExprodesC.Services;

namespace ExprodesC
{
    public static class Init
    {
        readonly static ServiceProvider _services;
        static Init()
        {
            IServiceCollection _collection = new ServiceCollection()
                .AddSingleton<IExpMessages, ExpMessages>();

            _services = _collection.BuildServiceProvider();
        }
        public static IExpMessages ExpMessages => _services.GetRequiredService<IExpMessages>();
    }
}
