using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class GameSaveData
    {
        public string Username { get; set; }
        public string Category { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int TimeRemaining { get; set; }
        public int TimeElapsed { get; set; }
        public List<CardSaveData> Cards { get; set; }
    }
}
