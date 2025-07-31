using Calc.Exceptions;
using Calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Wrappers;

public class GenotypeWR(Genotype genotype, bool isControl = false) : ReactiveObject
{
    public Genotype Genotype { get; } = genotype;
    [Reactive] public bool IsSelected { get; set; }

    [Reactive] public bool IsControl { get; set; } = isControl;

    public static GenotypeWR Empty(bool isControl) => new GenotypeWR(Genotype.Empty, isControl);

}
