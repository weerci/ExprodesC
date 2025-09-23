using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Calc;
using Calc.Calculation;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.ViewModels.Settings;
using ExprodesC.Wrappers;
using Func;
using Func.Services;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ExprodesC.ViewModels;

public class CalcPageVM() : BaseVM
{
    readonly IDisposable _cleanUp = null!;

    ICounter _counter = null!;
    readonly SourceList<ErWR> _erWr = new();

    public CalcPageVM(ISettingsProvider<ExSettingData> settings, IProject project, ICounter counter) : this()
    {
        Settings = settings;
        Project = project;
        _counter = counter;

        var selectedLoader = Project.SelectedGolumns.ToObservableChangeSet().Subscribe(s =>
        {
            if (Project.SelectedGolumns.Count == 0)
                SelectedCalcResearch = "";
        });

        var v = this.WhenAnyValue(x => x.SelectedCalcResearch).Subscribe(x =>
        {
            IsVisibleGrid = !string.IsNullOrEmpty(x);
        });

        var itemsLoader = _erWr.Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(Items)
            .Do(d => { this.RaisePropertyChanged(nameof(ShowReverse)); this.RaisePropertyChanged(nameof(ShowDirect)); })
            .Subscribe();

        CalcCommand = ReactiveCommand.CreateFromTask<string>(calc);
        CalcMethodCommand = ReactiveCommand.Create<string>(calcMethod);
        CloseCommand = ReactiveCommand.Create<GenotypeWR>((g) => { Project.UnSelectGenotype(g); });
        SelectGenotypeCommand = ReactiveCommand.Create<GenotypeWR>(Project.SelectGenotype);
        UnSelectGenotypeCommand = ReactiveCommand.Create<GenotypeWR>(Project.UnSelectGenotype);

        _cleanUp = new CompositeDisposable(itemsLoader, selectedLoader);

    }

    #region Properties
    public ISettingsProvider<ExSettingData> Settings { get; set; } = null!;

    public IProject Project { get; set; } = null!;

    /// <summary>
    /// True - если выбран расчет
    /// </summary>
    [Reactive] public bool IsVisibleGrid { get; set; }

    /// <summary>
    /// Содержит название расчета
    /// </summary>
    [Reactive] public string? SelectedCalcResearch { get; set; }

    /// <summary>
    /// Текущий генотип в дереве генотипов
    /// </summary>
    [Reactive] public GenotypeNode? SelectedNode { get; set; }

    /// <summary>
    /// Строки таблицы расчетов
    /// </summary>
    public IObservableCollection<ErWR> Items { get; } = new ObservableCollectionExtended<ErWR>();

    /// <summary>
    /// Если true - отображается столбец с прямым расчетом
    /// </summary>
    public bool ShowReverse => Items.Any(i => !string.IsNullOrEmpty(i.Er.RevFormula));

    /// <summary>
    /// Если true - отображается столбец с обратным расчетом
    /// </summary>
    public bool ShowDirect => Items.Any(i => !string.IsNullOrEmpty(i.Er.DirFormula));

    #endregion

    #region Command

    /// <summary>
    /// Команда закрытия окна генотипа
    /// </summary>
    public RxCommandGenotype CloseCommand { get; } = null!;

    /// <summary>
    /// Команда выбора расчета
    /// </summary>
    public RxCommandText? CalcCommand { get; }

    /// <summary>
    /// Команда выбора метода вычисления и запуска вычисления
    /// </summary>
    public RxCommandText? CalcMethodCommand { get; }

    /// <summary>
    /// Выбор генотипа в таблицу для работы
    /// </summary>
    public RxCommandGenotype SelectGenotypeCommand { get; } = null!;

    /// <summary>
    /// Удаление выбранного для работы элемента
    /// </summary>
    public RxCommandGenotype UnSelectGenotypeCommand { get; } = null!;

    #endregion

    #region Helper
    private async Task calc(string arg)
    {
        await Task.Run(() =>
        {
            SelectedCalcResearch = arg;

            var gs = Project.SelectedGolumns.Select(n => n.GenotypeWR.Genotype.ToEx());
            var res = _counter.Calculate(gs, SelectedCalcResearch);

            /*if (res.Where(n => n.IsException).Any())
                Log.SendMessages(res.Where(r => r.IsException).Select(n => n.Error!.Message), Lang.Resources.err_research_calc);*/

            if (res.Where(n => n.IsSuccess).Any())
            {
                _erWr.Clear();
                _erWr.AddRange(res.Where(n => n.IsSuccess).Select(n => new ErWR(n.Value!)));
            }

        });
    }

    private async void calcMethod(string method)
    {
        if (Enum.TryParse(method, out MethodCalc res) && !string.IsNullOrEmpty(SelectedCalcResearch))
        {
            TypeCalcEx.SetHashMethod(SelectedCalcResearch, res);
            await calc(SelectedCalcResearch);
        }
    }

    #endregion

}
