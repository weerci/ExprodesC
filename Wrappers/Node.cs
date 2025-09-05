using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class GenotypeNode
{
    /// <summary>
    /// Верхний уровень дерева генотипов содежит название "Генотипы"
    /// </summary>
    public static string GuidGenotypes => "{CF211558-2576-4B6A-B612-E990C949A041}";
    /// <summary>
    /// Поддерево "Профили"
    /// </summary>
    public static string GuidProfile => "{CF211558-2576-4B6A-B612-E990C949A042}";
    /// <summary>
    /// Поддерево "Эксперты/контроли"
    /// </summary>
    public static string GuidControl => "{CF211558-2576-4B6A-B612-E990C949A043}";

    public ReadOnlyObservableCollection<GenotypeNode>? SubNodes => _SubNodes;
    public ReadOnlyObservableCollection<GenotypeNode>? _SubNodes;
    public GenotypeWR GenotypeWR { get; }

    /// <summary>
    /// True - если выбранный node является предопределенной папкой (генотипы, проект, контроль/эксперт)
    /// </summary>
    public bool IsFolder =>
        GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A041}" ||
        GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A042}" ||
        GenotypeWR.Genotype.Id == "{CF211558-2576-4B6A-B612-E990C949A043}";

    public GenotypeNode(GenotypeWR genotypeWr)
    {
        GenotypeWR = genotypeWr;
    }

    public GenotypeNode(GenotypeWR genotypeWR, ReadOnlyObservableCollection<GenotypeNode> subNodes)
    {
        GenotypeWR = genotypeWR;
        _SubNodes = subNodes;
    }
    
    public static ObservableCollection<GenotypeNode> Roots => [new(
            new(new(Lang.Resources.cap_genotypes, []) { Id = GuidGenotypes }),
            new([Profiles, Controls]))];

    public static GenotypeNode? Profiles { get; } = new(new(new(Lang.Resources.cap_profiles, []) { Id = GuidProfile }));
    public static GenotypeNode? Controls { get; } = new(new(new(Lang.Resources.cap_contols, []) { Id = GuidControl }));

}
