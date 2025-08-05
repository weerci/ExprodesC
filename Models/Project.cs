using Calc.Data;
using Calc.Deprecated;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Services;
using ExprodesC.Views.Controls;
using ExprodesC.Views.Wrappers;
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
    readonly IDisposable _cleanUp;
    readonly SourceList<GenotypeWR> _genotypes = new();
    readonly SourceList<GenotypeColumnVM> _selected = new();

    public Project(IGenotypeStore genotypeStore)
    {
        _genotypeStore = genotypeStore;
        ReloadFromDb();
        IsChanged = false; // Необходимо, поскольку ReloadFromDb() устанавливает проект как измененный

        Genotypes = _genotypes.Connect().Publish();

        var selectedControl = _genotypes.Connect()
           .AutoRefresh(ar => ar.IsSelected)
           .Filter(g => g.IsSelected)
           .Do(d =>
           {
               var curr = d.First().Item.Current;
               if (curr.IsSelected == true)
                   SelectedGolumns.Add(new GenotypeColumnVM(curr, this));
               else
               {
                   var curColumn = SelectedGolumns.FirstOrDefault(n => n.GenotypeWR.Genotype.Id == curr.Genotype.Id);
                   if (curColumn != null)
                       SelectedGolumns.Remove(curColumn);
               }
               Debug.WriteLine(d.Count + "   " + d.First().Item.Current.Genotype.Name);
           })
           .Subscribe(s => HasSelected = SelectedGolumns.Any(n => n.GenotypeWR.IsSelected));

        SelectedGolumns.CollectionChanged += (s, e) => UpdateAllGenomeRows();

        _cleanUp = new CompositeDisposable(Genotypes.Connect(), selectedControl);

    }

    public IObservableCollection<GenotypeColumnVM> SelectedGolumns { get; } = new ObservableCollectionExtended<GenotypeColumnVM>();

    /// <inheritdoc/>
    [Reactive]
    public GenotypeWR? CurrentGenotype { get; set; }

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
