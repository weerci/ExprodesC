using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Synonym;

public record LocusGroup(int LocusId, string LocusName, int LocusOrd, IEnumerable<SynonymItem> Synonyms);


public record SynonymItem(string Synonym);
