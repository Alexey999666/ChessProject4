using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject2.Models
{
    public abstract class Piece
    {
        public PieceColor Color { get; protected set; }
        public PieceType Type { get; protected set; }
        public string Symbol { get; protected set; }
        public bool HasMoved { get; set; } = false;

        // Абстрактный метод для получения возможных ходов
        public abstract List<Position> GetPossibleMoves(Position from, Board board);

        // Общий метод для проверки, находится ли позиция в пределах доски
        protected bool IsInBoard(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        // Проверка, есть ли фигура в указанной позиции
        protected bool HasPiece(Position pos, Board board)
        {
            return board.GetPieceAt(pos) != null;
        }

        // Проверка, вражеская ли фигура в указанной позиции
        protected bool HasEnemyPiece(Position pos, Board board)
        {
            var piece = board.GetPieceAt(pos);
            return piece != null && piece.Color != this.Color;
        }

        // Проверка, союзная ли фигура в указанной позиции
        protected bool HasAllyPiece(Position pos, Board board)
        {
            var piece = board.GetPieceAt(pos);
            return piece != null && piece.Color == this.Color;
        }

        // Проверка, пустая ли клетка
        protected bool IsEmpty(Position pos, Board board)
        {
            return board.GetPieceAt(pos) == null;
        }

        // Добавление хода, если он валидный
        protected void AddMoveIfValid(List<Position> moves, int row, int col, Board board)
        {
            var target = new Position(row, col);
            if (IsInBoard(row, col) && !HasAllyPiece(target, board))
            {
                moves.Add(target);
            }
        }
    }
}
