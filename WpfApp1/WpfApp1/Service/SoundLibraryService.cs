using SoundPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    public class SoundLibraryService
    {
        public List<SoundFolder> Load()
        {
            return new List<SoundFolder>()
            {
                new SoundFolder()
                {
                    Name = "Folder 1",
                    Sounds = new List<Sound>()
                    {
                        new Sound()
                        {
                            Name = "Test",
                            FilePath = @"C:\Sounds\test.mp3"
                        }
                    }
                }
            };
        }
    }
}
