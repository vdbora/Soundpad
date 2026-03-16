using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SoundboardApp.Models;

namespace SoundboardApp.Services
{
    /// <summary>
    /// Service for exporting/importing sound configurations
    /// </summary>
    public class ConfigExportService
    {
        /// <summary>
        /// Exports sounds to JSON file
        /// </summary>
        public async Task ExportToJsonAsync(List<Sound> sounds, string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(sounds, Formatting.Indented);
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Imports sounds from JSON file
        /// </summary>
        public async Task<List<Sound>> ImportFromJsonAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var sounds = JsonConvert.DeserializeObject<List<Sound>>(json);
                return sounds ?? new List<Sound>();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to import: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates CSV export of sound statistics
        /// </summary>
        public async Task ExportStatsToCsvAsync(List<Sound> sounds, string filePath)
        {
            try
            {
                var lines = new List<string>
                {
                    "Name,Category,Times Played,Duration,Date Added,Last Played"
                };

                foreach (var sound in sounds)
                {
                    lines.Add($"\"{sound.Name}\",\"{sound.Category}\",{sound.TimesPlayed}," +
                             $"\"{sound.DurationText}\",\"{sound.DateAdded:yyyy-MM-dd}\"," +
                             $"\"{sound.LastPlayedText}\"");
                }

                await File.WriteAllLinesAsync(filePath, lines);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to export CSV: {ex.Message}", ex);
            }
        }
    }
}
