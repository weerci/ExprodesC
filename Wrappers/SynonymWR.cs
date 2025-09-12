using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class SynonymWR : ReactiveObject
{
    public SynonymWR(int id, string name, int locusId, string locusName, int ord)
    {
        Id = id;
        LocusId = locusId;
        Name = name;
        LocusName = locusName;
        Ord = ord;
    }

    [Reactive] public int Id { get; set; }
    [Reactive] public string Name { get; set; } = "";
    [Reactive] public int LocusId { get; set; }
    [Reactive] public string LocusName { get; set; } = "";
    [Reactive] public int Ord { get; set; }

}
