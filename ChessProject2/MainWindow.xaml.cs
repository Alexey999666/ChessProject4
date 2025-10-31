using ChessProject2.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessProject2
{
    public partial class MainWindow : Window
    {
        private Board chessBoard;
        private ObservableCollection<BoardSquare> squares;

        // Коллекции для координат
        public ObservableCollection<string> TopCoordinates { get; set; }
        public ObservableCollection<string> BottomCoordinates { get; set; }
        public ObservableCollection<string> LeftCoordinates { get; set; }
        public ObservableCollection<string> RightCoordinates { get; set; }

        private bool isWhitePlayer = true;
        private Position? selectedPosition = null;

        public MainWindow() : this(true) { }

        public MainWindow(bool playAsWhite)
        {
            InitializeComponent();
            isWhitePlayer = playAsWhite;
            InitializeCoordinates();
            InitializeChessBoard();
            DataContext = this;

            UpdateWindowTitle();
            MovesList.ItemsSource = chessBoard.MoveHistory;
        }

        private void UpdateWindowTitle()
        {
            string playerSide = isWhitePlayer ? "белых" : "черных";
            string gameState = "";

            if (chessBoard.IsCheckmate)
                gameState = " - МАТ!";
            else if (chessBoard.IsStalemate)
                gameState = " - ПАТ!";
            else if (chessBoard.IsCheck)
                gameState = " - ШАХ!";
            else if (chessBoard.IsGameOver)
                gameState = " - Игра завершена";

            this.Title = $"Шахматы - Игра за {playerSide}{gameState}";
        }

        private void InitializeCoordinates()
        {
            TopCoordinates = new ObservableCollection<string>();
            BottomCoordinates = new ObservableCollection<string>();
            LeftCoordinates = new ObservableCollection<string>();
            RightCoordinates = new ObservableCollection<string>();

            UpdateCoordinates();
        }

        private void UpdateCoordinates()
        {
            var letters = new[] { "a", "b", "c", "d", "e", "f", "g", "h" };
            var numbers = new[] { "1", "2", "3", "4", "5", "6", "7", "8" };

            TopCoordinates.Clear();
            BottomCoordinates.Clear();
            LeftCoordinates.Clear();
            RightCoordinates.Clear();

            if (isWhitePlayer)
            {
                foreach (var letter in letters)
                {
                    TopCoordinates.Add(letter);
                    BottomCoordinates.Add(letter);
                }
                for (int i = 7; i >= 0; i--)
                {
                    LeftCoordinates.Add(numbers[i]);
                    RightCoordinates.Add(numbers[i]);
                }
            }
            else
            {
                for (int i = 7; i >= 0; i--)
                {
                    TopCoordinates.Add(letters[i]);
                    BottomCoordinates.Add(letters[i]);
                }
                foreach (var number in numbers)
                {
                    LeftCoordinates.Add(number);
                    RightCoordinates.Add(number);
                }
            }
        }

        private void InitializeChessBoard()
        {
            chessBoard = new Board();
            squares = new ObservableCollection<BoardSquare>();
            chessBoard.UpdateGameState();
            UpdateBoard();
            ChessBoard.ItemsSource = squares;
        }

        private void CreateSquare(int row, int col)
        {
            int displayRow = isWhitePlayer ? 7 - row : row;
            int displayCol = isWhitePlayer ? col : 7 - col;

            var piece = chessBoard.Squares[row, col];

            // Определяем цвет фона с учетом выделения и состояния игры
            Brush backgroundColor;
            var position = new Position(row, col);

            if (chessBoard.IsGameOver)
            {
                // При окончании игры выделяем короля под шахом
                if (chessBoard.IsCheckmate && piece is King && piece.Color == chessBoard.CurrentPlayer)
                {
                    backgroundColor = Brushes.Red;
                }
                else
                {
                    backgroundColor = ((displayRow + displayCol) % 2 == 0) ? Brushes.LightGray : Brushes.Brown;
                }
            }
            else if (selectedPosition.HasValue && selectedPosition.Value.Equals(position))
            {
                backgroundColor = Brushes.LightBlue;
            }
            else if (chessBoard.PossibleMoves.Contains(position))
            {
                backgroundColor = Brushes.LightGreen;
            }
            else
            {
                backgroundColor = ((displayRow + displayCol) % 2 == 0) ? Brushes.LightGray : Brushes.Brown;
            }

            var square = new BoardSquare
            {
                Row = row,
                Column = col,
                DisplayRow = displayRow,
                DisplayColumn = displayCol,
                Color = backgroundColor,
                PieceSymbol = piece?.Symbol ?? "",
                PieceColor = piece?.Color == PieceColor.White ? Brushes.White : Brushes.Black
            };

            squares.Add(square);
        }

        private void UpdateBoard()
        {
            squares.Clear();

            if (isWhitePlayer)
            {
                for (int row = 7; row >= 0; row--)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        CreateSquare(row, col);
                    }
                }
            }
            else
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 7; col >= 0; col--)
                    {
                        CreateSquare(row, col);
                    }
                }
            }

            UpdateWindowTitle();

            // Проверяем окончание игры
            if (chessBoard.IsGameOver)
            {
                ShowGameOverWindow();
            }
        }

        private void ShowGameOverWindow()
        {
            var result = new GameResult
            {
                IsCheckmate = chessBoard.IsCheckmate,
                IsStalemate = chessBoard.IsStalemate,
                Winner = chessBoard.Winner
            };

            var gameOverWindow = new GameOverWindow(result, this);
            gameOverWindow.Owner = this;
            gameOverWindow.ShowDialog();
        }

        public void SwitchPlayerSide()
        {
            if (chessBoard.IsGameOver) return;

            isWhitePlayer = !isWhitePlayer;
            UpdateCoordinates();
            UpdateBoard();
        }

        private void SwitchSide_Click(object sender, RoutedEventArgs e)
        {
            SwitchPlayerSide();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chessBoard.IsGameOver) return;

            var border = sender as Border;
            var square = border?.DataContext as BoardSquare;

            if (square != null)
            {
                var position = new Position(square.Row, square.Column);
                chessBoard.SelectPosition(position);
                selectedPosition = chessBoard.SelectedPosition;
                UpdateBoard();
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем текущее окно и открываем меню
            MainMenuWindow menuWindow = new MainMenuWindow();
            menuWindow.Show();
            this.Close();
        }
        private void Resign_Click(object sender, RoutedEventArgs e)
        {
            var result = new GameResult
            {
                IsCheckmate = true,
                IsStalemate = false,
                Winner = isWhitePlayer ? PieceColor.Black : PieceColor.White
            };

            var gameOverWindow = new GameOverWindow(result, this);
            gameOverWindow.Owner = this;
            gameOverWindow.ShowDialog();
        }

        // Свойство для привязки кнопки смены стороны
        public bool IsFirstMove => chessBoard.MoveHistory.Count == 0;
        public bool IsWhitePlayer => isWhitePlayer;

        public class BoardSquare
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public int DisplayRow { get; set; }
            public int DisplayColumn { get; set; }
            public Brush Color { get; set; }
            public string PieceSymbol { get; set; }
            public Brush PieceColor { get; set; }
        }
    }
}

