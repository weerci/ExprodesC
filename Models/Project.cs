using Calc.Data;
using Calc.Deprecated;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Services;
using ExprodesC.Views.Controls;
using ExprodesC.Wrappers;
using Func;
using Func.Impl;
using Func.Meta;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExprodesC.Models;

public class Project : ReactiveObject, IProject
{
    readonly IGenotypeStore _genotypeStore;
    readonly IDialogService _dialogService;
    readonly IDisposable _cleanUp;
    readonly SourceList<GenotypeWR> _genotypes = new();
    readonly SourceList<GenotypeColumnVM> _selected = new();

    public Project(IGenotypeStore genotypeStore, IDialogService dialogService)
    {
        _genotypeStore = genotypeStore;
        _dialogService = dialogService;
        ReloadFromDb();
        IsChanged = false; // Необходимо, поскольку ReloadFromDb() устанавливает проект как измененный

        Genotypes = _genotypes.Connect().Do(_ => { HasGenotype = _genotypes.Count > 0;}).Publish();

        var selectedControl = Genotypes
           .AutoRefresh(ar => ar.IsSelected)
           .Filter(g => g.IsSelected)
           .Transform(t => new GenotypeColumnVM(t, this))
           .Bind(out _selectedGolumns)
           .Subscribe(s =>
           {
               SelectedCount = _selectedGolumns.Count;
               HasSelected = _selectedGolumns.Count > 0;
               UpdateAllGenomeRows();
           });

        _cleanUp = new CompositeDisposable(Genotypes.Connect(), selectedControl);

    }
    /// <inheritdoc/>
    [Reactive] public int SelectedCount { get; set; }

    /// <inheritdoc/>
    [Reactive] public bool HasGenotype { get; set; }

    //public IObservableCollection<GenotypeColumnVM> SelectedGolumns { get; } = new ObservableCollectionExtended<GenotypeColumnVM>();
    public ReadOnlyObservableCollection<GenotypeColumnVM> SelectedGolumns => _selectedGolumns;
    public ReadOnlyObservableCollection<GenotypeColumnVM> _selectedGolumns;
    /// <inheritdoc/>
    [Reactive] public GenotypeWR? CurrentGenotype { get; set; }

    /// <inheritdoc/>
    public IConnectableObservable<IChangeSet<GenotypeWR>> Genotypes { get; }

    /// <inheritdoc/>
    [Reactive] public FileName? PathToSavedFile { get; set; }

    /// <inheritdoc/>
    [Reactive] public bool HasSelected { get; set; }

    /// <inheritdoc/>
    [Reactive] public bool IsChanged { get; set; }

    /// <inheritdoc/>
    public Ex<GenotypeWR> AddGenotypes(IEnumerable<GenotypeWR> gs) => _genotypes.Try(g =>
    {
        g.AddRange(gs);
        IsChanged = true;
        return gs.First();
    });

    /// <inheritdoc/>
    public Ex<bool> LoadFromFile(Ex<FileName> fn) => _genotypeStore.TryBool(gs =>
    {
        gs.LoadFromFile(fn);
        _genotypes.AddRange(_genotypeStore.Genotypes.Select(g => new GenotypeWR(g)));
        IsChanged = true;
    });

    /// <inheritdoc/>
    public Ex<bool> ReloadFromDb() => _genotypeStore.TryBool(gs =>
    {
        gs.LoadControls();
        _genotypes.AddRange(_genotypeStore.Genotypes.Select(g => new GenotypeWR(g, true)));
        IsChanged = true;
    });

    /// <inheritdoc/>
    public Ex<bool> RemoveGenotypes(IEnumerable<GenotypeWR> genotypes) => _genotypes.TryBool(g =>
    {
        g.RemoveMany(genotypes);
        IsChanged = true;
    });

    /// <inheritdoc/>
    public Ex<bool> Save(Ex<FileName> fn) => _genotypes.TryBool(gs =>
    {
        // Сохраняем экспертов
        _genotypeStore.Genotypes.Clear();
        _genotypeStore.Genotypes.AddRange(gs.Items.Where(g => g.IsControl).Select(g => g.Genotype));
        _genotypeStore.SaveControls();

        // Сохраняем профили
        _genotypeStore.Genotypes.Clear();
        _genotypeStore.Genotypes.AddRange(gs.Items.Where(g => !g.IsControl).Select(g => g.Genotype));
        _genotypeStore.SaveToFile(fn);

        IsChanged = false;
        PathToSavedFile = fn.Right!;
    });

    /// <inheritdoc/>
    public Ex<bool> Close() => _genotypes.TryBool(g =>
    {
        g.Clear();
        IsChanged = false;
        HasSelected = false;
        PathToSavedFile = null;
    });

    /// <inheritdoc/>
    public async void EditGenotype(GenotypeWR genotype)
    {
        await _dialogService.DialogGenotype(new AddEditGenotype(genotype), Lang.Resources.cap_edit_genotype);
        UpdateAllGenomeRows();
    }

    private void UpdateAllGenomeRows()
    {
        if (!SelectedGolumns.Any()) return;

        // Получаем глобальный набор уникальных локусов
        var ls = SelectedGolumns
            .SelectMany(vm => vm.GenotypeWR.Genotype.Genomes.Select(g => g.Locus))
            .Distinct()
            .OrderBy(l => l.Ord())
            .ToList();

        // Обновляем все колонки
        foreach (var column in SelectedGolumns)
        {
            column.UpdateGenomeRows(ls);
        }
    }


}
