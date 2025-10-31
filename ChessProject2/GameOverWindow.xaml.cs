using ChessProject2.Models;
using System.Windows;
using System.Windows.Media;

namespace ChessProject2
{
    public partial class GameOverWindow : Window
    {
        public GameResult Result { get; }
        private Window parentGameWindow;

        public GameOverWindow(GameResult result, Window parentWindow)
        {
            InitializeComponent();
            Result = result;
            parentGameWindow = parentWindow;
            DataContext = this;
            SetWindowAppearance();
        }

        private void SetWindowAppearance()
        {
            Background = new SolidColorBrush(Color.FromRgb(222, 184, 135)); // DEB887
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {

            MainMenuWindow menuWindow = new MainMenuWindow();
            menuWindow.Show();
            // Закрываем все игровые окна и открываем меню
            if (parentGameWindow != null)
            {
                parentGameWindow.Close();
            }

           
            this.Close();
        }

        private void Rematch_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем текущее игровое окно и открываем новое
            if (parentGameWindow != null)
            {
                bool playAsWhite = (parentGameWindow as MainWindow)?.IsWhitePlayer ?? true;
                // Меняем цвет игроков при реванше
                bool newPlayAsWhite = !playAsWhite;
                MainWindow newGame = new MainWindow(newPlayAsWhite);
                newGame.Show();
                parentGameWindow.Close();
            }
            this.Close();
        }

        private void Analysis_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Анализ партии будет реализован в будущей версии", "Анализ",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class GameResult
    {
        public bool IsCheckmate { get; set; }
        public bool IsStalemate { get; set; }
        public PieceColor Winner { get; set; }
        public string ResultText => GetResultText();

        private string GetResultText()
        {
            if (IsStalemate) return "Пат! Ничья";
            if (IsCheckmate && Winner == PieceColor.White) return "Мат! Победили белые";
            if (IsCheckmate && Winner == PieceColor.Black) return "Мат! Победили черные";
            return "Игра завершена";
        }
    }
}