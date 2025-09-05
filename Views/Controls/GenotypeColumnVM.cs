using Calc.Models;
using DynamicData;
using DynamicData.Binding;
using ExprodesC.Models;
using ExprodesC.ViewModels;
using ExprodesC.Wrappers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Controls
{
    public class GenotypeColumnVM() : BaseVM
    {

        public GenotypeColumnVM(GenotypeWR genotype, IProject project) : this()
        {
            GenotypeWR = genotype;
            Project = project;

            CloseCommand = ReactiveCommand.Create<GenotypeWR>((g) => { Project.UnSelectGenotype(g); });
            EditCommand = ReactiveCommand.Create<GenotypeWR>((g) => { Project.EditGenotype(g); });

        }

        public IProject Project { get; set; } = null!;

        //TODO При удалении из дерева не удаляется из списка выбранных
        [Reactive] public GenotypeWR GenotypeWR { get; set; } = null!;

        public ObservableCollection<GenomeRow> GenomeRows { get; } = new();

        public RxCommandGenotype CloseCommand { get; } = null!;
        public RxCommandGenotype EditCommand { get; } = null!;

        public void UpdateGenomeRows(List<Locus> locuses)
        {
            var genomesDict = GenotypeWR.Genotype.Genomes
                .ToDictionary(g => g.Locus.Name);

            GenomeRows.Clear();

            foreach (var locus in locuses)
            {
                if (genomesDict.TryGetValue(locus.Name, out var genome))
                {
                    GenomeRows.Add(new GenomeRow(
                        genome.Locus.Name,
                        string.Join(", ", genome.Alleles.Select(a => a.Name))
                    ));
                }
                else
                {
                    GenomeRows.Add(new GenomeRow(locus.Name, ""));
                }
            }
        }
    }


}

