using Avalonia.Controls;
using ExprodesC.Imp;
using ExprodesC.Services;
using Func.Services;
using Material.Icons;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace ExprodesC.ViewModels.Settings;

public class SettingsPageVM() : BaseVM
{

    public SettingsPageVM(ISettingsProvider<ExSettingData> settings) : this()
    {
        Settings = settings;
        createTree();
        CurrentView = _appearanceView;

        this
            .WhenAnyValue(x => x.SelectedNode)
            .Select(n => n)
            .Do(n =>
            {
                setSubTitle(n);
                CurrentView = n?.Id switch
                {
                    2 => _confrimView,
                    3 => _calculationView,
                    _ => _appearanceView
                };
            })
            .Subscribe();

    }


    #region Properties
    [Reactive] public ISettingsProvider<ExSettingData>? Settings { get; set; }
    [Reactive] public string? Title { get; private set; } = Lang.Resources.cap_setting;
    [Reactive] public string? SubTitle { get; private set; }
    [Reactive] public MaterialIconKind Icon { get; set; }
    [Reactive] public MaterialIconKind SubIcon { get; set; }

    [Reactive] public bool IsCompactMode { get; private set; }
    [Reactive] public SettingNode? SelectedNode { get; set; }
    [Reactive] public UserControl? CurrentView { get; set; }


    public ObservableCollection<SettingNode>? Items { get; set; }

    #endregion

    #region Command

    RxCommandUnit? GoForward { get; }
    RxCommandUnit? GoBack { get; }

    #endregion

    #region Helper
    private void setSubTitle(SettingNode? n)
    {
        if (n != null)
        {
            SubIcon = n.Icon;
            SubTitle = n.ParetnNode == null ? n.Name : string.Concat(n.ParetnNode?.Name, " ->", n.Name);

        }
    }
    #endregion

    #region tree

    SettingNode? _appearanceNode;
    SettingNode? _a1;
    SettingNode? _calculateNode;

    void createTree()
    {
        // Номера id должны быть уникальны, по ним потом отбирается что отображать в CurrentView
        _appearanceNode = new(1, Lang.Resources.msg_page_appearance, MaterialIconKind.ViewInAugmentedReality, null);
        _a1 = new(2, Lang.Resources.msg_page_confirmations, MaterialIconKind.StickerCheckOutline, _appearanceNode); _appearanceNode.SubNode = [_a1];
        _calculateNode = new(3, Lang.Resources.msg_page_calc, MaterialIconKind.CalculatorVariantOutline, null);

        SelectedNode = _appearanceNode;
        Items = [_appearanceNode, _calculateNode];
    }
    #endregion

    #region Tabs

    UserControl _appearanceView = new AppearanceView(App.Services.GetRequiredService<IAppHost>());
    UserControl _confrimView = new ConfrimView();
    UserControl _calculationView = new CalculationView();

    #endregion
}
