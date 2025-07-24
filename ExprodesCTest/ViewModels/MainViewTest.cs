using ExprodesC.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ExprodesCTest.ViewModels;

public class MainViewTest
{

    [Fact]
    public void Create_MainPage_test()
    {
        var mPage = Init.Services.GetRequiredService<MainPageVM>();

        Assert.NotNull(mPage);
    }
}
