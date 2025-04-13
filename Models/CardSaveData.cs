using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoryGame.Models
{
    public class CardSaveData
    {
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Position { get; set; }
    }
}
