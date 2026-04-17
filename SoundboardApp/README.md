# Professional Soundboard Application

A modern WPF desktop application for managing and playing audio files with advanced features.

## Features

### Core Functionality
- **Audio Playback**: Play multiple audio formats (MP3, WAV, OGG, FLAC, M4A)
- **Sound Management**: Add and delete sounds with file picker
- **Search**: Real-time search by sound name or category
- **Filtering**: Filter sounds by category
- **Sorting**: Sort by name, play count, date added, or category
- **Statistics**: Track total sounds and play counts
- **Recent History**: View recently played sounds

### Technical Implementation

#### ✅ Requirements Coverage

1. **WPF/XAML**: Hand-coded XAML with no drag-and-drop
2. **Classes/Objects**: Sound, Services, ViewModels
3. **Inheritance**: ViewModelBase, IAudioPlayable interfaces
4. **Interfaces**: IAudioPlayable, ISearchable, ISortable
5. **Sorting/Filtering/Searching**: LINQ-based with CollectionView
6. **Lists/ObservableCollection**: Used throughout for data binding
7. **Event Handling**: Button clicks, property changes, command bindings
8. **Working with Dates**: DateAdded, LastPlayed with formatting
9. **Database**: SQLite with Entity Framework Core
10. **LINQ**: Extensive use in SoundRepositoryService
11. **Multiple Windows/Navigation**: TabControl for multiple screens
12. **JSON**: Export/Import configuration with Newtonsoft.Json
13. **Images**: Icon buttons and visual elements
14. **Styles**: CommonStyles.xaml with Material Design influence
15. **Data Templates**: ListBox and DataGrid templates
16. **Exception Handling**: Try-catch blocks with user feedback
17. **Testing**: Unit tests with xUnit
18. **Extra Feature**: NAudio integration for advanced audio playback

## Project Structure

```
SoundboardApp/
├── Models/
│   └── Sound.cs                    # Sound entity with interfaces
├── ViewModels/
│   ├── ViewModelBase.cs           # Base MVVM class
│   └── MainViewModel.cs           # Main app logic
├── Views/
│   └── MainWindow.xaml            # Main UI
├── Services/
│   ├── AudioPlayerService.cs      # NAudio playback
│   ├── SoundRepositoryService.cs  # Database operations with LINQ
│   └── ConfigExportService.cs     # JSON import/export
├── Data/
│   └── SoundDbContext.cs          # Entity Framework context
├── Interfaces/
│   ├── IAudioPlayable.cs          # Playback interface
│   ├── ISearchable.cs             # Search interface
│   └── ISortable.cs               # Sort interface
├── Helpers/
│   └── RelayCommand.cs            # MVVM command helper
├── Styles/
│   └── CommonStyles.xaml          # Application styles
└── Tests/
    └── SoundTests.cs              # Unit tests
```

## Architecture Patterns

### MVVM (Model-View-ViewModel)
- **Models**: Sound entity with data and business logic
- **ViewModels**: MainViewModel handles UI logic and commands
- **Views**: XAML-based user interface with data binding

### Repository Pattern
- SoundRepositoryService abstracts database operations
- Async/await for non-blocking database access

### Dependency Management
- Services injected in ViewModel constructor
- IDisposable implementation for resource cleanup

## Database Schema

### Sound Table
```sql
CREATE TABLE Sounds (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    FilePath TEXT NOT NULL,
    Category TEXT,
    TimesPlayed INTEGER DEFAULT 0,
    DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastPlayed DATETIME,
    DurationSeconds INTEGER
);
```

## LINQ Examples

```csharp
// Get all sounds ordered by name
await _context.Sounds
    .OrderBy(s => s.Name)
    .ToListAsync();

// Search with filtering
await _context.Sounds
    .Where(s => s.Name.ToLower().Contains(query) || 
                s.Category.ToLower().Contains(query))
    .OrderBy(s => s.Name)
    .ToListAsync();

// Get most played sounds
await _context.Sounds
    .OrderByDescending(s => s.TimesPlayed)
    .Take(10)
    .ToListAsync();

// Get unique categories
await _context.Sounds
    .Select(s => s.Category)
    .Distinct()
    .OrderBy(c => c)
    .ToListAsync();
```

## Usage

### Adding Sounds
1. Navigate to "Manage Sounds" tab
2. Click "Add Sounds" button
3. Select one or more audio files
4. Files are automatically added to database

### Playing Sounds
1. Navigate to "Play Sounds" tab
2. Use search box to find sounds
3. Filter by category dropdown
4. Sort using sort dropdown
5. Click "Play" button on any sound

### Managing Sounds
1. Edit categories directly in DataGrid
2. Select sound and click "Delete Selected"
3. View statistics in header

### Export/Import
- Export: Click "Export" → Choose JSON or CSV
- Import: Click "Import" → Select JSON file

## Building and Running

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Windows OS (WPF requirement)

### Build Commands
```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run

# Run tests
dotnet test Tests/SoundboardApp.Tests.csproj
```

### NuGet Packages
- Microsoft.EntityFrameworkCore (8.0.0)
- Microsoft.EntityFrameworkCore.Sqlite (8.0.0)
- NAudio (2.2.1)
- Newtonsoft.Json (13.0.3)
- xUnit (2.6.2)

## Error Handling

All operations include try-catch blocks with:
- User-friendly error messages
- Exception details in MessageBox
- Graceful degradation

## Code Quality

### Defensive Coding
- Null checks on all operations
- File existence validation
- Database connection handling
- Resource disposal (IDisposable)

### Code Comments
- XML documentation comments (3-10 words)
- Function purpose clearly stated
- Complex logic explained

### Testing
- Unit tests for core functionality
- Model behavior validation
- Search and filter testing

## Performance Optimizations

- Async/await for database operations
- CollectionView for efficient filtering
- ObservableCollection for automatic updates
- Proper resource disposal

## Future Enhancements

- Global hotkey support
- Audio effects and filters
- Playlist management
- Cloud sync
- Custom themes
- Waveform visualization

## License

Educational project for course requirements.

############################################## this document i ask to create in chat gpt