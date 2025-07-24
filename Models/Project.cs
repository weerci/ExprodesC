using Calc.Data;
using Calc.Deprecated;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Services;
using ExprodesC.Views.Wrappers;
using Func;
using Func.Meta;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ExprodesC.Models;

public class Project : IProject
{
    readonly IGenotypeStore _genotypeStore;
    readonly IDisposable _cleanUp;
    readonly SourceList<GenotypeWR> _genotypes = new();

    public Project(IGenotypeStore genotypeStore)
    {
        _genotypeStore = genotypeStore;
        ReloadFromDb();

        Genotypes = _genotypes.Connect()
            .Publish();

        _cleanUp = new CompositeDisposable(Genotypes.Connect());
    }

    /// <inheritdoc/>
    public IConnectableObservable<IChangeSet<GenotypeWR>> Genotypes { get; }

    /// <inheritdoc/>
    public bool IsChanged { get; } 

    /// <inheritdoc/>
    public Ex<GenotypeWR> AddGenotypes(IEnumerable<GenotypeWR> gs) => _genotypes.Try(g => { g.AddRange(gs); return gs.First(); });

    /// <inheritdoc/>
    public Ex<bool> LoadFromFile(Ex<FileName> fn) => _genotypeStore.TryBool(gs => {
        gs.LoadFromFile(fn);
        _genotypes.AddRange(_genotypeStore.Genotypes.Select(g => new GenotypeWR(g)));
    });

    /// <inheritdoc/>
    public Ex<bool> ReloadFromDb() => _genotypeStore.TryBool(gs => {
        gs.LoadControls();
        _genotypes.AddRange(_genotypeStore.Genotypes.Select(g => new GenotypeWR(g, true)));
    });

    /// <inheritdoc/>
    public Ex<bool> RemoveGenotypes(IEnumerable<GenotypeWR> genotypes) => _genotypes.TryBool(g => g.RemoveMany(genotypes));

    /// <inheritdoc/>
    public Ex<bool> Save(Ex<FileName> fn) => _genotypes.TryBool(gs => {
        // Сохраняем экспертов
        _genotypeStore.Genotypes.Clear();
        _genotypeStore.Genotypes.AddRange(gs.Items.Where(g => g.IsControl).Select(g => g.Genotype));
        _genotypeStore.SaveControls();

        // Сохраняем профили
        _genotypeStore.Genotypes.Clear();
        _genotypeStore.Genotypes.AddRange(gs.Items.Where(g => !g.IsControl).Select(g => g.Genotype));
        _genotypeStore.SaveToFile(fn);

    });

}
