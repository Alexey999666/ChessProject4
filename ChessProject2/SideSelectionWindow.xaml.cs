using System;
using System.Windows;

namespace ChessProject2
{
    public partial class SideSelectionWindow : Window
    {
        public bool? IsWhitePlayer { get; private set; }

        public SideSelectionWindow()
        {
            InitializeComponent();
        }

        private void PlayAsWhite_Click(object sender, RoutedEventArgs e)
        {
            IsWhitePlayer = true;
            StartGame();
        }

        private void PlayAsBlack_Click(object sender, RoutedEventArgs e)
        {
            IsWhitePlayer = false;
            StartGame();
        }

        private void PlayAsRandom_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            IsWhitePlayer = random.Next(2) == 0; // 50/50 шанс
            StartGame();
        }

        private void StartGame()
        {
            MainWindow gameWindow = new MainWindow(IsWhitePlayer.Value);
            gameWindow.Show();
            this.Close();
        }
    }
}
