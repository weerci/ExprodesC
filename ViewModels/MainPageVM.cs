using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.Views.Controls;
using ExprodesC.Views.Wrappers;
using FluentAvalonia.UI.Controls;
using Func;
using Func.Meta;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ExprodesC.ViewModels;

public class MainPageVM() : BaseVM
{
    readonly IDisposable _cleanUp = null!;
    readonly IDialogService _dialogService = null!;
    readonly static Node? _profiles = new(new(new(Lang.Resources.cap_profiles, []) { Id = Node.GuidProfile }));
    readonly static Node? _controls = new(new(new(Lang.Resources.cap_contols, []) { Id = Node.GuidControl }));
    readonly static Node core = new(
            new(new(Lang.Resources.cap_genotypes, []) { Id = Node.GuidGenotypes }),
            new([_profiles, _controls]));
    public MainPageVM(IProject project, IDialogService dialogService) : this()
    {
        Project = project;
        _dialogService = dialogService;

        var canAddGenotype = this
            .WhenAnyValue(x => x.Project.CurrentGenotype)
            .Select(curr => { return curr?.Genotype.Id == Node.GuidProfile || curr?.Genotype.Id == Node.GuidControl; });
        var canAddLoader = canAddGenotype.Subscribe(x => NodeSelected = x);

        var canDelGenotype = this
            .WhenAnyValue(x => x.Project.CurrentGenotype)
            .Select(curr =>
            {
                if (curr == null)
                    return false;
                //TODO Не отображается название "Новый проект" или название файла проекта 
                return
                    curr?.Genotype.Id != Node.GuidGenotypes &&
                    curr?.Genotype.Id != Node.GuidProfile &&
                    curr?.Genotype.Id != Node.GuidControl;
            });
        var canDelLoader = canDelGenotype.Subscribe(b => ListSelected = b);

        var profilesLoader = Project.Genotypes
            .Filter(g =>
            !g.IsControl)
            .Transform(g => new Node(g))
            .Sort(SortExpressionComparer<Node>.Ascending(n => n.GenotypeWR.Genotype.Name))
            .Bind(out _profiles!._SubNodes)
            .Subscribe();

        var controlLoader = Project.Genotypes
           .Filter(g => g.IsControl)
           .Transform(g => new Node(g))
           .Sort(SortExpressionComparer<Node>.Ascending(n => n.GenotypeWR.Genotype.Name))
           .Bind(out _controls!._SubNodes)
           .Subscribe();

        var selectedLoader = Project.Genotypes
            .AutoRefresh(vm => vm.IsSelected)
            .Filter(g => g.IsSelected)
            .Bind(SelectedProfiles)
            .Subscribe();

        this.WhenAnyValue(vm => vm.SelectedNode)
            .Subscribe(n =>
                Project.CurrentGenotype = n?.GenotypeWR
            );

        AddGenotypeCommand = ReactiveCommand.CreateFromTask(addGenotype, canAddGenotype);
        DelGenotypeCommand = ReactiveCommand.Create(delGenotype, canDelGenotype);
        EditGenotypeCommand = ReactiveCommand.CreateFromTask<GenotypeWR>(editGenotype, canDelGenotype);
        SelectGenotypeCommand = ReactiveCommand.Create(selectGenotype);
        _cleanUp = new CompositeDisposable(canDelLoader, canAddLoader, profilesLoader, controlLoader, selectedLoader);

        // Обновляем глобальные строки при изменении столбцов
        this.WhenAnyValue(x => x.Columns.Count)
            .Subscribe(_ => UpdateGlobalLocusRows());

        AddColumnCommand = ReactiveCommand.Create<GenotypeWR>(AddColumn);
        RemoveColumnCommand = ReactiveCommand.Create<GenotypeColumnVM>(RemoveColumn);
    }

    #region Properties

    public IProject Project { get; set; } = null!;

    public ObservableCollection<GenotypeColumnVM> Columns { get; } = new();

    public ObservableCollection<LocusRow> GlobalLocusRows { get; } = new();

    public IObservableCollection<GenotypeWR> SelectedProfiles { get; } = new ObservableCollectionExtended<GenotypeWR>();

    /// <summary>
    /// Иерархический список содержащий загруженные в проект генотипы.
    /// Состоит из двух частей - профили и контроли. 
    /// </summary>
    public ObservableCollection<Node> Nodes { get; } = [core];

    /// <summary>
    /// Выбранный в иерархическом списке <see cref="Nodes"/> элемент
    /// </summary>
    [Reactive]
    public Node? SelectedNode
    {
        get;
        set;
    }

    /// <summary>
    /// Выбран узловой элемент иерарахического списка
    /// </summary>
    [Reactive] public bool NodeSelected { get; set; }

    /// <summary>
    /// Выбран лист списка (профиль или контрол)
    /// </summary>
    [Reactive] public bool ListSelected { get; set; }
    
    private GenotypeColumnVM? DraggedColumn { get; set; }


    #endregion

    #region Command

    /// <summary>
    /// Команда удаления генотипа из иерархического списка
    /// </summary>
    public RxCommandUnit? DelGenotypeCommand { get; }

    /// <summary>
    /// Команда добаления генотипа в иерархический список
    /// </summary>
    public RxCommandUnit? AddGenotypeCommand { get; }

    /// <summary>
    /// Выбор генотипа в таблицу для работы
    /// </summary>
    public RxCommandUnit SelectGenotypeCommand { get; } = null!;

    /// <summary>
    /// Отркрывается окно для редактирования генотипа
    /// </summary>
    public RxCommandGenotype? EditGenotypeCommand { get; }

    public RxCommandGenotype AddColumnCommand { get; } = null!;
    public RxCommandGolumnVM RemoveColumnCommand { get; } = null!;

