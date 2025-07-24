using Material.Icons;
using System.Collections.ObjectModel;

namespace ExprodesC.ViewModels.Settings;

public class SettingNode
{
    public SettingNode(int id, string name, MaterialIconKind icon, SettingNode? paretn, List<SettingNode>? nodes = null)
    {
        Id = id;
        Name = name;
        Icon = icon;
        ParetnNode = paretn;
        SubNode = nodes != null ? new(nodes) : [];
    }
    public int Id { get; }
    public string? Name { get; }
    public MaterialIconKind Icon { get; }
    public SettingNode? ParetnNode { get; }
    public ObservableCollection<SettingNode>? SubNode { get; set; }
}
