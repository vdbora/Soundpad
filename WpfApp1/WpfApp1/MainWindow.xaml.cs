using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SoundPlayer.Models;
using WpfApp1.Models;
using WpfApp1.Services;
using WpfApp1;

namespace SoundPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private AudioPlayer audioPlayer = new AudioPlayer();

        public MainWindow()
        {
            InitializeComponent();
            // default folder
            tabSounds.SelectedIndex = 0;
        }

        SoundLibraryService library = new SoundLibraryService();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var folders = library.Load();

            lbxFolder1.ItemsSource = folders[0].Sounds;
        }


        //private void btnPlay_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lbxSounds.SelectedItem is Sound sound)
        //    {
        //        audioPlayer.Play(sound.FilePath);
        //    }
        //}

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Stop();
        }

        private void sldrVolume_ValueChanged(object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            audioPlayer.SetVolume(sldrVolume.Value / 100);
        }

      
          
          


            private void BtnFolder1_Click(object sender, RoutedEventArgs e)
            {
                tabSounds.SelectedIndex = 0;
            }

            private void BtnFolder2_Click(object sender, RoutedEventArgs e)
            {
                tabSounds.SelectedIndex = 1;
            }

            private void BtnFolder3_Click(object sender, RoutedEventArgs e)
            {
                tabSounds.SelectedIndex = 2;
            }


    }

    
}