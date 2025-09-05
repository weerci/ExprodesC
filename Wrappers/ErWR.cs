using Calc.Calculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExprodesC.Wrappers;

public class ErWR : ReactiveObject
{
    public ErWR(ER er)
    {
        Er = er;

        ToggleExpandCommand = ReactiveCommand.Create(() => IsExpanded = !IsExpanded);
    }

    #region Properties

    public ER Er { get; }

    public bool ShowReverse =>  !string.IsNullOrEmpty(Er.RevFormula);
    public bool ShowDirect =>  !string.IsNullOrEmpty(Er.DirFormula);

    [Reactive] public bool IsExpanded { get; set; }

    #endregion

    #region Command

    public ICommand ToggleExpandCommand { get; }

    #endregion
}
