using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.Services
{
    public class AudioPlayer
    {
        private MediaPlayer player = new MediaPlayer();

        public void Play(string path)
        {
            player.Open(new Uri(path));
            player.Play();
        }

        public void Pause()
        {
            player.Pause();
        }

        public void Stop()
        {
            player.Stop();
        }

        public void SetVolume(double volume)
        {
            player.Volume = volume;
        }
    }
}