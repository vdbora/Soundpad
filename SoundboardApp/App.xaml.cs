using System.Windows;

namespace SoundboardApp
{
    /// <summary>
    /// Application entry point
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Initialize database on startup
            using var context = new Data.SoundDbContext();
            context.Database.EnsureCreated();
        }
    }
}
