using Calc.Models;
using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Wrappers
{
     public class GenomeWR : ReactiveObject
    {
        readonly IDisposable _cleanUp;

        public GenomeWR(Genome genome, IObservableList<LocusAlleleWR> lvms)
        {
            Genome = genome;

            var listLaoder = lvms.Connect()
                .Bind(out _lvms)
                .Subscribe();

            var checkedGenome = this.WhenAnyValue(g => g.IsChecked)
                .Do(n => Debug.WriteLine(n))
                .Subscribe(n =>
                {
                    if (n == false)
                        while (_lvms.Count > 0)
                            _lvms[0].IsChecked = n;
                });

            _cleanUp = new CompositeDisposable(listLaoder, checkedGenome);

        }

        /// <summary>
        /// В процессе работы с формой набор аллелей в каждом геноме изменяется. Метод позволяет получить genom с актуальным набором аллелй
        /// </summary>
        /// <returns></returns>
        public Genome GetActualGenome => this.Genome.EditGenome(Genome.Locus.Name, Lvms?.ToList().Select(n => new Allele(n.LocusAllele.AName)) ?? []);

        public Genome Genome { get; }

        public ReadOnlyObservableCollection<LocusAlleleWR>? Lvms => _lvms;
        public ReadOnlyObservableCollection<LocusAlleleWR>? _lvms;

        [Reactive] public bool IsChecked { get; set; } = true;
    }
}
