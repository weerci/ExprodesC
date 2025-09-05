using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.Views.Controls;
using ExprodesC.Wrappers;
using FluentAvalonia.UI.Controls;
using Func;
using Func.Meta;
using Func.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ExprodesC.ViewModels;

public class MainPageVM() : BaseVM, IDisposable
{
    private readonly IDisposable _cleanUp;
    private readonly IDialogService _dialogService;
    private readonly ISettingsProvider<ExSettingData> _settings;

    public MainPageVM(IProject project, IDialogService dialogService, ISettingsProvider<ExSettingData> settings) : this()
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        var canAddGenotype = this
            .WhenAnyValue(x => x.Project.CurrentGenotype)
            .Select(curr => curr?.Genotype.Id == GenotypeNode.GuidProfile || curr?.Genotype.Id == GenotypeNode.GuidControl);
        var canAddLoader = canAddGenotype.Subscribe(x => NodeSelected = x);

        var canDelGenotype = this
            .WhenAnyValue(x => x.Project.CurrentGenotype)
            .Select(curr =>
            {
                if (curr == null)
                    return false;
                return curr.Genotype.Id != GenotypeNode.GuidGenotypes &&
                       curr.Genotype.Id != GenotypeNode.GuidProfile &&
                       curr.Genotype.Id != GenotypeNode.GuidControl;
            });
        var canDelLoader = canDelGenotype.Subscribe(b => ListSelected = b);

        var profilesLoader = Project.Genotypes
            .Filter(g => !g.IsControl)
            .Transform(g => new GenotypeNode(g))
            .Sort(SortExpressionComparer<GenotypeNode>.Ascending(n => n.GenotypeWR.Genotype.Name))
            .Bind(out GenotypeNode.Profiles!._SubNodes)
            .Subscribe();

        var controlLoader = Project.Genotypes
           .Filter(g => g.IsControl)
           .Transform(g => new GenotypeNode(g))
           .Sort(SortExpressionComparer<GenotypeNode>.Ascending(n => n.GenotypeWR.Genotype.Name))
           .Bind(out GenotypeNode.Controls!._SubNodes)
           .Subscribe();

        this.WhenAnyValue(vm => vm.SelectedNode)
            .Subscribe(n => Project.CurrentGenotype = n?.GenotypeWR);

        AddGenotypeCommand = ReactiveCommand.CreateFromTask(addGenotype, canAddGenotype);
        DelGenotypeCommand = ReactiveCommand.CreateFromTask(delGenotype, canDelGenotype);
        EditGenotypeCommand = ReactiveCommand.CreateFromTask<GenotypeWR>(editGenotype, canDelGenotype);
        SelectGenotypeCommand = ReactiveCommand.Create<GenotypeWR>(Project.SelectGenotype, canDelGenotype);
        UnSelectGenotypeCommand = ReactiveCommand.Create<GenotypeWR>(Project.UnSelectGenotype, canDelGenotype);

        _cleanUp = new CompositeDisposable(canDelLoader, canAddLoader, profilesLoader, controlLoader);
    }

    #region Properties
    public IProject Project { get; }
    [Reactive] public GenotypeNode? SelectedNode { get; set; }
    [Reactive] public bool NodeSelected { get; set; }
    [Reactive] public bool ListSelected { get; set; }
    #endregion

    #region Commands
    public RxCommandUnit? DelGenotypeCommand { get; }
    public RxCommandUnit? AddGenotypeCommand { get; }
    public RxCommandGenotype? EditGenotypeCommand { get; }
    public RxCommandGenotype SelectGenotypeCommand { get; }
    public RxCommandGenotype UnSelectGenotypeCommand { get; }
    #endregion

    #region Command Logic
    private async Task editGenotype(GenotypeWR genotype)
        => await _dialogService.DialogGenotype(new AddEditGenotype(genotype), Lang.Resources.cap_edit_genotype);

    private async Task addGenotype()
    {
        var isControl = SelectedNode?.GenotypeWR.Genotype.Id == GenotypeNode.GuidControl;
        var newGenotypeWR = await _dialogService.DialogGenotype(new AddEditGenotype(GenotypeWR.Empty(isControl)), Lang.Resources.cap_add_genotype);
        if (newGenotypeWR == null)
            return;

        void setCurrentNode(GenotypeWR g, bool isControl)
        {
            SelectedNode = isControl ?
                GenotypeNode.Controls?.SubNodes?.FirstOrDefault(n => n.GenotypeWR == g) :
                GenotypeNode.Profiles?.SubNodes?.FirstOrDefault(n => n.GenotypeWR == g);
        }

        Project.AddGenotypes(new[] { newGenotypeWR }).Match(g => setCurrentNode(g, isControl));
    }

    private async Task delGenotype()
    {
        var res = await _dialogService.ConfirmDelete(_settings.Value.ConfirmDeleteGenotype,
            string.Format(Lang.Resources.conf_delete_genotype, SelectedNode?.GenotypeWR.Genotype.Name),
            b => { _settings.Value.ConfirmDeleteGenotype = !b; });

        if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
            return;

        if (SelectedNode != null)
        {
            var isControl = SelectedNode.GenotypeWR.IsControl;
            var idx = isControl ?
               GenotypeNode.Controls?.SubNodes?.IndexOf(SelectedNode) ?? 0 :
               GenotypeNode.Profiles?.SubNodes?.IndexOf(SelectedNode) ?? 0;

            var nextNode = idx switch
            {
                > 0 => isControl ? GenotypeNode.Controls?.SubNodes?.ElementAt(idx - 1) : GenotypeNode.Profiles?.SubNodes?.ElementAt(idx - 1),
                _ => isControl ? GenotypeNode.Controls : GenotypeNode.Profiles
            };

            Project.RemoveGenotypes(new[] { SelectedNode.GenotypeWR });
            SelectedNode = nextNode;
        }
    }
    #endregion

    #region IDisposable
    public void Dispose()
    {
        _cleanUp?.Dispose();
        GC.SuppressFinalize(this);
    }
    #endregion
}
