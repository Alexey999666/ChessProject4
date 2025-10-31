using ChessProject2.Models;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject2.Models
{
    public class Knight : Piece
    {
        public Knight(PieceColor color)
        {
            Color = color;
            Type = PieceType.Knight;
            Symbol = color == PieceColor.White ? "♘" : "♞";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();

            // Все возможные L-образные движения коня
            int[] rowOffsets = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] colOffsets = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = from.Row + rowOffsets[i];
                int newCol = from.Column + colOffsets[i];
                AddMoveIfValid(moves, newRow, newCol, board);
            }

            return moves;
        }
    }
}
