using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SoundboardApp.Interfaces;

namespace SoundboardApp.Models
{
    /// <summary>
    /// Sound entity representing audio file
    /// </summary>
    public class Sound : INotifyPropertyChanged, IAudioPlayable, ISearchable, ISortable
    {
        private int _id;
        private string _name = string.Empty;
        private string _filePath = string.Empty;
        private string _category = "Uncategorized";
        private int _timesPlayed;
        private DateTime _dateAdded;
        private DateTime? _lastPlayed;
        private int _durationSeconds;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public int TimesPlayed
        {
            get => _timesPlayed;
            set => SetProperty(ref _timesPlayed, value);
        }

        public DateTime DateAdded
        {
            get => _dateAdded;
            set => SetProperty(ref _dateAdded, value);
        }

        public DateTime? LastPlayed
        {
            get => _lastPlayed;
            set
            {
                if (SetProperty(ref _lastPlayed, value))
                {
                    OnPropertyChanged(nameof(LastPlayedText));
                }
            }
        }

        public int DurationSeconds
        {
            get => _durationSeconds;
            set
            {
                if (SetProperty(ref _durationSeconds, value))
                {
                    OnPropertyChanged(nameof(DurationText));
                }
            }
        }

        /// <summary>
        /// Formatted duration display
        /// </summary>
        public string DurationText => TimeSpan.FromSeconds(DurationSeconds).ToString(@"mm\:ss");

        /// <summary>
        /// Human-readable last played time
        /// </summary>
        public string LastPlayedText
        {
            get
            {
                if (!LastPlayed.HasValue) return "Never";

                var span = DateTime.Now - LastPlayed.Value;
                if (span.TotalMinutes < 1) return "Just now";
                if (span.TotalHours < 1) return $"{(int)span.TotalMinutes}m ago";
                if (span.TotalDays < 1) return $"{(int)span.TotalHours}h ago";
                if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
                return LastPlayed.Value.ToString("MMM dd, yyyy");
            }
        }

        /// <summary>
        /// Increments play count and updates timestamp
        /// </summary>
        public void Play()
        {
            TimesPlayed++;
            LastPlayed = DateTime.Now;
        }

        /// <summary>
        /// Stops playback (handled by service)
        /// </summary>
        public void Stop()
        {
            // Handled by AudioPlayerService
        }

        /// <summary>
        /// Returns file path for playback
        /// </summary>
        public string GetFilePath() => FilePath;

        /// <summary>
        /// Checks if sound matches search query
        /// </summary>
        public bool MatchesSearch(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return true;

            var query = searchQuery.ToLower();
            return Name.ToLower().Contains(query) ||
                   Category.ToLower().Contains(query);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
