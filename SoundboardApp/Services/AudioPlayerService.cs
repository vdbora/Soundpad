using System;
using System.IO;
using NAudio.Wave;

namespace SoundboardApp.Services
{
    /// <summary>
    /// Audio playback service using NAudio
    /// </summary>
    public class AudioPlayerService : IDisposable
    {
        private IWavePlayer? _wavePlayer;
        private AudioFileReader? _audioFileReader;
        private bool _disposed;

        /// <summary>
        /// Plays audio file at specified path
        /// </summary>
        public void Play(string filePath)
        {
            try
            {
                Stop();

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Audio file not found: {filePath}");
                }

                _audioFileReader = new AudioFileReader(filePath);
                _wavePlayer = new WaveOutEvent();
                _wavePlayer.Init(_audioFileReader);
                _wavePlayer.Play();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to play audio: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Stops current playback
        /// </summary>
        public void Stop()
        {
            _wavePlayer?.Stop();
            _audioFileReader?.Dispose();
            _wavePlayer?.Dispose();
            _wavePlayer = null;
            _audioFileReader = null;
        }

        /// <summary>
        /// Gets audio file duration in seconds
        /// </summary>
        public int GetDuration(string filePath)
        {
            try
            {
                using var reader = new AudioFileReader(filePath);
                return (int)reader.TotalTime.TotalSeconds;
            }
            catch
            {
                return 0;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            Stop();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
