using System;

namespace SoundboardApp.Interfaces
{
    /// <summary>
    /// Interface for sortable items
    /// </summary>
    public interface ISortable
    {
        string Name { get; }
        int TimesPlayed { get; }
        DateTime DateAdded { get; }
        string Category { get; }
    }
}
