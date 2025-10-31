using ChessProject2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;



namespace ChessProject2.Models
{
    public class Bishop : Piece
    {
        public Bishop(PieceColor color)
        {
            Color = color;
            Type = PieceType.Bishop;
            Symbol = color == PieceColor.White ? "♗" : "♝";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();

            // Движение по диагоналям
            int[] rowOffsets = { -1, -1, 1, 1 };
            int[] colOffsets = { -1, 1, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                for (int step = 1; step < 8; step++)
                {
                    int newRow = from.Row + rowOffsets[i] * step;
                    int newCol = from.Column + colOffsets[i] * step;

                    if (!IsInBoard(newRow, newCol)) break;

                    var target = new Position(newRow, newCol);
                    if (HasAllyPiece(target, board)) break;

                    moves.Add(target);
                    if (HasEnemyPiece(target, board)) break;
                }
            }

            return moves;
        }
    }
}
