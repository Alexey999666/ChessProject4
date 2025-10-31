using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Media.Media3D;

namespace ChessProject2.Models
{
    public class Board
    {
        public Piece[,] Squares { get; private set; }
        public PieceColor CurrentPlayer { get; private set; }
        public ObservableCollection<MoveHistory> MoveHistory { get; private set; }
        public Position? SelectedPosition { get; private set; }
        public List<Position> PossibleMoves { get; private set; }

        public bool IsCheck { get; private set; }
        public bool IsCheckmate { get; private set; }
        public bool IsStalemate { get; private set; }
        public bool IsGameOver => IsCheckmate || IsStalemate;
        public PieceColor Winner { get; private set; }

        public Board()
        {
            Squares = new Piece[8, 8];
            CurrentPlayer = PieceColor.White;
            MoveHistory = new ObservableCollection<MoveHistory>();
            PossibleMoves = new List<Position>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Squares[row, col] = null;
                }
            }

            SetupPiecesForColor(PieceColor.White, 0, 1);
            SetupPiecesForColor(PieceColor.Black, 7, 6);
        }

        private void SetupPiecesForColor(PieceColor color, int backRow, int pawnRow)
        {
            Squares[backRow, 0] = new Rook(color);
            Squares[backRow, 7] = new Rook(color);
            Squares[backRow, 1] = new Knight(color);
            Squares[backRow, 6] = new Knight(color);
            Squares[backRow, 2] = new Bishop(color);
            Squares[backRow, 5] = new Bishop(color);
            Squares[backRow, 3] = new Queen(color);
            Squares[backRow, 4] = new King(color);

            for (int col = 0; col < 8; col++)
            {
                Squares[pawnRow, col] = new Pawn(color);
            }
        }

        public Piece GetPieceAt(Position position)
        {
            if (position.Row < 0 || position.Row >= 8 || position.Column < 0 || position.Column >= 8)
                return null;

            return Squares[position.Row, position.Column];
        }

        public List<Position> GetPossibleMoves(Position from)
        {
            var piece = GetPieceAt(from);
            if (piece == null || piece.Color != CurrentPlayer)
                return new List<Position>();

            // Получаем все возможные ходы без проверки шаха
            var allMoves = piece.GetPossibleMoves(from, this);

            // Фильтруем ходы, которые оставляют короля под шахом
            var safeMoves = new List<Position>();
            foreach (var move in allMoves)
            {
                if (IsMoveSafe(from, move, piece.Color))
                {
                    safeMoves.Add(move);
                }
            }

            return safeMoves;
        }

        /// <summary>
        /// Проверяет, безопасен ли ход (не оставляет короля под шахом)
        /// </summary>
        private bool IsMoveSafe(Position from, Position to, PieceColor movingColor)
        {
            // Сохраняем состояние доски
            var originalPieceFrom = GetPieceAt(from);
            var originalPieceTo = GetPieceAt(to);

            // Выполняем временный ход
            Squares[to.Row, to.Column] = originalPieceFrom;
            Squares[from.Row, from.Column] = null;

            // Проверяем, находится ли король под шахом после хода
            bool isInCheck = IsKingInCheck(movingColor);

            // Восстанавливаем доску
            Squares[from.Row, from.Column] = originalPieceFrom;
            Squares[to.Row, to.Column] = originalPieceTo;

            return !isInCheck;
        }

        /// <summary>
        /// Проверяет, находится ли король указанного цвета под шахом
        /// </summary>
        public bool IsKingInCheck(PieceColor kingColor)
        {
            Position kingPosition = FindKingPosition(kingColor);
            if (kingPosition.Row == -1) return false;

            PieceColor opponentColor = kingColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // Проверяем, может ли любая фигура противника атаковать короля
            // ИСПОЛЬЗУЕМ БАЗОВЫЕ ВОЗМОЖНОСТИ ФИГУР БЕЗ ПРОВЕРКИ БЕЗОПАСНОСТИ ХОДОВ
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var position = new Position(row, col);
                    var piece = GetPieceAt(position);

                    if (piece != null && piece.Color == opponentColor)
                    {
                        // Используем базовые возможности фигур без рекурсии
                        var opponentMoves = GetBasicPossibleMoves(position, piece);
                        if (opponentMoves.Contains(kingPosition))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Получает базовые возможные ходы без проверки безопасности (чтобы избежать рекурсии)
        /// </summary>
        private List<Position> GetBasicPossibleMoves(Position from, Piece piece)
        {
            // Для каждой фигуры получаем ходы по своим правилам
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return GetBasicPawnMoves(from, (Pawn)piece);
                case PieceType.Rook:
                    return GetBasicRookMoves(from, (Rook)piece);
                case PieceType.Knight:
                    return GetBasicKnightMoves(from, (Knight)piece);
                case PieceType.Bishop:
                    return GetBasicBishopMoves(from, (Bishop)piece);
                case PieceType.Queen:
                    return GetBasicQueenMoves(from, (Queen)piece);
                case PieceType.King:
                    return GetBasicKingMoves(from, (King)piece);
                default:
                    return new List<Position>();
            }
        }

        private List<Position> GetBasicPawnMoves(Position from, Pawn pawn)
        {
            List<Position> moves = new List<Position>();
            int direction = pawn.Color == PieceColor.White ? 1 : -1;

            // Движение вперед
            int newRow = from.Row + direction;
            if (IsInBoard(newRow, from.Column) && IsEmpty(new Position(newRow, from.Column)))
            {
                moves.Add(new Position(newRow, from.Column));

                if (!pawn.HasMoved)
                {
                    int doubleRow = from.Row + 2 * direction;
                    if (IsInBoard(doubleRow, from.Column) && IsEmpty(new Position(doubleRow, from.Column)))
                    {
                        moves.Add(new Position(doubleRow, from.Column));
                    }
                }
            }

            // Взятие
            int[] captureCols = { from.Column - 1, from.Column + 1 };
            foreach (int col in captureCols)
            {
                if (IsInBoard(newRow, col))
                {
                    var capturePos = new Position(newRow, col);
                    if (HasEnemyPiece(capturePos, pawn.Color))
                    {
                        moves.Add(capturePos);
                    }
                }
            }

            return moves;
        }

        private List<Position> GetBasicRookMoves(Position from, Rook rook)
        {
            List<Position> moves = new List<Position>();
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
                    if (HasAllyPiece(target, rook.Color)) break;

                    moves.Add(target);
                    if (HasEnemyPiece(target, rook.Color)) break;
                }
            }

            return moves;
        }

        private List<Position> GetBasicKnightMoves(Position from, Knight knight)
        {
            List<Position> moves = new List<Position>();
            int[] rowOffsets = { -2, -2, -1, -1, 1, 1, 2, 2 };
            int[] colOffsets = { -1, 1, -2, 2, -2, 2, -1, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = from.Row + rowOffsets[i];
                int newCol = from.Column + colOffsets[i];
                if (IsInBoard(newRow, newCol) && !HasAllyPiece(new Position(newRow, newCol), knight.Color))
                {
                    moves.Add(new Position(newRow, newCol));
                }
            }

            return moves;
        }

        private List<Position> GetBasicBishopMoves(Position from, Bishop bishop)
        {
            List<Position> moves = new List<Position>();
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
                    if (HasAllyPiece(target, bishop.Color)) break;

                    moves.Add(target);
                    if (HasEnemyPiece(target, bishop.Color)) break;
                }
            }

            return moves;
        }

        private List<Position> GetBasicQueenMoves(Position from, Queen queen)
        {
            // Комбинация ладьи и слона
            var moves = GetBasicRookMoves(from, new Rook(queen.Color));
            moves.AddRange(GetBasicBishopMoves(from, new Bishop(queen.Color)));
            return moves;
        }

        private List<Position> GetBasicKingMoves(Position from, King king)
        {
            List<Position> moves = new List<Position>();
            int[] rowOffsets = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] colOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int i = 0; i < 8; i++)
            {
                int newRow = from.Row + rowOffsets[i];
                int newCol = from.Column + colOffsets[i];
                if (IsInBoard(newRow, newCol) && !HasAllyPiece(new Position(newRow, newCol), king.Color))
                {
                    moves.Add(new Position(newRow, newCol));
                }
            }

            return moves;
        }

        // Вспомогательные методы для проверки состояния клеток
        private bool IsInBoard(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        private bool IsEmpty(Position pos)
        {
            return GetPieceAt(pos) == null;
        }

        private bool HasAllyPiece(Position pos, PieceColor color)
        {
            var piece = GetPieceAt(pos);
            return piece != null && piece.Color == color;
        }

        private bool HasEnemyPiece(Position pos, PieceColor color)
        {
            var piece = GetPieceAt(pos);
            return piece != null && piece.Color != color;
        }

        /// <summary>
        /// Находит позицию короля указанного цвета
        /// </summary>
        private Position FindKingPosition(PieceColor kingColor)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = Squares[row, col];
                    if (piece is King && piece.Color == kingColor)
                    {
                        return new Position(row, col);
                    }
                }
            }
            return new Position(-1, -1);
        }

        /// <summary>
        /// Проверяет, есть ли у игрока возможные ходы
        /// </summary>
        private bool HasAnyValidMoves(PieceColor playerColor)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var position = new Position(row, col);
                    var piece = GetPieceAt(position);

                    if (piece != null && piece.Color == playerColor)
                    {
                        var moves = GetPossibleMoves(position);
                        if (moves.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Обновляет состояние игры (шах, мат, пат)
        /// </summary>
        public void UpdateGameState()
        {
            IsCheck = IsKingInCheck(CurrentPlayer);

            if (!HasAnyValidMoves(CurrentPlayer))
            {
                if (IsCheck)
                {
                    IsCheckmate = true;
                    Winner = CurrentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                }
                else
                {
                    IsStalemate = true;
                }
            }
            else
            {
                IsCheckmate = false;
                IsStalemate = false;
            }
        }

        public void SelectPosition(Position position)
        {
            if (IsGameOver) return;

            var piece = GetPieceAt(position);

            if (piece != null && piece.Color == CurrentPlayer)
            {
                SelectedPosition = position;
                PossibleMoves = GetPossibleMoves(position);
            }
            else if (SelectedPosition.HasValue && PossibleMoves.Contains(position))
            {
                // Выполняем ход
                if (MovePiece(SelectedPosition.Value, position))
                {
                    // После успешного хода обновляем состояние игры
                    UpdateGameState();
                }
                SelectedPosition = null;
                PossibleMoves.Clear();
            }
            else
            {
                SelectedPosition = null;
                PossibleMoves.Clear();
            }
        }

        public bool MovePiece(Position from, Position to)
        {
            var piece = GetPieceAt(from);
            if (piece == null || piece.Color != CurrentPlayer)
                return false;

            var possibleMoves = GetPossibleMoves(from);
            if (!possibleMoves.Contains(to))
                return false;

            var capturedPiece = GetPieceAt(to);

            RecordMove(piece, from, to, capturedPiece);

            Squares[to.Row, to.Column] = piece;
            Squares[from.Row, from.Column] = null;
            piece.HasMoved = true;

            CurrentPlayer = CurrentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;

            return true;
        }

        private void RecordMove(Piece piece, Position from, Position to, Piece capturedPiece)
        {
            string moveNotation = GetMoveNotation(piece, from, to, capturedPiece);
            int moveNumber = (MoveHistory.Count + 2) / 2;

            if (CurrentPlayer == PieceColor.White)
            {
                if (MoveHistory.Count > 0)
                {
                    var lastMove = MoveHistory.Last();
                    lastMove.BlackMove = moveNotation;
                }
            }
            else
            {
                var history = new MoveHistory
                {
                    MoveNumber = moveNumber,
                    WhiteMove = moveNotation,
                    BlackMove = "",
                    Piece = piece,
                    From = from,
                    To = to,
                    CapturedPiece = capturedPiece
                };
                MoveHistory.Add(history);
            }
        }

        private string GetMoveNotation(Piece piece, Position from, Position to, Piece capturedPiece)
        {
            char file = (char)('a' + to.Column);
            int rank = 8 - to.Row;
            string capture = capturedPiece != null ? "x" : "";

            return $"{piece.Symbol}{capture}{file}{rank}";
        }
    }
}
