using Avalonia;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Extensions;

public static class VisualExtensions
{
    public static T? ExFindAncestorOfType<T>(this Visual visual) where T : Visual
    {
        var parent = visual.Parent as Visual;
        while (parent != null)
        {
            if (parent is T result)
                return result;
            parent = parent.Parent as Visual;
        }
        return null;
    }

    public static T? ExFindDescendantOfType<T>(this Visual visual, Func<T, bool>? predicate = null) where T : Visual
    {
        foreach (var child in visual.GetVisualChildren())
        {
            if (child is T result && (predicate == null || predicate(result)))
                return result;

            var descendant = child.ExFindDescendantOfType<T>(predicate);
            if (descendant != null)
                return descendant;
        }
        return null;
    }
}
