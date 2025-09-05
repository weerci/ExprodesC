using Calc.Exceptions;
using Calc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class GenotypeWR() : ReactiveObject
{
    public GenotypeWR(Genotype genotype, bool isControl = false) : this()
    { 
        Genotype = genotype;
        IsControl = isControl;
    }


    public Genotype Genotype { get; } = null!;
    [Reactive] public bool IsSelected { get; set; }

    [Reactive] public bool IsControl { get; set; } 


    public static GenotypeWR Empty(bool isControl) => new GenotypeWR(Genotype.Empty, isControl);

}
