using System;
using Xunit;
using SoundboardApp.Models;

namespace SoundboardApp.Tests
{
    /// <summary>
    /// Tests for Sound model functionality
    /// </summary>
    public class SoundTests
    {
        [Fact]
        public void Play_IncrementsTimesPlayed()
        {
            // Arrange
            var sound = new Sound { Name = "Test", FilePath = "test.mp3" };
            var initialCount = sound.TimesPlayed;

            // Act
            sound.Play();

            // Assert
            Assert.Equal(initialCount + 1, sound.TimesPlayed);
        }

        [Fact]
        public void Play_UpdatesLastPlayed()
        {
            // Arrange
            var sound = new Sound { Name = "Test", FilePath = "test.mp3" };

            // Act
            sound.Play();

            // Assert
            Assert.NotNull(sound.LastPlayed);
            Assert.True((DateTime.Now - sound.LastPlayed.Value).TotalSeconds < 1);
        }

        [Fact]
        public void MatchesSearch_ReturnsTrue_WhenNameMatches()
        {
            // Arrange
            var sound = new Sound { Name = "Epic Sound", Category = "Music" };

            // Act
            var result = sound.MatchesSearch("epic");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MatchesSearch_ReturnsTrue_WhenCategoryMatches()
        {
            // Arrange
            var sound = new Sound { Name = "Test", Category = "Music" };

            // Act
            var result = sound.MatchesSearch("music");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void MatchesSearch_IsCaseInsensitive()
        {
            // Arrange
            var sound = new Sound { Name = "EPIC SOUND", Category = "MUSIC" };

            // Act
            var result = sound.MatchesSearch("epic sound");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void DurationText_FormatsCorrectly()
        {
            // Arrange
            var sound = new Sound { DurationSeconds = 125 }; // 2:05

            // Act
            var text = sound.DurationText;

            // Assert
            Assert.Equal("02:05", text);
        }

        [Fact]
        public void LastPlayedText_ReturnsNever_WhenNotPlayed()
        {
            // Arrange
            var sound = new Sound();

            // Act
            var text = sound.LastPlayedText;

            // Assert
            Assert.Equal("Never", text);
        }
    }
}
