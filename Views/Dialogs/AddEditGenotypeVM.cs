using Calc.Data;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.ViewModels;
using ExprodesC.Views.Wrappers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace ExprodesC.Views.Dialogs;

public class AddEditGenotypeVM : BaseVM
{
    readonly SourceList<LocusAlleleWR> _locusAlleles = new();
    readonly IDisposable _cleanUp;

    public AddEditGenotypeVM(GenotypeWR genotypeWR) : this()
    {
        GenotypeWR = genotypeWR;

        if (GenotypeWR != null)
        {
            Name = GenotypeWR.Genotype.Name;

            if (GenotypeWR.Genotype.Genomes.Count() > 0)
            {
                List<LocusAlleleWR> lavm = [];
                foreach (Genome g in GenotypeWR.Genotype.Genomes)
                {
                    string lName = g.Locus.Name;
                    foreach (var a in g.Alleles)
                    {
                        LocusAllele la = new(lName, 0, false, 0, a.Name, 0, 0);
                        lavm.Add(new LocusAlleleWR(la) { IsChecked = true });
                    }
                }
                updateLocusAlleles(lavm);
            }
        }
    }
    public AddEditGenotypeVM()
    {

        _locusAlleles.AddRange(LocusAlleleWR.FromLocusAlleles(HL.Dic.Values));

        var laVM = _locusAlleles.Connect()
            .Sort(SortExpressionComparer<LocusAlleleWR>.Ascending(la => la.LocusAllele.LOrd).ThenBy(la => la.AOrd))
            .Publish();

        // Общий список всех локусов
        var _listAvailableLoader = laVM
            .GroupOn(la => la.LocusAllele.LName)
            .Transform(g => g.GroupKey)
            .Bind(out _listAvailable)
            .Subscribe();

        // Аллели выбранного локуса
        var filterAlleleForLocus = this
           .WhenAnyValue(x => x.SelectedLocus)
           .Select(selectedLocus => (Func<LocusAlleleWR, bool>)(item =>
               !string.IsNullOrEmpty(selectedLocus) &&
               item.LocusAllele.LName == selectedLocus))
           ;

        var _listAllelesForLocus = laVM.Filter(filterAlleleForLocus)
            .Bind(out _listAlleles)
            .Subscribe();

        // Выбранные локусы
        var _listSelectedLoader = laVM
            .AutoRefresh(la => la.IsChecked)
            .Filter(la => la.IsChecked)
            .GroupOn(la => la.LocusAllele.LName)
            .Transform(group =>
            {
                // Создаем объект Genome для этой группы
                var genome = Genome.NewGenome(group.GroupKey, group.List.Items.Select(n => n.AsAllele()));

                // Получаем все элементы группы
                IObservableList<LocusAlleleWR>? lvms = group.List;

                // Создаем объект GenomeVM
                return new GenomeWR(genome, lvms);
            })
            .Sort(SortExpressionComparer<GenomeWR>.Ascending(g => g.Genome.Locus.Name))
            .Bind(out _listSelected).Subscribe();


        var _canSaveLoader = this.WhenAnyValue(vm => vm.Name)
            .Subscribe(n => CanSave = !string.IsNullOrWhiteSpace(n));

        var _genomeSyncLoader = this.WhenAnyValue(vm => vm.SelectedGenome)
            .Subscribe(g =>
            {
                if (SelectedLocus != null && g != null)
                    SelectedLocus = g.Genome.Locus.Name;
            });

        _cleanUp = new CompositeDisposable(_locusAlleles, _listAvailableLoader, _listSelectedLoader, _listAllelesForLocus, _canSaveLoader, laVM.Connect());

        SelectedLocus = ListAvailable.Count > 0 ? ListAvailable[0] : null;

    }

    #region Properties

    [Reactive] public string? SelectedLocus { get; set; }
    [Reactive] public GenomeWR? SelectedGenome { get; set; }
    [Reactive] public string? Name { get; set; }
    public GenotypeWR? GenotypeWR { get; }

    public ReadOnlyObservableCollection<string> ListAvailable => _listAvailable;
    public ReadOnlyObservableCollection<string> _listAvailable;

    public ReadOnlyObservableCollection<LocusAlleleWR>? ListAlleles => _listAlleles;
    public ReadOnlyObservableCollection<LocusAlleleWR>? _listAlleles;

    public ReadOnlyObservableCollection<GenomeWR>? ListSelected => _listSelected;
    public ReadOnlyObservableCollection<GenomeWR>? _listSelected;

    #endregion

    #region Helper

    private void updateLocusAlleles(IEnumerable<LocusAlleleWR> attachedLocusAllele)
    {
        _locusAlleles.Edit(updater =>
        {
            foreach (var u in updater)
                foreach (var a in attachedLocusAllele)
                    if (a.LocusAllele.LName == u.LocusAllele.LName && a.LocusAllele.AName == u.LocusAllele.AName)
                        u.IsChecked = a.IsChecked;
        });
    }

    #endregion
}

    

