using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoundboardApp.Data;
using SoundboardApp.Models;

namespace SoundboardApp.Services
{
    /// <summary>
    /// Repository for sound database operations using LINQ
    /// </summary>
    public class SoundRepositoryService : IDisposable
    {
        private readonly SoundDbContext _context;
        private bool _disposed;

        public SoundRepositoryService()
        {
            _context = new SoundDbContext();
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Gets all sounds from database
        /// </summary>
        public async Task<List<Sound>> GetAllSoundsAsync()
        {
            return await _context.Sounds
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets sounds filtered by category
        /// </summary>
        public async Task<List<Sound>> GetSoundsByCategoryAsync(string category)
        {
            return await _context.Sounds
                .Where(s => s.Category == category)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all unique categories
        /// </summary>
        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Sounds
                .Select(s => s.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        /// <summary>
        /// Searches sounds by name or category
        /// </summary>
        public async Task<List<Sound>> SearchSoundsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await GetAllSoundsAsync();

            var lowerQuery = query.ToLower();
            return await _context.Sounds
                .Where(s => s.Name.ToLower().Contains(lowerQuery) ||
                           s.Category.ToLower().Contains(lowerQuery))
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Adds new sound to database
        /// </summary>
        public async Task<Sound> AddSoundAsync(Sound sound)
        {
            sound.DateAdded = DateTime.Now;
            _context.Sounds.Add(sound);
            await _context.SaveChangesAsync();
            return sound;
        }

        /// <summary>
        /// Updates existing sound
        /// </summary>
        public async Task UpdateSoundAsync(Sound sound)
        {
            _context.Sounds.Update(sound);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes sound from database
        /// </summary>
        public async Task DeleteSoundAsync(int soundId)
        {
            var sound = await _context.Sounds.FindAsync(soundId);
            if (sound != null)
            {
                _context.Sounds.Remove(sound);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets total play count across all sounds
        /// </summary>
        public async Task<int> GetTotalPlaysAsync()
        {
            return await _context.Sounds
                .SumAsync(s => s.TimesPlayed);
        }

        /// <summary>
        /// Gets most played sounds
        /// </summary>
        public async Task<List<Sound>> GetMostPlayedAsync(int count = 10)
        {
            return await _context.Sounds
                .OrderByDescending(s => s.TimesPlayed)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recently added sounds
        /// </summary>
        public async Task<List<Sound>> GetRecentlyAddedAsync(int count = 10)
        {
            return await _context.Sounds
                .OrderByDescending(s => s.DateAdded)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Gets recently played sounds
        /// </summary>
        public async Task<List<Sound>> GetRecentlyPlayedAsync(int count = 10)
        {
            return await _context.Sounds
                .Where(s => s.LastPlayed != null)
                .OrderByDescending(s => s.LastPlayed)
                .Take(count)
                .ToListAsync();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _context.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
