using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Models;

namespace SoundPlayer.Models
{
    public class SoundFolder
    {
        public string Name { get; set; }

        public List<Sound> Sounds { get; set; }
            = new List<Sound>();
    }
}
