using Calc.Data;
using Calc.Models;
using ExprodesC.Models;
using ExprodesC.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace ExprodesC.Services
{
    public interface IAppDb : ICalcDb
    {
        /// <summary>
        /// Получает все локусы для всех популяций
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocusAllele> GetAllPopLocuses();

        /// <summary>
        /// Сохранение популяции в базе данных
        /// </summary>
        /// <param name="population">Популяция для сохранения в базе</param>
        /// <param name="baseId">Популяция, на основе которой создается текущая</param>
        /// <returns>Идентификатор созданной популяции</returns>
        public int InsertPopulation(Population population, string baseName = "");

        /// <summary>
        /// Удаление набора популяций из списка популяций
        /// </summary>
        /// <param name="populations">Набор популяций для удаления</param>
        /// <returns>Кличество удаленных записей</returns>
        public int DeletePopulations(IEnumerable<Population> populations);

        /// <summary>
        /// Меняет значение свойства Ord, которое использутеся для задания сортировки
        /// </summary>
        public void MovePopulation(Population from, Population to);

        /// <summary>
        /// Изменяет название популяции
        /// </summary>
        public void UpdatePopulation(Population population);

        /// <summary>
        /// Сохранение локуса в базе данных
        /// </summary>
        public (int, int) InsertLocus(LocusWR lwr);

        /// <summary>
        /// Обновление локуса в базе данных
        /// </summary>
        public void UpdateLocus(LocusWR lwr);

        /// <summary>
        /// Удаление локуса из базы данных
        /// </summary>
        public int DeleteLocus(IEnumerable<LocusWR> lwrs);

        /// <summary>
        /// Меняет значение свойства Ord, которое использутеся для задания сортировки
        /// </summary>
        public void MoveLocus(LocusWR from, LocusWR to);

        /// <summary>
        /// Сохранение аллеля в базе данных
        /// </summary>
        public int InsertAllele(AlleleWR awr);

        /// <summary>
        /// Обновление аллеля в базе данных
        /// </summary>
        public void UpdateAllele(AlleleWR awr);

        /// <summary>
        /// Удаление аллеля из базы данных
        /// </summary>
        public int DeleteAllele(IEnumerable<AlleleWR> awrs);

    }
}
