using Calc.Calculation;
using Calc.Models;
using Func;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public partial class ErWR
{
    public ErWR() : this(new ER()) // Конструктор для дизайнера
    {
        // Заполняем фиктивными данными
        List<Allele> alleles = new List<Allele>
            {
                new Allele("10"),
                new Allele("12"),
                new Allele("14"),
                new Allele("16")
            };
        Hash hash = new();
        hash.Append(alleles);
        Er = new ER
        {
            Locus = new Locus { Name = "Пример локуса" },
            DirFormula = "2pa*2pb",
            Hash = hash,
            DirExpression = "2*0,1763*2*0,3459",
            RevFormula = "(pa+pb) * 2pa*pb",
            RevExpression = "(pa+pb) * 2pa*pb",
            DirResult = 0.5463,
            RevResult = 0.2593
        };

       

        // Добавляем фиктивные аллели
        Er.Genomes.Add(new Ex<Genome>(new Genome(Er.Locus, alleles[0], alleles[1], alleles)));
    }
}
