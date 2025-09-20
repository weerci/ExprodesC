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
        Er = new ER
        {
            Locus = new Locus { Name = "Пример локуса" },
            DirFormula = "(pa+pb)^2-(pa)^2",
            DirExpression = "",
            RevFormula = "p^2",
            DirResult = 0.5,
            RevResult = 0.25
        };

        List<Allele> alleles = new List<Allele>
            {
                new Allele("10"),
                new Allele("12"),
                new Allele("14"),
                new Allele("16")
            };

        // Добавляем фиктивные аллели
        Er.Genomes.Add(new Ex<Genome>(new Genome(Er.Locus, alleles[0], alleles[1], alleles)));
    }
}
