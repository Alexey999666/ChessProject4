using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject2.Models
{
    public struct Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"{(char)('a' + Column)}{8 - Row}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position)) return false;
            Position other = (Position)obj;
            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            return (Row * 397) ^ Column;
        }
    }
}
