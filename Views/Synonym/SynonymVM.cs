using Calc.Data;
using Calc.Deprecated;
using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Imp;
using ExprodesC.Services;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using FluentAvalonia.UI.Controls;
using Func.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Synonym;

public class SynonymVM() : BaseVM
{
    readonly SourceList<SynonymWR> _sn = new();
    IAppDb _appDb = null!;
    IDialogService _dialogService = null!;
    IDisposable _cleanUp = null!;
    ISettingsProvider<ExSettingData>? _settings = null!;


    public SynonymVM(ISettingsProvider<ExSettingData> settings, IAppDb appDb, IDialogService dialogService) : this()
    {
        _appDb = appDb;
        _dialogService = dialogService;
        _settings = settings;

        LoadData();

        AddSynonymCommand = ReactiveCommand.CreateFromTask(addSynonym);
        EditSynonymCommand = ReactiveCommand.CreateFromTask<SynonymWR>(editSynonym);
        DelSynonymCommand = ReactiveCommand.CreateFromTask<SynonymWR>(delSynonym);

    }

    #region Porperties

    public ReadOnlyObservableCollection<SynonymWR>? Synonyms => _synonyms;
    ReadOnlyObservableCollection<SynonymWR>? _synonyms;

    public ReadOnlyObservableCollection<LocusGroup>? LocusGroups => _locusGroup;
    ReadOnlyObservableCollection<LocusGroup>? _locusGroup;

    [Reactive] public SynonymWR? SelectedSynonym { get; set; }

    #endregion

    #region Commands

    public RxCommandUnit? AddSynonymCommand { get; }
    public RxCommandSynonymWR? EditSynonymCommand { get; }
    public RxCommandSynonymWR? DelSynonymCommand { get; }


    #endregion

    #region Helper

    private async Task addSynonym()
    {
        var res = await _dialogService.AddEditSynonym(new AddEditSynonym(), Lang.Resources.cap_new_synonym);
        if (res != null)
        {
            try
            {
                int newId = _appDb.InsertSynonym(res);
                res.Id = newId;

                // Просто перезагружаем все данные
                LoadData();

                SelectedSynonym = res;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    Log.SendWarrning(Lang.Resources.err_add_synonym, Lang.Resources.err_synonym_unique);
                else
                    Log.SendError(Lang.Resources.err_add_synonym, ex);
            }

        }
    }

    private async Task editSynonym(SynonymWR sWR)
    {
        var res = await _dialogService.AddEditSynonym(new AddEditSynonym(sWR), Lang.Resources.cap_new_synonym);
        if (res != null)
        {
            try
            {
                res.Id = sWR.Id;
                _appDb.UpdateSynonym(res);

                // Просто перезагружаем все данные
                LoadData();

                SelectedSynonym = res;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("UNIQUE"))
                    Log.SendWarrning(Lang.Resources.err_add_synonym, Lang.Resources.err_synonym_unique);
                else
                    Log.SendError(Lang.Resources.err_add_synonym, ex);
            }

        }
    }

    private async Task delSynonym(SynonymWR sWR)
    {
        if (Synonyms != null && _settings != null)
        {
            var currIdx = Synonyms.IndexOf(sWR);
            var cnt = Synonyms.Count;

            var res = await _dialogService.ConfirmDelete(_settings.Value.ConfirmDeleteSynonym, string.Format(Lang.Resources.conf_delete_synonym, sWR.Name),
                (b) => { _settings.Value.ConfirmDeleteSynonym = !b; });

            if (res is TaskDialogStandardResult r && r != TaskDialogStandardResult.Yes)
                return;

            _appDb.DeleteSynonym(new[] { sWR });
            LoadData();

            SelectedSynonym = currIdx switch
            {
                0 when cnt > 1 => Synonyms.FirstOrDefault(),
                var idx when idx == cnt - 1 => Synonyms.LastOrDefault(),
                var idx when idx < cnt - 1 => Synonyms.ElementAt(idx),
                _ => SelectedSynonym
            };
        }

    }

    private void LoadData()
    {
        var allSynonyms = _appDb.GetAllSynonyms();

        // Очищаем и загружаем синонимы
        _sn.Clear();
        _sn.AddRange(allSynonyms);

        // Создаем группы вручную
        var groups = allSynonyms
            .GroupBy(s => (s.LocusId, s.LocusName, s.Ord))
            .Select(g => new LocusGroup(
                g.Key.LocusId,
                g.Key.LocusName,
                g.Key.Ord,
                g.Select(s => new SynonymItem(s.Name))))
            .OrderBy(g => g.LocusOrd)
            .ToList();

        _locusGroup = new ReadOnlyObservableCollection<LocusGroup>(
            new ObservableCollection<LocusGroup>(groups));

        _synonyms = new ReadOnlyObservableCollection<SynonymWR>(
            new ObservableCollection<SynonymWR>(allSynonyms.OrderBy(s => s.Ord)));

        this.RaisePropertyChanged(nameof(LocusGroups));
        this.RaisePropertyChanged(nameof(Synonyms));
    }


    #endregion
}