    #endregion

    #region Функции реализующие команды представления

    private void AddColumn(GenotypeWR genotype)
    {
        if (Columns.Any(c => c.GenotypeWR == genotype)) return;

        var column = new GenotypeColumnVM(genotype, this);
        Columns.Add(column);
    }

    private void RemoveColumn(GenotypeColumnVM column)
    {
        Columns.Remove(column);
    }

    public void StartDrag(GenotypeColumnVM column)
    {
        DraggedColumn = column;
    }

    public void HandleDrop(GenotypeColumnVM targetColumn)
    {
        if (DraggedColumn == null || DraggedColumn == targetColumn) return;

        int oldIndex = Columns.IndexOf(DraggedColumn);
        int newIndex = Columns.IndexOf(targetColumn);

        Columns.Move(oldIndex, newIndex);
        DraggedColumn = null;
    }
    //TODO Необходимо сделать перетаскивание столбцов
    //TODO Необходимо сделать расцветку по совпадающим аллелям

    private void UpdateGlobalLocusRows()
    {
        // Собираем все уникальные локусы из всех столбцов
        var allLoci = Columns
            .SelectMany(c => c.GenotypeWR.Genotype.Genomes)
            .Select(g => g.Locus)
            .Distinct()
            .ToList();

        // Создаем строки для каждого локуса
        var newRows = allLoci.Select(locus => new LocusRow(locus.Name)).ToList();

        // Синхронизация строк
        foreach (var row in newRows)
        {
            var existing = GlobalLocusRows.FirstOrDefault(r => r.LocusName == row.LocusName);
            if (existing == null)
            {
                GlobalLocusRows.Add(row);
            }
        }

        // Удаляем старые строки
        for (int i = GlobalLocusRows.Count - 1; i >= 0; i--)
        {
            if (!newRows.Any(r => r.LocusName == GlobalLocusRows[i].LocusName))
            {
                GlobalLocusRows.RemoveAt(i);
            }
        }

        // Обновляем все столбцы
        foreach (var column in Columns)
        {
            column.UpdateGenomeRows(GlobalLocusRows);
        }
    }

    async Task editGenotype(GenotypeWR genotype)
    {
        var edtiGenotype = await _dialogService.DialogGenotype(new AddEditGenotype(genotype), Lang.Resources.cap_edit_genotype);
        if (edtiGenotype == null)
            return;
        UpdateGlobalLocusRows();
    }
    //TODO Форма диалога не закрывается при нажатии на Enter
    async Task addGenotype()
    {
        var isControl = SelectedNode?.GenotypeWR.Genotype.Id == Node.GuidControl;

        var newGenotypeWR = await _dialogService.DialogGenotype(new AddEditGenotype(GenotypeWR.Empty(isControl)), Lang.Resources.cap_add_genotype);
        if (newGenotypeWR == null)
            return;

        void setCurrentNode(GenotypeWR g, bool isControl)
        {
            SelectedNode = isControl ?
                _controls?.SubNodes?.FirstOrDefault(n => n.GenotypeWR == g) :
                _profiles?.SubNodes?.FirstOrDefault(n => n.GenotypeWR == g);
        }

        Project.AddGenotypes([newGenotypeWR]).Match(g => setCurrentNode(g, isControl));

    }

    async void selectGenotype()
    {
        if (Project.CurrentGenotype != null)
            await Task.Run(() => Project.CurrentGenotype.IsSelected = true);
    }

    private async void delGenotype()
    {
        var res = await _dialogService.ConfirmDeleteGenotype(
            string.Format(Lang.Resources.conf_delete_genotype, SelectedNode?.GenotypeWR.Genotype.Name));

        if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
            return;

        if (SelectedNode != null)
        {
            var isControl = SelectedNode.GenotypeWR.IsControl;
            var idx = isControl ?
               _controls?.SubNodes?.IndexOf(SelectedNode) ?? 0 :
               _profiles?.SubNodes?.IndexOf(SelectedNode) ?? 0;

            var nextNode = idx switch
            {
                > 0 => isControl ? _controls?.SubNodes?.ElementAt(idx - 1) : _profiles?.SubNodes?.ElementAt(idx - 1),
                _ => isControl ? _controls : _profiles
            };

            Project.RemoveGenotypes([SelectedNode.GenotypeWR]);
            SelectedNode = nextNode;
        }

        #endregion
    }

    public class Node
    {
        /// <summary>
        /// Верхний уровень дерева генотипов содежит название "Генотипы"
        /// </summary>
        public static string GuidGenotypes => "{CF211558-2576-4B6A-B612-E990C949A041}";
        /// <summary>
        /// Поддерево "Профили"
        /// </summary>
        public static string GuidProfile => "{CF211558-2576-4B6A-B612-E990C949A042}";
        /// <summary>
        /// Поддерево "Эксперты/контроли"
        /// </summary>
        public static string GuidControl => "{CF211558-2576-4B6A-B612-E990C949A043}";

        public ReadOnlyObservableCollection<Node>? SubNodes => _SubNodes;
        public ReadOnlyObservableCollection<Node>? _SubNodes;
        public GenotypeWR GenotypeWR { get; }

        /// <summary>
        /// True - если выбранный node является предопределенной папкой (генотипы, проект, контроль/эксперт)
        /// </summary>
        public bool IsFolder =>
            GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A041}" ||
            GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A042}" ||
            GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A043}";

        public Node(GenotypeWR genotypeWr)
        {
            GenotypeWR = genotypeWr;
        }

        public Node(GenotypeWR genotypeWR, ReadOnlyObservableCollection<Node> subNodes)
        {
            GenotypeWR = genotypeWR;
            _SubNodes = subNodes;
        }

    }
}
