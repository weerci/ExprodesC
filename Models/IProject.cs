using Calc.Models;
using DynamicData;
using ExprodesC.Views.Wrappers;
using Func;
using System.Reactive.Subjects;

namespace ExprodesC.Models
{
    public interface IProject
    {
        /// <summary>
        /// Список всех генотипов
        /// </summary>
        public IConnectableObservable<IChangeSet<GenotypeWR>> Genotypes { get; }

        /// <summary>
        /// Проект сохранен
        /// </summary>
        public bool IsSaved => !IsChanged;

        /// <summary>
        /// Проект изменен
        /// </summary>
        public bool IsChanged { get; }

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
        /// Изменения в проекте сохраняются
        /// </summary>
        public Ex<bool> Save(Ex<FileName> fn);


    }
}
