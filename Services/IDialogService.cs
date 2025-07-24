using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Calc.Models;
using ExprodesC.Views.Wrappers;
using System.Threading.Tasks;

namespace ExprodesC.Services;

public interface IDialogService
{
    public Task<IStorageFile?> OpenFileAsync();
    public Task<IStorageFile?> SaveFileAsync();
    public Task<object> ConfirmDeleteGenotype(string content);

    public Task<GenotypeWR?> DialogGenotype(UserControl inputType, string title);
}

