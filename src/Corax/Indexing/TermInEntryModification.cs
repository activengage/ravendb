using System;

namespace Corax.Indexing;

internal struct TermInEntryModification : IEquatable<TermInEntryModification>, IComparable<TermInEntryModification>
{
    public long EntryId;
    public int TermsPerEntryIndex; 
    public short Frequency;

    public override string ToString() => EntryId + ", " + Frequency;

    public bool Equals(TermInEntryModification other)
    {
        return EntryId == other.EntryId && Frequency == other.Frequency;
    }

    public int CompareTo(TermInEntryModification other)
    {
        var entryIdComparison = EntryId.CompareTo(other.EntryId);
        if (entryIdComparison != 0) return entryIdComparison;
        return Frequency.CompareTo(other.Frequency);
    }
}
