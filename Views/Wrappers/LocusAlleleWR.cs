using Calc.Data;
using Calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Wrappers
{
    public class LocusAlleleWR(LocusAllele locusAllele) : ReactiveObject
    {
        public LocusAllele LocusAllele { get; } = locusAllele;
        public double AOrd { get; } = Allele.GetOrd(locusAllele.AName);
        [Reactive] public bool IsChecked { get; set; }

        public static IEnumerable<LocusAlleleWR> FromLocusAlleles(IEnumerable<LocusAllele> las)
        {
            foreach (var item in las)
                yield return new LocusAlleleWR(item);
        }
        public Allele AsAllele() => new Allele(LocusAllele.AName);
    }
}
