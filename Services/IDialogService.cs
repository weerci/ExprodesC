using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Calc.Models;
using ExprodesC.Wrappers;
using System.Threading.Tasks;

namespace ExprodesC.Services;

public interface IDialogService
{
    public Task<IStorageFile?> OpenFileAsync();
    public Task<IStorageFile?> SaveFileAsync();
    public Task<object> ConfirmDelete(bool confconfirmSetting, string content, Action<bool> action);
    
    /// <summary>
    /// Форма создания и редактирования популяции
    /// </summary>
    public Task<Population?> AddEditPopulation(UserControl content, string title);
    
    /// <summary>
    /// Форма создания и редактирования локуса
    /// </summary>
    public Task<LocusWR?> AddEditLocus(UserControl content, string title);

    /// <summary>
    /// Форма создания и редактирования аллеля
    /// </summary>
    public Task<AlleleWR?> AddEditAllele(UserControl content, string title);

    /// <summary>
    /// Форма создания генотипа
    /// </summary>
    public Task<GenotypeWR?> DialogGenotype(UserControl content, string title);

    /// <summary>
    /// Форма синонимов
    /// </summary>
    public Task<SynonymWR?> DialogSynonyms(UserControl content, string title);

    /// <summary>
    /// Форма создания и редактирования синонима
    /// </summary>
    public Task<SynonymWR?> AddEditSynonym(UserControl content, string title);
}

