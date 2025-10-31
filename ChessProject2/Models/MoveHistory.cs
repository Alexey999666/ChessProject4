using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject2.Models
{
    public class MoveHistory
    {
        public int MoveNumber { get; set; }
        public string WhiteMove { get; set; }
        public string BlackMove { get; set; }
        public Piece Piece { get; set; }
        public Position From { get; set; }
        public Position To { get; set; }
        public Piece CapturedPiece { get; set; }

        public string DisplayText => $"{MoveNumber}. {WhiteMove} {BlackMove}";
    }
}
