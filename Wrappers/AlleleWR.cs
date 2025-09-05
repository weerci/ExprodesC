using Calc.Data;
using Calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class AlleleWR : ReactiveObject
{
    public AlleleWR(LocusAllele locusAllele)
    {
        Id = locusAllele.AlleleId;
        LocusId = locusAllele.LocusId;
        Name = locusAllele.AName;
        Ord = Allele.Ord;
        Freq = locusAllele.AFreq;
        PopId = locusAllele.PopId;
    }

    public AlleleWR(int id, int locusId, string name, int ord, double freq, int popId)
    {
        Id = id;
        LocusId = locusId;
        Name = name;
        Ord = ord;
        Freq = freq;
        PopId = popId;
    }

    [Reactive] public int Id { get; set; }
    [Reactive] public int LocusId { get; set; }
    [Reactive] public string Name { get; set; }
    public Allele Allele => new(Name);
    [Reactive] public double Ord { get; set; }
    [Reactive] public double Freq { get; set; }
    [Reactive] public bool Unknown { get; set; } = false;
    [Reactive] public int PopId { get; set; }

}
