using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Calc.Models;
using ExprodesC.Services;
using ExprodesC.Views.Dialogs;
using ExprodesC.Views.Wrappers;
using FluentAvalonia.UI.Controls;
using Func.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ExprodesC.Imp
{
    public class DialogService : IDialogService
    {
        private readonly Window? _owner;

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

        public async Task<object> ConfirmDeleteGenotype(string content)
        {
            if (!_settingsProvider.Value.ConfirmDeleteGenotype)
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
                    _settingsProvider.Value.ConfirmDeleteGenotype = !b;
            };

            return await td.ShowAsync(true);

        }

        public async Task<GenotypeWR?> DialogGenotype(UserControl userControl, string title)
        {
            if (userControl is AddEditGenotype g)
            {
                var v = await new DialogWindow(title, userControl) { Width = 850, Height = 600 }
                    .ShowDialog<DialogResult>(App.MainWindow!);

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

    }
}
