using System.Windows;
using SoundboardApp.ViewModels;

namespace SoundboardApp
{
    /// <summary>
    /// Main application window
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
           // InitializeComponent();
            
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            _viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
