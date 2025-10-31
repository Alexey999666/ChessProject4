using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject2.Models
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color)
        {
            Color = color;
            Type = PieceType.Pawn;
            Symbol = color == PieceColor.White ? "♙" : "♟";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();
            int direction = Color == PieceColor.White ? 1 : -1;

            // Движение вперед на одну клетку
            int newRow = from.Row + direction;
            if (IsInBoard(newRow, from.Column) && IsEmpty(new Position(newRow, from.Column), board))
            {
                moves.Add(new Position(newRow, from.Column));

                // Движение вперед на две клетки (первый ход)
                if (!HasMoved)
                {
                    int doubleRow = from.Row + 2 * direction;
                    if (IsInBoard(doubleRow, from.Column) && IsEmpty(new Position(doubleRow, from.Column), board))
                    {
                        moves.Add(new Position(doubleRow, from.Column));
                    }
                }
            }

            // Взятие по диагонали
            int[] captureCols = { from.Column - 1, from.Column + 1 };
            foreach (int col in captureCols)
            {
                if (IsInBoard(newRow, col))
                {
                    var capturePos = new Position(newRow, col);
                    if (HasEnemyPiece(capturePos, board))
                    {
                        moves.Add(capturePos);
                    }
                }
            }

            return moves;
        }
    }
}
