using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Services;
using ExprodesC.Views.Controls;
using ExprodesC.Wrappers;
using Func;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;

namespace ExprodesC.Models
{
    public interface IProject
    {
        /// <summary>
        /// Количество выбранных генотипов (нужно для срабатывания CalcConverter.EnabledCalc, чтобы делать доступными те методы расчета у которых количество 
        /// генотипов для расчета равно или меньше выбранного количества)
        /// </summary>
        public int SelectedCount { get; set; }

        /// <summary>
        /// True - если проект содержит генотипы
        /// </summary>
        public bool HasGenotype { get; set; }

        /// <summary>
        /// Список всех генотипов
        /// </summary>
        public IConnectableObservable<IChangeSet<GenotypeWR>> Genotypes { get; }

        /// <summary>
        /// Список генотипов выбранных для работы
        /// </summary>
        ReadOnlyObservableCollection<GenotypeColumnVM> SelectedGolumns { get; }

        /// <summary>
        /// Текущий генотип выбранный в наборе <see cref="Genotypes"/>
        /// </summary>
        public GenotypeWR? CurrentGenotype { get; set; }

        /// <summary>
        /// В проекте есть выбранные аллели для работы
        /// </summary>
        public bool HasSelected { get; set; }

        /// <summary>
        /// Путь к файлу, в котором сохранен проект, если null - проект новый
        /// </summary>
        public FileName? PathToSavedFile { get; set; }

        /// <summary>
        /// Проект изменен и нуждается в сохранении
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// В проект добавляются новые генотипы
        /// </summary>
        public Ex<GenotypeWR> AddGenotypes(IEnumerable<GenotypeWR> gs);

        /// <summary>
        /// Из проекта удаляются существующие генотипы
        /// </summary>
        public Ex<bool> RemoveGenotypes(IEnumerable<GenotypeWR> genotypes);

        /// <summary>
        /// Данные о экспертах перезагружаются из базы данных
        /// </summary>
        /// <returns></returns>
        public Ex<bool> ReloadFromDb();

        /// <summary>
        /// В проект подгружаются данные из файлов
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public Ex<bool> LoadFromFile(Ex<FileName> fn);

        /// <summary>
        /// Изменения в проекте сохраняются. В файл записываются данные по профилям, в базу данных данные по экспертам
        /// </summary>
        public Ex<bool> Save(Ex<FileName> fn);

        /// <summary>
        /// Проект закрывается и на его месте создается новый
        /// </summary>
        /// <returns></returns>
        public Ex<bool> Close();

        /// <summary>
        /// Выбор генотипа в список генотипов для работы
        /// </summary>
        public void SelectGenotype(GenotypeWR gwr) => gwr.IsSelected = true;

        /// <summary>
        /// Удаление генотипа из списока генотипов для работы
        /// </summary>
        /// <param name="gwr"></param>
        public void UnSelectGenotype(GenotypeWR gwr) => gwr.IsSelected = false;

        /// <summary>
        /// Редактирование выбранного генома
        /// </summary>
        public void EditGenotype(GenotypeWR genotype);
    }
}
