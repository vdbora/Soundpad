namespace SoundboardApp.Interfaces
{
    /// <summary>
    /// Interface for searchable sound items
    /// </summary>
    public interface ISearchable
    {
        bool MatchesSearch(string searchQuery);
    }
}
