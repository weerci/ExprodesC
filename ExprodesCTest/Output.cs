using System.Diagnostics;

public class OutputTests
{
    /*  private readonly ITestOutputHelper _output;

      public MyTests(ITestOutputHelper output)
      {
          _output = output;
      }

      [Fact]
      public void TestMethod1()
      {
          _output.WriteLine("This is a test output.");
          Assert.Equal(1, 1);
      }*/
    [Fact]
    public void out_test()
    {
        // для отображения данных внести в файл xunit.runner.json строку "diagnosticMessages": true
        // перезапустить visual studio
        var ers = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList();
        ers.ForEach(n => { Debug.WriteLine(n); }); // Выводит данные в меню отладка
        ers.ForEach(n => { Console.WriteLine(n); }); // Выводит данные в меню тесты

    }
}