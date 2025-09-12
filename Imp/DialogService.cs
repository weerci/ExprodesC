using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Calc.Models;
using ExprodesC.Services;
using ExprodesC.ViewModels;
using ExprodesC.Views.Dialogs;
using ExprodesC.Views.Synonym;
using ExprodesC.Wrappers;
using FluentAvalonia.UI.Controls;
using Func.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ExprodesC.Imp;

public class DialogService : IDialogService
{

    readonly ISettingsProvider<ExSettingData> _settingsProvider;
    public DialogService(ISettingsProvider<ExSettingData> settingsProvider)
    {
        _settingsProvider = settingsProvider;
    }
    public async Task<IStorageFile?> OpenFileAsync()
    {
        var files = await App.MainWindow!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        return files.Count >= 1 ? files[0] : null;
    }

    public async Task<IStorageFile?> SaveFileAsync()
    {
        return await App.MainWindow!.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
        {
            Title = "Save Text File"
        });
    }

    public async Task<object> ConfirmDelete(bool confirmSetting, string content, Action<bool> action)
    {
        if (!confirmSetting)
            return false;

        TaskDialog td = new()
        {
            Header = Lang.Resources.conf_delete,
            Content = content,
            ShowProgressBar = false,
            FooterVisibility = TaskDialogFooterVisibility.Always,
            IconSource = new SymbolIconSource { Symbol = Symbol.Pin },
            IconForeground = new SolidColorBrush(Color.FromRgb(224, 159, 0), 1),
            //HeaderForeground = new SolidColorBrush(Color.FromRgb(224, 159, 0), 1),
            Footer = new CheckBox { Content = Lang.Resources.msg_no_show },
            Buttons =
            {
                TaskDialogButton.YesButton,
                TaskDialogButton.NoButton
            },
            XamlRoot = App.MainWindow,
        };
        ((CheckBox)td.Footer).IsCheckedChanged += (object? sender, Avalonia.Interactivity.RoutedEventArgs e) =>
        {
            if (sender is CheckBox cb && cb.IsChecked is bool b)
                action?.Invoke(b);
        };

        return await td.ShowAsync(true);
    }

    /// <inheritdoc/>
    public async Task<Population?> AddEditPopulation(UserControl content, string title)
    {
        if (content is AddEditPopulation control)
        {
            var res = await new DialogWindow(title, content) { Width = 640, Height = 290 }
                .ShowDialog<DialogResult>(getOwner(control));

            if (res == DialogResult.Save)
            {
                var name = control.tbName.Text;
                string basePop = "";
                if (control.cbBase.IsVisible && control.cbBase.SelectedItem is Population bp)
                    basePop = bp.Name;
                else
                    basePop = control.tbBase.Text != null ? control.tbBase.Text : Calc.Lang.Resources.cap_pop_create_manuale;
                if (basePop != null && name != null)
                    return new Population() { Name = name, BaseOn = basePop };
            }
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<LocusWR?> AddEditLocus(UserControl content, string title)
    {
        if (content is AddEditLocus ctrl)
        {
            var v = await new DialogWindow(title, content) { Width = 640, Height = 400 }
                .ShowDialog<DialogResult>(getOwner(ctrl));

            if (v == DialogResult.Save)
                return new LocusWR(-1, ctrl.tbName.Text!, ctrl.nbMinFreq.Value, ctrl.nbMutFreq.Value, (bool)ctrl.chbIsCalculate.IsChecked!, 0, 0);
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<AlleleWR?> AddEditAllele(UserControl content, string title)
    {
        if (content is AddEditAllele ctrl)
        {
            var v = await new DialogWindow(title, content) { Width = 500, Height = 280 }
                .ShowDialog<DialogResult>(getOwner(ctrl));

            if (v == DialogResult.Save)
                return new AlleleWR(0, 0, ctrl.tbName.Text!, 0, ctrl.nbFreq.Value, 0);
        }
        return null;
    }

    /// <inheritdoc/>
    public async Task<SynonymWR?> AddEditSynonym(UserControl content, string title)
    {
        if (content is AddEditSynonym ctrl)
        {
            var v = await new DialogWindow(title, content) { Width = 500, Height = 250 }
                .ShowDialog<DialogResult>(getOwner(ctrl));

            if (v == DialogResult.Save && ctrl.cbLocuses.SelectedItem is LocusGroup lg && ctrl.tbName.Text != null)
                return new SynonymWR(0, ctrl.tbName.Text, lg.LocusId, lg.LocusName, lg.LocusOrd);
        }
        return null;
    }

    public async Task<GenotypeWR?> DialogGenotype(UserControl content, string title)
    {
        if (content is AddEditGenotype g)
        {
            var v = await new DialogWindow(title, content) { Width = 850, Height = 600 }
                .ShowDialog<DialogResult>(getOwner(g));

            if (v == DialogResult.Save)
            {
                var genomes = g.lbSelected.ItemsSource?.Cast<GenomeWR>() ?? [];
                if (g.ccGenotype.Content is GenotypeWR genotype)
                {
                    genotype.Genotype.Name = g.tbName.Text!;
                    genotype.Genotype.ReloadGenomes(genomes.Select(n => n.GetActualGenome));
                    genotype.Genotype.RaisePropertyChanged(nameof(Genotype.Genomes));
                    return genotype;
                }
            }
        }
        return null;
    }

    public async Task<SynonymWR?> DialogSynonyms(UserControl content, string title)
    {
        if (content is SynonymView g)
        {
            var v = await new DialogWindow(title, content) { Width = 850, Height = 600, IsVisibleButton = false }
                .ShowDialog<DialogResult>(App.MainWindow!);
        }
        return null;
    }

    Window getOwner(UserControl? uc = null)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop )
            return desktop.Windows.FirstOrDefault(w => w.IsActive, App.MainWindow!);

        return App.MainWindow!;
    }

    //TODO Сделать сообщения закрывающмимся по истечении некоторого времени
    //TODO AddEditSynonyms при выборе текущего локуса не открывается форма с позиционированием на нем
}
