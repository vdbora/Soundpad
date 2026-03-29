using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Win32;
using SoundboardApp.Helpers;
using SoundboardApp.Models;
using SoundboardApp.Services;

namespace SoundboardApp.ViewModels
{
    /// <summary>
    /// Main application view model
    /// </summary>
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private readonly SoundRepositoryService _repository;
        private readonly AudioPlayerService _audioPlayer;
        private readonly ConfigExportService _exportService;

        private ObservableCollection<Sound> _sounds;
        private ICollectionView _soundsView;
        private Sound? _selectedSound;
        private string _searchText = string.Empty;
        private string _selectedCategory = "All";
        private string _sortBy = "Name";
        private int _totalSounds;
        private int _totalPlays;
        private bool _disposed;

        public ObservableCollection<Sound> Sounds
        {
            get => _sounds;
            set => SetProperty(ref _sounds, value);
        }

        public ICollectionView SoundsView
        {
            get => _soundsView;
            set => SetProperty(ref _soundsView, value);
        }

        public Sound? SelectedSound
        {
            get => _selectedSound;
            set => SetProperty(ref _selectedSound, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    ApplyFilters();
                }
            }
        }

        public string SortBy
        {
            get => _sortBy;
            set
            {
                if (SetProperty(ref _sortBy, value))
                {
                    ApplySorting();
                }
            }
        }

        public int TotalSounds
        {
            get => _totalSounds;
            set => SetProperty(ref _totalSounds, value);
        }

        public int TotalPlays
        {
            get => _totalPlays;
            set => SetProperty(ref _totalPlays, value);
        }

        public ObservableCollection<string> Categories { get; }
        public ObservableCollection<string> SortOptions { get; }
        public ObservableCollection<Sound> RecentPlays { get; }

        // Commands
        public ICommand PlaySoundCommand { get; }
        public ICommand AddSoundCommand { get; }
        public ICommand DeleteSoundCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportConfigCommand { get; }
        public ICommand ImportConfigCommand { get; }
        public ICommand StopPlaybackCommand { get; }

        public MainViewModel()
        {
            _repository = new SoundRepositoryService();
            _audioPlayer = new AudioPlayerService();
            _exportService = new ConfigExportService();

            _sounds = new ObservableCollection<Sound>();
            _soundsView = CollectionViewSource.GetDefaultView(_sounds);
            
            Categories = new ObservableCollection<string> { "All" };
            SortOptions = new ObservableCollection<string>
            {
                "Name", "Times Played", "Date Added", "Category"
            };
            RecentPlays = new ObservableCollection<Sound>();

            // Initialize commands
            PlaySoundCommand = new RelayCommand<Sound>(async s => await PlaySoundAsync(s), s => s != null);
            AddSoundCommand = new RelayCommand(async _ => await AddSoundAsync());
            DeleteSoundCommand = new RelayCommand<Sound>(async s => await DeleteSoundAsync(s), s => s != null);
            RefreshCommand = new RelayCommand(async _ => await LoadSoundsAsync());
            ExportConfigCommand = new RelayCommand(async _ => await ExportConfigAsync());
            ImportConfigCommand = new RelayCommand(async _ => await ImportConfigAsync());
            StopPlaybackCommand = new RelayCommand(_ => _audioPlayer.Stop());

            // Load initial data
            _ = LoadSoundsAsync();
        }

        /// <summary>
        /// Loads all sounds from database
        /// </summary>
        private async Task LoadSoundsAsync()
        {
            try
            {
                var sounds = await _repository.GetAllSoundsAsync();
                
                Sounds.Clear();
                foreach (var sound in sounds)
                {
                    Sounds.Add(sound);
                }

                await UpdateStatisticsAsync();
                await UpdateCategoriesAsync();
                await UpdateRecentPlaysAsync();
                
                ApplyFilters();
                ApplySorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sounds: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Plays selected sound file
        /// </summary>
        private async Task PlaySoundAsync(Sound? sound)
        {
            if (sound == null) return;

            try
            {
                if (!File.Exists(sound.FilePath))
                {
                    MessageBox.Show($"Sound file not found: {sound.FilePath}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _audioPlayer.Play(sound.FilePath);
                sound.Play();
                
                await _repository.UpdateSoundAsync(sound);
                await UpdateStatisticsAsync();
                await UpdateRecentPlaysAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens file dialog and adds sound
        /// </summary>
        private async Task AddSoundAsync()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "Audio Files|*.mp3;*.wav;*.ogg;*.flac;*.m4a|All Files|*.*",
                    Multiselect = true,
                    Title = "Select Audio Files"
                };

                if (dialog.ShowDialog() != true) return;

                foreach (var filePath in dialog.FileNames)
                {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var duration = _audioPlayer.GetDuration(filePath);

                    var sound = new Sound
                    {
                        Name = fileName,
                        FilePath = filePath,
                        Category = "Uncategorized",
                        DurationSeconds = duration,
                        DateAdded = DateTime.Now
                    };

                    var added = await _repository.AddSoundAsync(sound);
                    Sounds.Add(added);
                }

                await UpdateStatisticsAsync();
                await UpdateCategoriesAsync();
                SoundsView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding sound: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Deletes selected sound from database
        /// </summary>
        private async Task DeleteSoundAsync(Sound? sound)
        {
            if (sound == null) return;

            try
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{sound.Name}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes) return;

                await _repository.DeleteSoundAsync(sound.Id);
                Sounds.Remove(sound);
                
                await UpdateStatisticsAsync();
                await UpdateCategoriesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting sound: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Exports configuration to JSON file
        /// </summary>
        private async Task ExportConfigAsync()
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "JSON Files|*.json|CSV Files|*.csv",
                    DefaultExt = "json",
                    FileName = $"soundboard_config_{DateTime.Now:yyyyMMdd}.json"
                };

                if (dialog.ShowDialog() != true) return;

                var sounds = Sounds.ToList();
                
                if (dialog.FilterIndex == 1)
                {
                    await _exportService.ExportToJsonAsync(sounds, dialog.FileName);
                }
                else
                {
                    await _exportService.ExportStatsToCsvAsync(sounds, dialog.FileName);
                }

                MessageBox.Show("Configuration exported successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Imports configuration from JSON file
        /// </summary>
        private async Task ImportConfigAsync()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "JSON Files|*.json",
                    Title = "Import Configuration"
                };

                if (dialog.ShowDialog() != true) return;

                var sounds = await _exportService.ImportFromJsonAsync(dialog.FileName);
                
                foreach (var sound in sounds)
                {
                    if (File.Exists(sound.FilePath))
                    {
                        await _repository.AddSoundAsync(sound);
                    }
                }

                await LoadSoundsAsync();
                
                MessageBox.Show("Configuration imported successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Applies search and category filters
        /// </summary>
        private void ApplyFilters()
        {
            SoundsView.Filter = obj =>
            {
                if (obj is not Sound sound) return false;

                // Category filter
                if (SelectedCategory != "All" && sound.Category != SelectedCategory)
                    return false;

                // Search filter
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    var query = SearchText.ToLower();
                    if (!sound.Name.ToLower().Contains(query) &&
                        !sound.Category.ToLower().Contains(query))
                        return false;
                }

                return true;
            };
        }

        /// <summary>
        /// Applies sorting to sounds view
        /// </summary>
        private void ApplySorting()
        {
            SoundsView.SortDescriptions.Clear();

            switch (SortBy)
            {
                case "Name":
                    SoundsView.SortDescriptions.Add(
                        new SortDescription(nameof(Sound.Name), ListSortDirection.Ascending));
                    break;
                case "Times Played":
                    SoundsView.SortDescriptions.Add(
                        new SortDescription(nameof(Sound.TimesPlayed), ListSortDirection.Descending));
                    break;
                case "Date Added":
                    SoundsView.SortDescriptions.Add(
                        new SortDescription(nameof(Sound.DateAdded), ListSortDirection.Descending));
                    break;
                case "Category":
                    SoundsView.SortDescriptions.Add(
                        new SortDescription(nameof(Sound.Category), ListSortDirection.Ascending));
                    SoundsView.SortDescriptions.Add(
                        new SortDescription(nameof(Sound.Name), ListSortDirection.Ascending));
                    break;
            }
        }

        /// <summary>
        /// Updates statistics display
        /// </summary>
        private async Task UpdateStatisticsAsync()
        {
            TotalSounds = Sounds.Count;
            TotalPlays = await _repository.GetTotalPlaysAsync();
        }

        /// <summary>
        /// Updates categories list
        /// </summary>
        private async Task UpdateCategoriesAsync()
        {
            var categories = await _repository.GetCategoriesAsync();
            
            Categories.Clear();
            Categories.Add("All");
            
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        /// <summary>
        /// Updates recent plays list
        /// </summary>
        private async Task UpdateRecentPlaysAsync()
        {
            var recent = await _repository.GetRecentlyPlayedAsync(5);
            
            RecentPlays.Clear();
            foreach (var sound in recent)
            {
                RecentPlays.Add(sound);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _audioPlayer.Dispose();
            _repository.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
