namespace SoundboardApp.Interfaces
{
    /// <summary>
    /// Interface for audio playback functionality
    /// </summary>
    public interface IAudioPlayable
    {
        void Play();
        void Stop();
        string GetFilePath();
    }
}
