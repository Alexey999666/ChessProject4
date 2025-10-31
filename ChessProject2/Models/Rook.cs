using System.Collections.Generic;

namespace ChessProject2.Models
{
    public class Rook : Piece
    {
        public Rook(PieceColor color)
        {
            Color = color;
            Type = PieceType.Rook;
            Symbol = color == PieceColor.White ? "♖" : "♜";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();

            // Движение по горизонтали и вертикали
            int[] rowOffsets = { -1, 1, 0, 0 };
            int[] colOffsets = { 0, 0, -1, 1 };

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
