using Calc.Models;
using ExprodesC.Models;
using ExprodesC.Views.Controls;
using ExprodesC.Views.Wrappers;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ExprodesC.Views.Controls.GenotypeColumnVM;

namespace ExprodesC.ViewModels
{
    public class ComparePageVM() : BaseVM
    {
        private GenotypeColumnVM? _draggedColumn;

        public ComparePageVM(MainPageVM mianPageVM) : this()
        {
            MainPageVM = mianPageVM;

            // Обновляем глобальные строки при изменении столбцов
            this.WhenAnyValue(x => x.Columns.Count)
                .Subscribe(_ => UpdateGlobalLocusRows());

            AddColumnCommand = ReactiveCommand.Create<GenotypeWR>(AddColumn);
            RemoveColumnCommand = ReactiveCommand.Create<GenotypeColumnVM>(RemoveColumn);
        }

        #region Properties

        public MainPageVM? MainPageVM { get; }

        public ObservableCollection<GenotypeColumnVM> Columns { get; } = new();

        public ObservableCollection<LocusRow> GlobalLocusRows { get; } = new();

        #endregion

        #region Commands

        public ReactiveCommand<GenotypeWR, Unit>? AddColumnCommand { get; }
        public ReactiveCommand<GenotypeColumnVM, Unit>? RemoveColumnCommand { get; }

        #endregion

        #region Helper

        private void AddColumn(GenotypeWR genotype)
        {
            if (Columns.Any(c => c.GenotypeWR == genotype)) return;

           // var column = new GenotypeColumnVM(genotype, this);
           // Columns.Add(column);
        }

        private void RemoveColumn(GenotypeColumnVM column)
        {
            Columns.Remove(column);
        }

        public void StartDrag(GenotypeColumnVM column)
        {
            _draggedColumn = column;
        }

        public void HandleDrop(GenotypeColumnVM targetColumn)
        {
            if (_draggedColumn == null || _draggedColumn == targetColumn) return;

            int oldIndex = Columns.IndexOf(_draggedColumn);
            int newIndex = Columns.IndexOf(targetColumn);

            Columns.Move(oldIndex, newIndex);
            _draggedColumn = null;
        }

        private void UpdateGlobalLocusRows()
        {
            // Собираем все уникальные локусы из всех столбцов
            var allLoci = Columns
                .SelectMany(c => c.GenotypeWR.Genotype.Genomes)
                .Select(g => g.Locus)
                .Distinct()
                .ToList();

            // Создаем строки для каждого локуса
            var newRows = allLoci.Select(locus => new LocusRow(locus.Name)).ToList();

            // Синхронизация строк
            foreach (var row in newRows)
            {
                var existing = GlobalLocusRows.FirstOrDefault(r => r.LocusName == row.LocusName);
                if (existing == null)
                {
                    GlobalLocusRows.Add(row);
                }
            }

            // Удаляем старые строки
            for (int i = GlobalLocusRows.Count - 1; i >= 0; i--)
            {
                if (!newRows.Any(r => r.LocusName == GlobalLocusRows[i].LocusName))
                {
                    GlobalLocusRows.RemoveAt(i);
                }
            }

            // Обновляем все столбцы
            foreach (var column in Columns)
            {
                column.UpdateGenomeRows(GlobalLocusRows);
            }
        }

        #endregion

    }
}
