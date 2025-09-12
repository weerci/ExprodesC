using Calc.Data;
using Calc.Deprecated;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Models;
using ExprodesC.Services;
using ExprodesC.Wrappers;
using FluentAvalonia.UI.Controls;
using Func;
using Func.Extensions;
using Func.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.ViewModels;

public class LibraryPageVM() : BaseVM
{
    readonly SourceList<LocusAllele> _la = new();
    readonly IDialogService _dialogService = null!;
    readonly IAppDb _appDb = null!;
    IDisposable? _cleanUp;

    public LibraryPageVM(ISettingsProvider<ExSettingData> settings, IAppDb appDb, IDialogService dialogService) : this()
    {
        Settings = settings;
        _appDb = appDb;
        _dialogService = dialogService;

        _la.AddRange(appDb.GetAllPopLocuses());

        #region tunnel

        LocusesAlleles = _la.Connect().Publish();
        if (settings.Value.Populations != null)
            SelectedPopulation = settings.Value.Populations.FirstOrDefault(p => p.Id == settings.Value.CurrentPopulation.Id);

        // Локусы в зависимости от выбранной популяции
        var filterLocusByPopulation = this
           .WhenAnyValue(x => x.SelectedPopulation!.Id)
           .Select(idx => (Func<LocusAllele, bool>)(la => la.PopId == idx));

        var locusesLoader = LocusesAlleles.Filter(filterLocusByPopulation)
            .GroupOn(g => (g.LocusId, g.LName, g.AMinFreq, g.MutFreq, g.LCalc, g.LOrd, g.PopId))
            .Transform(k =>
                new LocusWR(k.GroupKey.LocusId, k.GroupKey.LName, k.GroupKey.AMinFreq, k.GroupKey.MutFreq, k.GroupKey.LCalc, k.GroupKey.LOrd, k.GroupKey.PopId))
            .Sort(SortExpressionComparer<LocusWR>.Ascending(l => l.Ord))
            .Bind(out _locuses)
            .Subscribe();

        // Аллели в зависимости от выбранного локуса
        var filterAlleleByLocus = this
           .WhenAnyValue(x => x.SelectedLocus)
           .Select(l => (Func<LocusAllele, bool>)(la =>
                l != null &&
                SelectedPopulation != null &&
                la.LName == l.Name &&
                la.AlleleId != 0 &&
                la.PopId == SelectedPopulation.Id));

        var allelesLoader = LocusesAlleles.Filter(filterAlleleByLocus)
           .Sort(SortExpressionComparer<LocusAllele>.Ascending(la => new Allele(la.AName).Ord))
           .Bind(out _alleles)
           .Subscribe();

        _cleanUp = new CompositeDisposable(LocusesAlleles.Connect(), locusesLoader, allelesLoader);

        #endregion


        AddPopulationCommand = ReactiveCommand.CreateFromTask(addPopulation);
        EditPopulationCommand = ReactiveCommand.CreateFromTask<Population>(editPopulation);
        DelPopulationCommand = ReactiveCommand.CreateFromTask(delPopulation);
        MovePopulationCommand = ReactiveCommand.Create<string>(movePopulation);

        AddLocusCommand = ReactiveCommand.CreateFromTask(addLocus);
        EditLocusCommand = ReactiveCommand.CreateFromTask<LocusWR>(editLocus);
        DelLocusCommand = ReactiveCommand.CreateFromTask(delLocus);
        MoveLocusCommand = ReactiveCommand.Create<string>(moveLocus);

        AddAlleleCommand = ReactiveCommand.CreateFromTask(addAllele);
        EditAlleleCommand = ReactiveCommand.CreateFromTask<LocusAllele>(editAllele);
        DelAlleleCommand = ReactiveCommand.CreateFromTask(delAllele);

        SynonymsCommand = ReactiveCommand.CreateFromTask<LocusWR>(openSymonyms);

    }

    #region Porperties

    IConnectableObservable<IChangeSet<LocusAllele>> LocusesAlleles { get; set; } = null!;

    public ISettingsProvider<ExSettingData>? Settings { get; set; }

    public ReadOnlyObservableCollection<LocusAllele>? Alleles => _alleles;
    private ReadOnlyObservableCollection<LocusAllele>? _alleles;
    public ReadOnlyObservableCollection<LocusWR>? Locuses => _locuses;
    private ReadOnlyObservableCollection<LocusWR>? _locuses;

    [Reactive] public LocusWR? SelectedLocus { get; set; }
    [Reactive] public LocusAllele? SelectedAllele { get; set; }
    [Reactive] public Population? SelectedPopulation { get; set; }

    #endregion

    #region Commands

