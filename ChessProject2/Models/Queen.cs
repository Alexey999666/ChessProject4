using System.Collections.Generic;
using System.Windows.Documents;

namespace ChessProject2.Models
{
    public class Queen : Piece
    {
        public Queen(PieceColor color)
        {
            Color = color;
            Type = PieceType.Queen;
            Symbol = color == PieceColor.White ? "♕" : "♛";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();

            // Комбинация движений ладьи и слона
            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
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
