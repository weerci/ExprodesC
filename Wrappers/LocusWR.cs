using Calc.Data;
using Calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class LocusWR : ReactiveObject
{
    public LocusWR(int id, string name, double minFreq, double mutFreq, bool isCalc, int ord, int popId)
    {
        Id = id;
        Name = name;
        MinFreq = minFreq;
        MutFreq = mutFreq;
        IsCalc = isCalc;
        Ord = ord;
        PopId = popId;
    }

    [Reactive] public int Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public double MinFreq { get; set; }
    [Reactive] public double MutFreq { get; set; }
    [Reactive] public bool IsCalc { get; set; }
    [Reactive] public int Ord { get; set; }
    [Reactive] public int PopId { get; set; }

    public string FreqTxt => $"Min. частота = {MinFreq}, Частота мутаций = {MutFreq}";
    public string IsCalcTxt => IsCalc ? "Локус участвует в расчетах" : "Не участвует в расчетах";
}