    // Популяции
    public RxCommandUnit? AddPopulationCommand { get; }
    public RxCommandPopulation? EditPopulationCommand { get; }
    public RxCommandUnit? DelPopulationCommand { get; }
    public RxCommandText? MovePopulationCommand { get; }

    // Локусы
    public RxCommandUnit? AddLocusCommand { get; }
    public RxCommandLocusWR? EditLocusCommand { get; }
    public RxCommandUnit? DelLocusCommand { get; }
    public RxCommandText? MoveLocusCommand { get; }

    // Аллели
    public RxCommandUnit? AddAlleleCommand { get; }
    public RxCommandLocusAllele? EditAlleleCommand { get; }
    public RxCommandUnit? DelAlleleCommand { get; }

    // Синонимы
    public RxCommandLocusWR? SynonymsCommand { get; }

    #endregion

    #region Helper

    // Популяции
    private async Task addPopulation()
    {
        var res = await _dialogService.AddEditPopulation(new AddEditPopulation(), Lang.Resources.cap_new_population);
        if (res != null && _dialogService != null && _appDb != null)
        {
            try
            {
                Population newPop = new Population { Name = res.Name, BaseOn = res.BaseOn };
                newPop.Id = _appDb.InsertPopulation(newPop, res.BaseOn);
                newPop.BaseOn = res.BaseOn;
                newPop.Ord = newPop.Id; // В базе данных, при создании новой популяции ord получает номер id
                Settings.Value.SourcePopulation.Add(newPop);
                _la.Clear();
                _la.AddRange(_appDb.GetAllPopLocuses());

                SelectedPopulation = newPop;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    Log.SendWarrning(Lang.Resources.err_add_population, Lang.Resources.err_pop_unique);
                else
                    Log.SendError(Lang.Resources.err_add_population, ex);
            }

        }
    }
    private async Task editPopulation(Population population)
    {
        if (SelectedPopulation != null && _dialogService != null && _appDb != null)
        {
            if (SelectedPopulation == Settings.Value.CurrentPopulation)
            {
                Log.SendWarrning(Lang.Resources.msg_cant_delete_population, Lang.Resources.cap_edit_population);
                return;
            }

            var res = await _dialogService.AddEditPopulation(new AddEditPopulation(SelectedPopulation), Lang.Resources.cap_edit_population);
            if (res != null && res.Name != SelectedPopulation.Name)
            {
                try
                {
                    res.Id = SelectedPopulation.Id;
                    res.Ord = SelectedPopulation.Ord;
                    _appDb.UpdatePopulation(res);
                    Settings.Value.SourcePopulation.Replace(SelectedPopulation, res);
                    SelectedPopulation = res;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("UNIQUE"))
                        Log.SendWarrning(Lang.Resources.err_edit_population, Lang.Resources.err_pop_unique);
                    else
                        Log.SendError(Lang.Resources.err_edit_population, ex);
                }

            }
        }
    }
    private async Task delPopulation()
    {
        if (SelectedPopulation == Settings.Value.CurrentPopulation)
        {
            Log.SendWarrning(Lang.Resources.msg_cant_delete_population, Lang.Resources.cap_edit_population);
            return;
        }

        if (_dialogService != null && _appDb !=  null)
        {
            var res = await _dialogService.ConfirmDelete(Settings.Value.ConfirmDeletePopulation, string.Format(Lang.Resources.conf_delete_population, SelectedPopulation?.Name),
            (b) => { Settings.Value.ConfirmDeletePopulation = !b; });

            if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
                return;

            if (Settings.Value.Populations?.Any() != true || SelectedPopulation == null)
                return;

            var currIdx = Settings.Value.Populations!.IndexOf(SelectedPopulation!);
            var cnt = Settings.Value.Populations!.Count;

            _appDb.DeletePopulations(new[] { SelectedPopulation! });
            Settings.Value.SourcePopulation.Remove(SelectedPopulation!);

            SelectedPopulation = currIdx switch
            {
                0 when cnt > 1 => Settings.Value.Populations.First(),
                var idx when idx == cnt - 1 => Settings.Value.Populations.Last(),
                var idx when idx < cnt - 1 => Settings.Value.Populations.ElementAt(idx),
                _ => SelectedPopulation
            };
        }

    }
    private void movePopulation(string s)
    {
        if (Settings.Value.Populations == null || SelectedPopulation == null)
            return;

        var index = Settings.Value.Populations!.IndexOf(SelectedPopulation!);
        int curr = 0;
        if (s == "Up")
        {
            if (index <= 0) return; // Уже на первом месте

            var previous = Settings.Value.Populations[index - 1];
            if (SelectedPopulation != null && previous != null)
            {
                _appDb.MovePopulation(SelectedPopulation, previous);

                curr = previous.Ord;
                var tempOrd = SelectedPopulation!.Ord;
                SelectedPopulation.Ord = previous.Ord;
                previous.Ord = tempOrd;
            }
        }
        else
        {
            if (index >= Settings.Value.Populations.Count - 1) return; // Уже на последнем месте

            var next = Settings.Value.Populations[index + 1];
            if (SelectedPopulation != null && next != null)
            {
                _appDb.MovePopulation(SelectedPopulation, next);

                curr = next.Ord;
                var tempOrd = SelectedPopulation!.Ord;
                SelectedPopulation.Ord = next.Ord;
                next.Ord = tempOrd;
            }
        }
        SelectedPopulation = Settings.Value.Populations!.FirstOrDefault(n => n.Ord == curr);
    }

    // Локусы
    private async Task addLocus()
    {
        if (SelectedPopulation != null && await _dialogService.AddEditLocus(new AddEditLocus(), Lang.Resources.cap_new_locus) is var inputed && inputed != null)
        {
            try
            {
                inputed.PopId = SelectedPopulation.Id;
                var inserted = _appDb.InsertLocus(inputed);

                LocusAllele la = new()
                {
                    LName = inputed.Name,
                    LOrd = inserted.Item2,
                    LCalc = inputed.IsCalc,
                    MutFreq = inputed.MutFreq,
                    AMinFreq = inputed.MinFreq,
                    PopId = inputed.PopId,
                    LocusId = inserted.Item1
                };

                _la.Add(la);
                if (Locuses != null && Locuses.Any())
                    SelectedLocus = Locuses.FirstOrDefault(l => l.Id == inserted.Item1);
            }
            catch (Exception ex)
            {
                Log.SendError(Lang.Resources.err_add_locus, ex);
            }

        }
    }
    private async Task editLocus(LocusWR locusWR)
    {
        if (SelectedLocus != null && Locuses != null)
        {
            var res = await _dialogService.AddEditLocus(new AddEditLocus(SelectedLocus), Lang.Resources.cap_edit_locus);
            if (res != null)
            {
                try
                {
                    res.Id = SelectedLocus.Id;
                    res.Ord = SelectedLocus.Ord;
                    res.PopId = SelectedLocus.PopId;
                    _appDb.UpdateLocus(res);

                    _la.Edit(innerList =>
                    {
                        var itemsToUpdate = innerList
                            .Where(n => n.PopId == SelectedLocus.PopId && n.LocusId == SelectedLocus.Id)
                            .ToList();

                        foreach (var la in itemsToUpdate)
                        {
                            var updatedItem = la.With(
                                lName: res.Name,
                                lCalc: res.IsCalc,
                                mutFreq: res.MutFreq,
                                aMinFreq: res.MinFreq
                            );

                            innerList.Replace(la, updatedItem);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.SendError(Lang.Resources.err_add_locus, ex);
                }
            }
        }
    }
    private async Task delLocus()
    {
        if (SelectedLocus != null && Locuses != null && Locuses.Any())
        {
            var currIdx = Locuses.IndexOf(SelectedLocus);
            var cnt = Locuses.Count;

            var res = await _dialogService.ConfirmDelete(Settings.Value.ConfirmDeleteLocus, string.Format(Lang.Resources.conf_delete_locus, SelectedLocus.Name),
                (b) => { Settings.Value.ConfirmDeleteLocus = !b; });

            if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
                return;

            _appDb.DeleteLocus(new[] { SelectedLocus! });
            var dl = _la.Items.FirstOrDefault(i => i.LName == SelectedLocus.Name && i.PopId == SelectedLocus.PopId);
            if (dl != null)
                _la.Remove(dl);

            SelectedLocus = currIdx switch
            {
                0 when cnt > 1 => Locuses.FirstOrDefault(),
                var idx when idx == cnt - 1 => Locuses.LastOrDefault(),
                var idx when idx < cnt - 1 => Locuses.ElementAt(idx),
                _ => SelectedLocus
            };
        }
    }
    private void moveLocus(string s)
    {
        if (SelectedLocus != null && Locuses != null && Locuses.Any())
        {

            var index = Locuses.IndexOf(SelectedLocus);
            int curr = 0;
            if (s == "Up")
            {
                if (index <= 0) return; // Уже на первом месте

                var previous = Locuses[index - 1];
                if (previous != null)
                {
                    _appDb.MoveLocus(SelectedLocus, previous);

                    curr = previous.Ord;
                    var tempOrd = SelectedLocus!.Ord;
                    SelectedLocus.Ord = previous.Ord;
                    previous.Ord = tempOrd;
                }
            }
            else
            {
                if (index >= Locuses.Count - 1) return; // Уже на последнем месте

                var next = Locuses[index + 1];
                if (SelectedLocus != null && next != null)
                {
                    _appDb.MoveLocus(SelectedLocus, next);

                    curr = next.Ord;
                    var tempOrd = SelectedLocus!.Ord;
                    SelectedLocus.Ord = next.Ord;
                    next.Ord = tempOrd;
                }
            }
            SelectedLocus = Locuses.FirstOrDefault(n => n.Ord == curr);
        }
    }

    // Аллели
    private async Task addAllele()
    {
        if (SelectedLocus != null && SelectedPopulation != null &&
            await _dialogService.AddEditAllele(new AddEditAllele(), Lang.Resources.cap_new_allele) is var res && res != null)
        {
            try
            {
                res.LocusId = SelectedLocus.Id;
                res.PopId = SelectedPopulation.Id;

                var alleleId = _appDb.InsertAllele(res);
                res.Id = alleleId;
                res.Ord = alleleId;

                LocusAllele la = new()
                {
                    LName = SelectedLocus.Name,
                    LOrd = SelectedLocus.Ord,
                    LCalc = SelectedLocus.IsCalc,
                    MutFreq = SelectedLocus.MutFreq,
                    AName = res.Name,
                    AFreq = res.Freq,
                    AMinFreq = SelectedLocus.MinFreq,
                    PopId = SelectedLocus.PopId,
                    LocusId = res.LocusId,
                    AlleleId = alleleId,
                };

                _la.Add(la);
                if (Alleles != null && Alleles.Any())
                    SelectedAllele = Alleles.FirstOrDefault(a => a.AlleleId == alleleId);
            }
            catch (Exception ex)
            {
                Log.SendError(Lang.Resources.err_add_locus, ex);
            }

        }
    }
    private async Task editAllele(LocusAllele alleleWR)
    {
        if (SelectedAllele != null && Alleles != null && SelectedPopulation != null)
        {
            var res = await _dialogService.AddEditAllele(new AddEditAllele(new(SelectedAllele)), Lang.Resources.cap_edit_allele);

            if (res != null && _la.Items.IndexOf(SelectedAllele) is var index && index >= 0)
                try
                {
                    res.Id = SelectedAllele.AlleleId;
                    res.PopId = SelectedPopulation.Id;
                    _appDb.UpdateAllele(res);

                    var updatedItem = SelectedAllele.With(
                        aName: res.Name,
                        aFreq: res.Freq
                        );

                    _la.ReplaceAt(index, updatedItem);
                    SelectedAllele = updatedItem;
                }
                catch (Exception ex)
                {
                    Log.SendError(Lang.Resources.err_add_locus, ex);
                }
        }
    }
    private async Task delAllele()
    {
        if (SelectedAllele != null && Alleles != null && SelectedLocus != null && SelectedPopulation != null)
        {
            var currIdx = Alleles.IndexOf(SelectedAllele);
            var cnt = Alleles.Count;

            var res = await _dialogService.ConfirmDelete(Settings.Value.ConfirmDeleteAllele, string.Format(Lang.Resources.conf_delete_allele, SelectedAllele.AName),
                (b) => { Settings.Value.ConfirmDeleteAllele = !b; });

            if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
                return;

            _appDb.DeleteAllele(new[] { new AlleleWR(SelectedAllele) });
            var itemsLocus = _la.Items.Where(it => it.PopId == SelectedPopulation.Id && it.LocusId == SelectedLocus.Id);

            if (itemsLocus.Count() > 1)
                _la.Remove(SelectedAllele);
            else
                _la.Edit(innerList =>
                {
                    LocusAllele? updatedItem = SelectedAllele.With(
                        alleleId: 0,
                        aName: "",
                        aFreq: 1
                    );

                    innerList.Replace(SelectedAllele, updatedItem);
                });

            SelectedAllele = currIdx switch
            {
                0 when cnt > 1 => Alleles.FirstOrDefault(),
                var idx when idx == cnt - 1 => Alleles.LastOrDefault(),
                var idx when idx < cnt - 1 => Alleles.ElementAt(idx),
                _ => SelectedAllele
            };
        }
    }

    private async Task openSymonyms(LocusWR locusWR) => await _dialogService.DialogSynonyms(new SynonymView(), Lang.Resources.cap_synonyms);
   
    //TODO не работает сортировка аллелей после добавления/редактирования аллелей на форме редактирования локусов
    #endregion

    private void RefreshLocuses()
    {

        _la.Clear();
        _la.AddRange(_appDb.GetAllPopLocuses());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cleanUp?.Dispose();
        }
        base.Dispose(disposing);
    }
}
