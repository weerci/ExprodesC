using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Views.Controls
{
    public record LocusRow(string LocusName);
    public record GenomeRow(string LocusName, string Alleles);
}
