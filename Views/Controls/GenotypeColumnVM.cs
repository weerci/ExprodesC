using ExprodesC.ViewModels;
using ExprodesC.Views.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Controls
{
    public class GenotypeColumnVM() : BaseVM
    {
        
        public GenotypeColumnVM(GenotypeWR genotype, MainPageVM mainPageVM) : this()
        {
            GenotypeWR = genotype;
            MainPageVM = mainPageVM;

            UpdateGenomeRows(MainPageVM.GlobalLocusRows);
        }

        public MainPageVM MainPageVM { get; set; } = null!;

        //TODO При удалении из дерева не удаляется из списка выбранных
        [Reactive] public GenotypeWR GenotypeWR { get; set; } = null!;
        
        public ObservableCollection<GenomeRow> GenomeRows { get; } = new();

        public ReactiveCommand<Unit, IDisposable> CloseCommand { get; } = null!;

        public void UpdateGenomeRows(IEnumerable<LocusRow> globalRows)
        {
            var genomesDict = GenotypeWR.Genotype.Genomes
                .ToDictionary(g => g.Locus.Name);

            GenomeRows.Clear();

            foreach (var locusRow in globalRows)
            {
                if (genomesDict.TryGetValue(locusRow.LocusName, out var genome))
                {
                    GenomeRows.Add(new GenomeRow(
                        genome.Locus.Name,
                        string.Join(", ", genome.Alleles.Select(a => a.Name))
                    ));
                }
                else
                {
                    GenomeRows.Add(new GenomeRow(locusRow.LocusName, ""));
                }
            }
        }


    }
}
