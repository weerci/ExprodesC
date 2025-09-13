using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExprodesC.Wrappers;

public class SynonymWR : ReactiveObject, IEquatable<SynonymWR>
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

    // Реализация IEquatable<SynonymWR>
    public bool Equals(SynonymWR? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id &&
               Name == other.Name &&
               LocusId == other.LocusId &&
               LocusName == other.LocusName &&
               Ord == other.Ord;
    }

    // Переопределение Equals(object)
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((SynonymWR)obj);
    }

    // Переопределение GetHashCode()
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, LocusId, LocusName, Ord);
    }

    // Операторы сравнения
    public static bool operator ==(SynonymWR? left, SynonymWR? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(SynonymWR? left, SynonymWR? right)
    {
        return !(left == right);
    }

    // Дополнительно: метод Deconstruct для pattern matching (опционально)
    public void Deconstruct(out int id, out string name, out int locusId, out string locusName, out int ord)
    {
        id = Id;
        name = Name;
        locusId = LocusId;
        locusName = LocusName;
        ord = Ord;
    }
}
