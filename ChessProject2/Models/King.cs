using System.Collections.Generic;

namespace ChessProject2.Models
{
    public class King : Piece
    {
        public King(PieceColor color)
        {
            Color = color;
            Type = PieceType.King;
            Symbol = color == PieceColor.White ? "♔" : "♚";
        }

        public override List<Position> GetPossibleMoves(Position from, Board board)
        {
            List<Position> moves = new List<Position>();

            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = from.Row + rowOffsets[i];
                int newCol = from.Column + colOffsets[i];
                AddMoveIfValid(moves, newRow, newCol, board);
            }

            // Рокировка (упрощенная версия)
            if (!HasMoved)
            {
                // Короткая рокировка
                if (IsEmpty(new Position(from.Row, from.Column + 1), board) &&
                    IsEmpty(new Position(from.Row, from.Column + 2), board))
                {
                    var rook = board.GetPieceAt(new Position(from.Row, from.Column + 3));
                    if (rook is Rook && !rook.HasMoved)
                    {
                        AddMoveIfValid(moves, from.Row, from.Column + 2, board);
                    }
                }

                // Длинная рокировка
                if (IsEmpty(new Position(from.Row, from.Column - 1), board) &&
                    IsEmpty(new Position(from.Row, from.Column - 2), board) &&
                    IsEmpty(new Position(from.Row, from.Column - 3), board))
                {
                    var rook = board.GetPieceAt(new Position(from.Row, from.Column - 4));
                    if (rook is Rook && !rook.HasMoved)
                    {
                        AddMoveIfValid(moves, from.Row, from.Column - 2, board);
                    }
                }
            }

            return moves;
        }
    }
}
